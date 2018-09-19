namespace Annytab.Doxservr.Client.V1
{
    /// <summary>
    /// This class represent an account
    /// </summary>
    public class Account
    {
        #region Variables

        public string phrase { get; set; }
        public string country_code { get; set; }
        public string number { get; set; }
        public string description { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new post
        /// </summary>
        public Account()
        {
            // Set values for instance variables
            this.phrase = "";
            this.country_code = "";
            this.number = "";
            this.description = "";

        } // End of the constructor

        #endregion

    } // End of the class

} // End of the namespace