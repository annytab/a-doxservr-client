using Newtonsoft.Json;

namespace Annytab.Doxservr.Client.V1
{
    /// <summary>
    /// This class represent put block data
    /// </summary>
    public class PutBlockData
    {
        #region Variables

        public string id { get; set; }
        public string url { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new post
        /// </summary>
        public PutBlockData()
        {
            // Set values for instance variables
            this.id = "";
            this.url = "";

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