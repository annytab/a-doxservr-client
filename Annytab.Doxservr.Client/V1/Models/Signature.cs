using Newtonsoft.Json;

namespace Annytab.Doxservr.Client.V1
{
    /// <summary>
    /// This class represent a signature
    /// </summary>
    public class Signature
    {
        #region Variables

        public string algorithm { get; set; }
        public string padding { get; set; }
        public string data { get; set; }
        public string value { get; set; }
        public string certificate { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new post with default properties
        /// </summary>
        public Signature()
        {
            // Set values for instance variables
            this.algorithm = "";
            this.padding = "";
            this.data = "";
            this.value = "";
            this.certificate = "";

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