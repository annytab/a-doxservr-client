using Newtonsoft.Json;

namespace Annytab.Doxservr.Client.V1
{
    /// <summary>
    /// This class represent a response with a model and other properties
    /// </summary>
    public class DoxservrResponse<T>
    {
        #region Variables

        public T model { get; set; }
        public string error { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new post
        /// </summary>
        public DoxservrResponse()
        {
            // Set values for instance variables
            this.model = default(T);
            this.error = null;

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