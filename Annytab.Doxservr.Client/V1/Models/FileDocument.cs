using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Annytab.Doxservr.Client.V1
{
    /// <summary>
    /// This class includes a page with file metadata items
    /// </summary>
    public class FileDocuments
    {
        #region Variables

        public IList<FileDocument> items { get; set; }
        public string ct { get; set; }
        public bool error { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new post
        /// </summary>
        public FileDocuments()
        {
            // Set values for instance variables
            this.items = new List<FileDocument>();
            this.ct = "";
            this.error = false;

        } // End of the constructor

        #endregion

        #region Get methods

        /// <summary>
        /// Convert the object to a json string
        /// </summary>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);

        } // End of the ToString method

        #endregion

    } // End of the class

    /// <summary>
    /// This class represent a file document post
    /// </summary>
    public class FileDocument
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
        public FileDocument()
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
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);

        } // End of the ToString method

        #endregion

    } // End of the class

} // End of the namespace