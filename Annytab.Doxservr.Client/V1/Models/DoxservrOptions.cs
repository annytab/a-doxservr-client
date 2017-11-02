namespace Annytab.Doxservr.Client.V1
{
    /// <summary>
    /// This class represent doxservr options
    /// </summary>
    public class DoxservrOptions
    {
        #region Variables

        public string ApiHost { get; set; }
        public string ApiEmail { get; set; }
        public string ApiPassword { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new post
        /// </summary>
        public DoxservrOptions()
        {
            // Set values for instance variables
            this.ApiHost = "";
            this.ApiEmail = "";
            this.ApiPassword = "";

        } // End of the constructor

        #endregion

    } // End of the class

} // End of the namespace