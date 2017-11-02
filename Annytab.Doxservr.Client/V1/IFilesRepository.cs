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
        Task<FileMetadata> Send(HttpClient client, Stream stream, string receivers, string filename, string encoding = "", string standard = "", string language = "", string status = "");
        Task<bool> CreateInvoice(HttpClient client, Int64 gib);
        Task<Signature> Sign(HttpClient client, string id, string method, string date, string algorithm, string padding, string value = "", string certificate = "");
        Task<bool> MarkAsClosed(HttpClient client, string id);
        Task<FilesMetadata> GetList(HttpClient client, string ct = "", Int32 party_status = -1, Int32 file_status = -1, Int32 party_closed = -1, Int32 page_size = 10);
        Task GetFile(HttpClient client, string id, Stream stream);
        Task<bool> Delete(HttpClient client, string id);
        HttpClient GetClient();

    } // End of the interface

} // End of the namespace