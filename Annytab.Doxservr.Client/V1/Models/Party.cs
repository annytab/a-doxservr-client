using System;
using Newtonsoft.Json;

namespace Annytab.Doxservr.Client.V1
{
    /// <summary>
    /// This class represent a document party
    /// </summary>
    public class Party
    {
        #region Variables

        public string email { get; set; }
        public Int32 is_sender { get; set; }
        public Int32 status { get; set; }
        public Int32 closed { get; set; }
        public Signature signature { get; set; }
        
        #endregion

        #region Constructors

        /// <summary>
        /// Create a new post with default properties
        /// </summary>
        public Party()
        {
            // Set values for instance variables
            this.email = "";
            this.is_sender = 0; 
            this.status = 0;
            this.closed = 0;
            this.signature = null;

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