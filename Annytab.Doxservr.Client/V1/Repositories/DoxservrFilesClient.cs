using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Annytab.Doxservr.Client.V1
{
    /// <summary>
    /// This class represent a doxservr files client
    /// </summary>
    public class DoxservrFilesClient : IDoxservrFilesClient
    {
        #region Variables

        private readonly HttpClient client;
        private readonly DoxservrOptions options;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new repository
        /// </summary>
        public DoxservrFilesClient(HttpClient http_client, IOptions<DoxservrOptions> options)
        {
            // Set values for instance variables
            this.client = http_client;
            this.options = options.Value;

            // Create a http client
            HttpClientHandler handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.None
            };
            this.client = new HttpClient(handler);
            this.client.BaseAddress = new Uri(this.options.ApiHost.TrimEnd('/') + "/api/v1/files/");
            this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(string.Format("{0}:{1}", this.options.ApiEmail, this.options.ApiPassword))));
            this.client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            this.client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("*"));

        } // End of the constructor

        #endregion

        #region Add methods

        /// <summary>
        /// Send a file to one or multiple receivers
        /// </summary>
        /// <param name="stream">A stream of the file to upload.</param>
        /// <param name="receivers">One or more email addresses to recipients. Each e-mail address should be delimited by comma (,).</param>
        /// <param name="filename">A filename, the extension is used to set the mime-type of the file.</param>
        /// <param name="encoding">You can specify how the text in the file has been encoded so that the recipient can convert the file to a string (ASCII, UTF-8, UTF-16 or UTF-32).</param>
        /// <param name="standard">You can specify the name of the standard applied to create the file. This information is important if the recipient should be able to process the file.</param>
        /// <param name="language">Enter a 2-letter language code according to ISO 639-1 that specifies the language used in the file. If no value is specified, the language code is set to en (English).</param>
        /// <param name="status">Enter 0 if you want to require the file to be signed by all parties or 1 if the file not needs to be signed. If no value is specified, the status is set to 1.</param>
        /// <returns>A doxservr response with a file document</returns>
        public async Task<DoxservrResponse<FileDocument>> Send(Stream stream, string receivers, string filename, string encoding = "", string standard = "", string language = "", string status = "")
        {
            // Create the response to return
            DoxservrResponse<FileDocument> dr = new DoxservrResponse<FileDocument>();

            // Send data as multipart/form-data content
            using (MultipartFormDataContent content = new MultipartFormDataContent())
            {
                // Add content
                content.Add(new StringContent(receivers), "receiver_email");
                content.Add(new StringContent(encoding), "file_encoding");
                content.Add(new StringContent(standard), "standard_name");
                content.Add(new StringContent(language), "language_code");
                content.Add(new StringContent(status), "status");
                content.Add(new StreamContent(stream), "file", filename);

                try
                {
                    // Get the response
                    HttpResponseMessage response = await this.client.PostAsync("send", content);

                    // Check the status code for the response
                    if (response.IsSuccessStatusCode == true)
                    {
                        // Get the data
                        string data = await response.Content.ReadAsStringAsync();

                        // Deserialize the content
                        dr.model = JsonConvert.DeserializeObject<FileDocument>(data);
                    }
                    else
                    {
                        // Get string data
                        string data = await response.Content.ReadAsStringAsync();

                        // Add error data
                        dr.error = $"Send: {filename}. {data}";
                    }
                }
                catch (Exception ex)
                {
                    // Add exception data
                    dr.error = $"Send: {filename}. {ex.ToString()}";
                } 
            }

            // Return the response
            return dr;

        } // End of the Send method

        #endregion

        #region Update methods

        /// <summary>
        /// Sign a file
        /// </summary>
        /// <param name="id">The identity of the file you want to sign.</param>
        /// <param name="method">Enter a code for the signature method to be applied (0: Your signature with your own certificate, 1: Signature with our certificate). Must be stated.</param>
        /// <param name="date">The date when the signature was created, formatted as yyyy-MM-dd. Must be specified and included in the signature's underlying data.</param>
        /// <param name="algorithm">The hashalgorithm applied to the signature (SHA-1, SHA-256, SHA-384 or SHA-512). Must be stated.</param>
        /// <param name="padding">The type of padding applied to the signature (Pkcs1 or Pss). Must be stated.</param>
        /// <param name="value">The hash of the signature encoded as a Base64 string, no BEGIN or END headers should be specified. Must be specified if signature method 0 is applied.</param>
        /// <param name="certificate">Your public certificate encoded as a Base64 string, no BEGIN or END headers should be specified. Must be specified if signature method 0 is applied.</param>
        /// <returns>A doxservr response with a signature</returns>
        public async Task<DoxservrResponse<Signature>> Sign(string id, string method, string date, string algorithm, string padding, string value = "", string certificate = "")
        {
            // Create the response to return
            DoxservrResponse<Signature> dr = new DoxservrResponse<Signature>();

            // Send data as multipart/form-data content
            using (MultipartFormDataContent content = new MultipartFormDataContent())
            {
                // Add content
                content.Add(new StringContent(id), "id");
                content.Add(new StringContent(method), "signature_method");
                content.Add(new StringContent(date), "signature_date");
                content.Add(new StringContent(algorithm), "signature_algorithm");
                content.Add(new StringContent(padding), "signature_padding");
                content.Add(new StringContent(value), "signature_value");
                content.Add(new StringContent(certificate), "signature_certificate");

                try
                {
                    // Get the response
                    HttpResponseMessage response = await this.client.PostAsync($"sign", content);

                    // Check the status code for the response
                    if (response.IsSuccessStatusCode == true)
                    {
                        // Get the data
                        string data = await response.Content.ReadAsStringAsync();

                        // Deserialize the content
                        dr.model = JsonConvert.DeserializeObject<Signature>(data);
                    }
                    else
                    {
                        // Get string data
                        string data = await response.Content.ReadAsStringAsync();

                        // Add error data
                        dr.error = $"Sign: {id}. {data}";
                    }
                }
                catch (Exception ex)
                {
                    // Add exception data
                    dr.error = $"Sign: {id}. {ex.ToString()}";
                }
            }

            // Return the response
            return dr;

        } // End of the Sign method

        /// <summary>
        /// Mark a file as closed
        /// </summary>
        /// <param name="id">The identity of the file you want to mark as closed.</param>
        /// <returns>A doxservr response with a boolean that indicates if the file was closed.</returns>
        public async Task<DoxservrResponse<bool>> MarkAsClosed(string id)
        {
            // Create the response to return
            DoxservrResponse<bool> dr = new DoxservrResponse<bool>();

            try
            {
                // Get the response
                HttpResponseMessage response = await this.client.GetAsync($"mark_as_closed/{id}");

                // Check the status code for the response
                if (response.IsSuccessStatusCode == true)
                {
                    // Add success data
                    dr.model = true;
                }
                else
                {
                    // Get string data
                    string data = await response.Content.ReadAsStringAsync();

                    // Add error data
                    dr.model = false;
                    dr.error = $"MarkAsClosed: {id}. {data}";
                }
            }
            catch (Exception ex)
            {
                // Add exception data
                dr.model = false;
                dr.error = $"MarkAsClosed: {id}. {ex.ToString()}";
            }

            // Return the response
            return dr;

        } // End of the MarkAsClosed method

        /// <summary>
        /// Reset your status for a file
        /// </summary>
        /// <param name="id">The identity of the file you want to mark as closed.</param>
        /// <returns>A doxservr response with a boolean that indicates if the status was reset.</returns>
        public async Task<DoxservrResponse<bool>> ResetStatus(string id)
        {
            // Create the response to return
            DoxservrResponse<bool> dr = new DoxservrResponse<bool>();

            try
            {
                // Get the response
                HttpResponseMessage response = await this.client.GetAsync($"reset_status/{id}");

                // Check the status code for the response
                if (response.IsSuccessStatusCode == true)
                {
                    // Add success data
                    dr.model = true;
                }
                else
                {
                    // Get string data
                    string data = await response.Content.ReadAsStringAsync();

                    // Add error data
                    dr.model = false;
                    dr.error = $"ResetStatus: {id}. {data}";
                }
            }
            catch (Exception ex)
            {
                // Add exception data
                dr.model = false;
                dr.error = $"ResetStatus: {id}. {ex.ToString()}";
            }

            // Return the response
            return dr;

        } // End of the ResetStatus method

        #endregion

        #region Get methods

        /// <summary>
        /// Get a page with file documents
        /// </summary>
        /// <param name="ct">Specify a continuation-string to retrieve the next page. You'll get back such a string if there are more pages.</param>
        /// <param name="party_status">You can filter the list by entering your status as a party (0: New, 1: Sent, 2: Downloaded, 3: Signed).</param>
        /// <param name="file_status">Filter on file status(0: Waiting for signatures, 1: Ready to be used).</param>
        /// <param name="party_closed">Specify whether to retrieve open or closed files (0: Open, 1: Closed).</param>
        /// <param name="page_size">Enter how many items you want in your request. If you do not specify a value, the page size is set to 10.</param>
        /// <returns>A doxservr response with a page of file documents</returns>
        public async Task<DoxservrResponse<FileDocuments>> GetPage(string ct = "", Int32 party_status = -1, Int32 file_status = -1, Int32 party_closed = -1, Int32 page_size = 10)
        {
            // Create the response to return
            DoxservrResponse<FileDocuments> dr = new DoxservrResponse<FileDocuments>();

            // Send data as multipart/form-data content
            using (MultipartFormDataContent content = new MultipartFormDataContent())
            {
                // Add content
                content.Add(new StringContent(party_status.ToString()), "party_status");
                content.Add(new StringContent(file_status.ToString()), "file_status");
                content.Add(new StringContent(party_closed.ToString()), "party_closed");
                content.Add(new StringContent(page_size.ToString()), "page_size");
                content.Add(new StringContent(ct), "continuation");

                try
                {
                    // Get the response
                    HttpResponseMessage response = await this.client.PostAsync($"get_page", content);

                    // Check the status code for the response
                    if (response.IsSuccessStatusCode == true)
                    {
                        // Get string data
                        string data = await response.Content.ReadAsStringAsync();

                        // Deserialize the content
                        dr.model = JsonConvert.DeserializeObject<FileDocuments>(data);
                    }
                    else
                    {
                        // Get string data
                        string data = await response.Content.ReadAsStringAsync();

                        // Add error data
                        dr.error = $"GetPage, ps: {party_status.ToString()}, fs: {file_status.ToString()}, pc: {party_closed.ToString()}, pz: {page_size.ToString()}. {data}";
                    }
                }
                catch (Exception ex)
                {
                    // Add exception data
                    dr.error = $"GetPage, ps: {party_status.ToString()}, fs: {file_status.ToString()}, pc: {party_closed.ToString()}, pz: {page_size.ToString()}. {ex.ToString()}";
                }
            }

            // Return the response
            return dr;

        } // End of the GetPage method

        /// <summary>
        /// Get a file as a stream
        /// </summary>
        /// <param name="id">The identity of the file you want to download.</param>
        /// <param name="stream">A reference to a stream, the response stream will be copied to this stream.</param>
        /// <returns>A doxservr response with a boolean that indicates if the file was downloaded.</returns>
        public async Task<DoxservrResponse<bool>> GetFile(string id, Stream stream)
        {
            // Create the response to return
            DoxservrResponse<bool> dr = new DoxservrResponse<bool>();

            try
            {
                // Get the response
                HttpResponseMessage response = await this.client.GetAsync($"get_file/{id}", HttpCompletionOption.ResponseHeadersRead);

                // Check the status code for the response
                if (response.IsSuccessStatusCode == true)
                {
                    // Get header values
                    ContentDispositionHeaderValue disposition = response.Content.Headers.ContentDisposition;
                    MediaTypeHeaderValue contentType = response.Content.Headers.ContentType;
                    long? contentLength = response.Content.Headers.ContentLength;
                    byte[] contentMD5 = response.Content.Headers.ContentMD5;

                    // Get the stream
                    await response.Content.CopyToAsync(stream);

                    // Add success data
                    dr.model = true;
                }
                else
                {
                    // Get string data
                    string data = await response.Content.ReadAsStringAsync();

                    // Add error data
                    dr.model = false;
                    dr.error = $"GetFile: {id}. {data}";
                }
            }
            catch (Exception ex)
            {
                // Add exception data
                dr.model = false;
                dr.error = $"GetFile: {id}. {ex.ToString()}";
            }

            // Return the response
            return dr;

        } // End of the GetFile method

        #endregion

        #region Delete methods

        /// <summary>
        /// Delete a file and the metadata for the file
        /// </summary>
        /// <param name="id">The identity of the file you want to delete.</param>
        /// <returns>A doxservr response with a boolean that indicates if the file was deleted.</returns>
        public async Task<DoxservrResponse<bool>> Delete(string id)
        {
            // Create the response to return
            DoxservrResponse<bool> dr = new DoxservrResponse<bool>();

            try
            {
                // Get the response
                HttpResponseMessage response = await this.client.DeleteAsync($"delete/{id}");

                // Check the status code for the response
                if (response.IsSuccessStatusCode == true)
                {
                    // Add success data
                    dr.model = true;
                }
                else
                {
                    // Get string data
                    string data = await response.Content.ReadAsStringAsync();

                    // Add error data
                    dr.model = false;
                    dr.error = $"Delete: {id}. {data}";
                }
            }
            catch (Exception ex)
            {
                // Add exception data
                dr.model = false;
                dr.error = $"Delete: {id}. {ex.ToString()}";
            }

            // Return the response
            return dr;

        } // End of the Delete method

        #endregion

    } // End of the class

} // End of the namespace