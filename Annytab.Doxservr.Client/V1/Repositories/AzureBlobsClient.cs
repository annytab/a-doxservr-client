using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;

namespace Annytab.Doxservr.Client.V1
{
    /// <summary>
    /// This class represents an azure blobs client
    /// </summary>
    public class AzureBlobsClient : IAzureBlobsClient
    {
        #region Variables

        private readonly HttpClient client;
        private readonly AzureBlobOptions options;
        private readonly Random rnd;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new repository
        /// </summary>
        public AzureBlobsClient(HttpClient http_client, IOptions<AzureBlobOptions> options)
        {
            // Set values for instance variables
            this.client = http_client;
            this.options = options.Value;
            this.rnd = new Random();

            // Create a http client
            HttpClientHandler handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.None
            };
            this.client = new HttpClient(handler);
            this.client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            this.client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("*"));
            this.client.Timeout = TimeSpan.FromSeconds(this.options.TimeoutInSeconds);

        } // End of the constructor

        #endregion

        #region Upload methods

        /// <summary>
        /// Upload a file block by block
        /// </summary>
        /// <param name="stream">A stream of the file to upload.</param>
        /// <param name="data">A reference to a put block data object.</param>
        /// <param name="block_size">The size of each block in bytes, max 100 MiB.</param>
        /// <param name="progress">A reference to a provider for progress updates.</param>
        /// <param name="ct">A cancellation token to be able to cancel the upload.</param>
        /// <returns>A reference to block list data, used when uploading the block list.</returns>
        public async Task<DoxservrResponse<BlockListData>> UploadBlocks(Stream stream, PutBlockData data, Int32 block_size, 
            IProgress<Int64> progress = null, CancellationToken ct = default(CancellationToken))
        {
            // Create the response to return
            DoxservrResponse<BlockListData> dr = new DoxservrResponse<BlockListData>();

            // Create variables
            byte[] buffer = null;
            Int32 index = 0;
            Int32 bytes_read = 0;
            MD5 md5 = null;

            // Parallelism
            List<Task> tasks = new List<Task>();
            SemaphoreSlim throttler = new SemaphoreSlim(this.options.DegreeOfParallelism);

            try
            {
                // Set values
                dr.model = new BlockListData();
                dr.model.id = data.id;
                dr.model.block_list = new List<string>();
                buffer = new byte[block_size];
                md5 = MD5.Create();

                // Read block by block until the file is read
                while ((bytes_read = stream.Read(buffer, 0, block_size)) > 0)
                {
                    // Get a block id as a Base64 string
                    string block_id = Convert.ToBase64String(BitConverter.GetBytes(index));

                    // Add the block id to the list
                    dr.model.block_list.Add(block_id);

                    // Increment the index
                    index += 1;

                    // Add to the md5 hash
                    md5.TransformBlock(buffer, 0, bytes_read, null, 0);

                    // Wait for a open slot
                    await throttler.WaitAsync();

                    // Create a new array to hold the block, we need an unique instance and can not use a reference to the buffer
                    byte[] block = new byte[bytes_read];
                    Array.Copy(buffer, block, bytes_read);

                    // Start a task
                    tasks.Add(UploadBlock(data.url, block_id, block, throttler, progress, ct));

                    // Break out from the loop if one task is faulted or canceled
                    if (tasks.Exists(x => x.IsFaulted == true || x.IsCanceled == true) == true)
                    {
                        break;
                    }

                    // Remove tasks that have been completed successfully
                    tasks.RemoveAll(x => x.IsCompleted == true && x.IsFaulted == false && x.IsCanceled == false);

                } // End of while (true)

                // Wait for all tasks to finish
                await Task.WhenAll(tasks);

                // All blocks are uploaded, calculate the md5 hash
                md5.TransformFinalBlock(new byte[0], 0, 0);
                dr.model.file_md5 = Convert.ToBase64String(md5.Hash);
            }
            catch (Exception ex)
            {
                // Delete the blob
                await DeleteBlob(data);

                // Add error data and return
                dr.model = null;
                dr.error = $"UploadBlocks: {data.url}. {ex.ToString()}";
            }
            finally
            {
                // Dispose of objects
                if (md5 != null)
                {
                    md5.Dispose();
                }
                if(throttler != null)
                {
                    throttler.Dispose();
                }
            }

            // Return the response
            return dr;

        } // End of the UploadBlocks method

        /// <summary>
        /// Upload a block
        /// </summary>
        /// <param name="url">The url that you have received in a PutBlockData object.</param>
        /// <param name="block_id">An id for the block as a Base64 encoded string, all id:s must have the same length.</param>
        /// <param name="block">An byte array that holds the block.</param>
        /// <param name="throttler">A reference to a an access lock.</param>
        /// <param name="progress">A reference to a provider for progress updates.</param>
        /// /// <param name="ct">A cancellation token to be able to cancel the upload.</param>
        /// <returns>The number of bytes uploaded as an Int32.</returns>
        public async Task<Int32> UploadBlock(string url, string block_id, byte[] block, SemaphoreSlim throttler, IProgress<Int64> progress, 
            CancellationToken ct = default(CancellationToken))
        {
            // Return bytes uploaded
            Int32 bytes_uploaded = 0;

            // Retry loop
            for (int i = 0; i < this.options.MaxRetries; i++)
            {
                // Throw an exception if cancellation is requested
                ct.ThrowIfCancellationRequested();

                try
                {
                    // Upload the block
                    HttpResponseMessage response = await this.client.PutAsync(url + "&comp=block&blockid=" + WebUtility.UrlEncode(block_id), new ByteArrayContent(block));

                    // Check the status code for the response
                    if (response.IsSuccessStatusCode == true)
                    {
                        // Report progress
                        if (progress != null)
                        {
                            progress.Report(block.Length);
                        }

                        // Break out from the retry loop
                        bytes_uploaded = block.Length;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    // Retry on exception
                    if (i == (this.options.MaxRetries - 1))
                    {
                        // Release the lock
                        throttler.Release();

                        // Throw the exception
                        throw ex;
                    }
                }

                // Sleep between 0 and 1 seconds before trying again
                await Task.Delay(rnd.Next(0, 1000));
                
            } // End of for (int i = 0; i < this.options.MaxRetries; i++)

            // Release the lock
            throttler.Release();

            // Return bytes uploaded
            return bytes_uploaded;

        } // End of the UploadBlock method

        #endregion

        #region Delete methods

        /// <summary>
        /// Delete a blob by using a SAS url
        /// </summary>
        /// <param name="data">A reference to a put block data object</param>
        /// <returns>A doxservr response with a boolean that indicates if the blob was deleted.</returns>
        public async Task<DoxservrResponse<bool>> DeleteBlob(PutBlockData data)
        {
            // Create the response to return
            DoxservrResponse<bool> dr = new DoxservrResponse<bool>();

            try
            {
                // Delete a blob
                HttpResponseMessage response = await this.client.DeleteAsync(data.url);

                // Check the status code for the response
                if (response.IsSuccessStatusCode == true)
                {
                    // Add success data
                    dr.model = true;
                }
                else
                {
                    // Add error data
                    dr.model = false;
                    dr.error = $"DeleteBlob: {data.url}. {await response.Content.ReadAsStringAsync()}";
                }
            }
            catch (Exception ex)
            {
                // Add exception data
                dr.model = false;
                dr.error = $"DeleteBlob: {data.url}. {ex.ToString()}";
            }

            // Return the response
            return dr;

        } // End of the DeleteBlob method

        #endregion

    } // End of the class

} // End of the namespace