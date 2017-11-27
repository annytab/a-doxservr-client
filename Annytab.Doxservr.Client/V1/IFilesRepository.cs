using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Annytab.Doxservr.Client.V1
{
    /// <summary>
    /// This interface represent a doxservr repository
    /// </summary>
    public interface IFilesRepository
    {
        /// <summary>
        /// Send a file to one or multiple receivers
        /// </summary>
        /// <param name="client">A reference to a http client.</param>
        /// <param name="stream">A stream of the file to upload.</param>
        /// <param name="receivers">One or more email addresses to recipients. Each e-mail address should be delimited by comma (,).</param>
        /// <param name="filename">A filename, the extension is used to set the mime-type of the file.</param>
        /// <param name="encoding">You can specify how the text in the file has been encoded so that the recipient can convert the file to a string (ASCII, UTF-8, UTF-16 or UTF-32).</param>
        /// <param name="standard">You can specify the name of the standard applied to create the file. This information is important if the recipient should be able to process the file.</param>
        /// <param name="language">Enter a 2-letter language code according to ISO 639-1 that specifies the language used in the file. If no value is specified, the language code is set to en (English).</param>
        /// <param name="status">Enter 0 if you want to require the file to be signed by all parties or 1 if the file not needs to be signed. If no value is specified, the status is set to 1.</param>
        /// <returns>A reference to a file metadata post</returns>
        Task<FileMetadata> Send(HttpClient client, Stream stream, string receivers, string filename, string encoding = "", string standard = "", string language = "", string status = "");

        /// <summary>
        /// Create an invoice to refill your account with gibibytes
        /// </summary>
        /// <param name="client">A reference to a client</param>
        /// <param name="gib">The number of gibibytes to refill your account with.</param>
        /// <param name="iblt">The amount of bytes that your account balance must be less than in order for an invoice to be created.</param>
        /// <returns>A boolean that indicates if the response was successful</returns>
        Task<bool> CreateInvoice(HttpClient client, Int64 gib, Int64? iblt = null);

        /// <summary>
        /// Sign a file
        /// </summary>
        /// <param name="client">A reference to an http client</param>
        /// <param name="id">The identity of the file you want to sign.</param>
        /// <param name="method">Enter a code for the signature method to be applied (0: Your signature with your own certificate, 1: Signature with our certificate). Must be stated.</param>
        /// <param name="date">The date when the signature was created, formatted as yyyy-MM-dd. Must be specified and included in the signature's underlying data.</param>
        /// <param name="algorithm">The hashalgorithm applied to the signature (SHA-1, SHA-256, SHA-384 or SHA-512). Must be stated.</param>
        /// <param name="padding">The type of padding applied to the signature (Pkcs1 or Pss). Must be stated.</param>
        /// <param name="value">The hash of the signature encoded as a Base64 string, no BEGIN or END headers should be specified. Must be specified if signature method 0 is applied.</param>
        /// <param name="certificate">Your public certificate encoded as a Base64 string, no BEGIN or END headers should be specified. Must be specified if signature method 0 is applied.</param>
        /// <returns>The created signature as a signature modell</returns>
        Task<Signature> Sign(HttpClient client, string id, string method, string date, string algorithm, string padding, string value = "", string certificate = "");

        /// <summary>
        /// Mark a file as closed
        /// </summary>
        /// <param name="client">A reference to an http client.</param>
        /// <param name="id">The identity of the file you want to mark as closed.</param>
        /// <returns>A boolean that indicates if the file was closed.</returns>
        Task<bool> MarkAsClosed(HttpClient client, string id);

        /// <summary>
        /// Get a list with file metadata posts
        /// </summary>
        /// <param name="client">A reference to a http client.</param>
        /// <param name="ct">Specify a continuation-string to retrieve the next page. You'll get back such a string if there are more pages.</param>
        /// <param name="party_status">You can filter the list by entering your status as a party (0: New, 1: Sent, 2: Downloaded, 3: Signed).</param>
        /// <param name="file_status">Filter on file status(0: Waiting for signatures, 1: Ready to be used).</param>
        /// <param name="party_closed">Specify whether to retrieve open or closed files (0: Open, 1: Closed).</param>
        /// <param name="page_size">Enter how many items you want in your request. If you do not specify a value, the page size is set to 10.</param>
        /// <returns>A files metadata post with a list of posts and a continuation token.</returns>
        Task<FilesMetadata> GetList(HttpClient client, string ct = "", Int32 party_status = -1, Int32 file_status = -1, Int32 party_closed = -1, Int32 page_size = 10);

        /// <summary>
        /// Get a file as a stream
        /// </summary>
        /// <param name="client">A reference to a http client.</param>
        /// <param name="id">The identity of the file you want to download.</param>
        /// <param name="stream">A reference to a stream, the response stream will be copied to this stream.</param>
        /// <returns>A boolean that indicates if the file was downloaded.</returns>
        Task<bool> GetFile(HttpClient client, string id, Stream stream);

        /// <summary>
        /// Delete a file and the metadata for the file
        /// </summary>
        /// <param name="client">A reference to a http client.</param>
        /// <param name="id">The identity of the file you want to delete.</param>
        /// <returns>A boolean that indicates if the file was deleted.</returns>
        Task<bool> Delete(HttpClient client, string id);

        /// <summary>
        /// Get a reference to a http client
        /// </summary>
        HttpClient GetClient();

    } // End of the interface

} // End of the namespace