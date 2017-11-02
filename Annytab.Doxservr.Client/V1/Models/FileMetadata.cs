using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Annytab.Doxservr.Client.V1
{
    /// <summary>
    /// This class includes a list with file metadata posts and a continuation token
    /// </summary>
    public class FilesMetadata
    {
        #region Variables

        public IList<FileMetadata> posts { get; set; }
        public string ct { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new post
        /// </summary>
        public FilesMetadata()
        {
            // Set values for instance variables
            this.posts = new List<FileMetadata>();
            this.ct = "";

        } // End of the constructor

        #endregion

    } // End of the class

    /// <summary>
    /// This class represent a file metadata post
    /// </summary>
    public class FileMetadata
    {
        #region Variables

        public string id { get; set; }
        public string model_type { get; set; }
        public DateTime date_of_sending { get; set; }
        public string file_encoding { get; set; }
        public string filename { get; set; }
        public Int64 file_length { get; set; }
        public string file_md5 { get; set; }
        public string standard_name { get; set; }
        public string language_code { get; set; }
        public Int32 status { get; set; }
        public IList<Party> parties { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new post with default properties
        /// </summary>
        public FileMetadata()
        {
            // Set values for instance variables
            this.id = Guid.NewGuid().ToString();
            this.model_type = "file_metadata";
            this.date_of_sending = new DateTime(2000,1,1);
            this.file_encoding = "";
            this.filename = "";
            this.file_length = -1;
            this.file_md5 = "";
            this.standard_name = "";
            this.language_code = "en";
            this.status = 0;
            this.parties = new List<Party>();

        } // End of the constructor

        #endregion

        #region Get methods

        /// <summary>
        /// Convert the object to a json string
        /// </summary>
        /// <returns>A json formatted string</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);

        } // End of the ToString method

        #endregion

    } // End of the class

} // End of the namespace