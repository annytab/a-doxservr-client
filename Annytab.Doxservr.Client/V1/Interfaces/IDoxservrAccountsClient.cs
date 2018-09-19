using System.Collections.Generic;
using System.Threading.Tasks;

namespace Annytab.Doxservr.Client.V1
{
    /// <summary>
    /// This interface represent a doxservr accounts client
    /// </summary>
    public interface IDoxservrAccountsClient
    {
        /// <summary>
        /// Submit an account advise
        /// </summary>
        Task<DoxservrResponse<Account>> Teach(Account item);

        /// <summary>
        /// Submit a list with account advises
        /// </summary>
        Task<DoxservrResponse<IList<string>>> TeachWithList(IList<Account> items);

        /// <summary>
        /// Get an account advise
        /// </summary>
        Task<DoxservrResponse<Account>> Get(Account item);

        /// <summary>
        /// Get a list with account advises
        /// </summary>
        Task<DoxservrResponse<IList<Account>>> GetList(IList<Account> items);

    } // End of the interface

} // End of the namespace