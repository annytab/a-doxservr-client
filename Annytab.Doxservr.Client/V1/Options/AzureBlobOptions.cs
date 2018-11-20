using System;

namespace Annytab.Doxservr.Client.V1
{
    /// <summary>
    /// This class represent azure blob options
    /// </summary>
    public class AzureBlobOptions
    {
        #region Variables

        public Int32 DegreeOfParallelism { get; set; }
        public Int32 MaxRetries  { get; set; }
        public Int32 TimeoutInSeconds { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new post
        /// </summary>
        public AzureBlobOptions()
        {
            // Set values for instance variables
            this.DegreeOfParallelism = 1;
            this.MaxRetries = 3;
            this.TimeoutInSeconds = 100;

        } // End of the constructor

        #endregion

    } // End of the class

} // End of the namespace