using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Annytab.Doxservr.Client.V1
{
    /// <summary>
    /// This interface represent an azure blobs client
    /// </summary>
    public interface IAzureBlobsClient
    {
        /// <summary>
        /// Upload a file block by block
        /// </summary>
        /// <param name="stream">A stream of the file to upload.</param>
        /// <param name="data">A reference to a put block data object.</param>
        /// <param name="block_size">The size of each block in bytes, max 100 MiB.</param>
        /// <param name="progress">A reference to a provider for progress updates.</param>
        /// <param name="ct">A cancellation token to be able to cancel the upload.</param>
        /// <returns>A reference to block list data, used when uploading the block list.</returns>
        Task<DoxservrResponse<BlockListData>> UploadBlocks(Stream stream, PutBlockData data, Int32 block_size, IProgress<Int64> progress = null, CancellationToken ct = default(CancellationToken));

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
        Task<Int32> UploadBlock(string url, string block_id, byte[] block, SemaphoreSlim throttler, IProgress<Int64> progress, CancellationToken ct = default(CancellationToken));

        /// <summary>
        /// Delete a blob by using a SAS url
        /// </summary>
        /// <param name="data">A reference to a put block data object</param>
        /// <returns>A doxservr response with a boolean that indicates if the blob was deleted.</returns>
        Task<DoxservrResponse<bool>> DeleteBlob(PutBlockData data);

    } // End of the interface

} // End of the namespace