using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Annytab.Doxservr.Client.V1
{
    /// <summary>
    /// This class represent a block list data
    /// </summary>
    public class BlockListData
    {
        #region Variables

        public string id { get; set; }
        public string file_md5 { get; set; }
        public IList<string> block_list { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new post with default properties
        /// </summary>
        public BlockListData()
        {
            // Set values for instance variables
            this.id = "";
            this.file_md5 = "";
            this.block_list = new List<string>();

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