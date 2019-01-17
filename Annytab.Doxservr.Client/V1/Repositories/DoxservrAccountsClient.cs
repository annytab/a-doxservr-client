using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Annytab.Doxservr.Client.V1
{
    /// <summary>
    /// This class represents a doxservr accounts client
    /// </summary>
    public class DoxservrAccountsClient : IDoxservrAccountsClient
    {
        #region Variables

        private readonly HttpClient client;
        private readonly DoxservrOptions options;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new repository
        /// </summary>
        public DoxservrAccountsClient(HttpClient http_client, IOptions<DoxservrOptions> options)
        {
            // Set values for instance variables
            this.client = http_client;
            this.options = options.Value;

            // Set properties for the client
            this.client.BaseAddress = new Uri(this.options.ApiHost.TrimEnd('/') + "/api/v1/accounts/");
            this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(string.Format("{0}:{1}", this.options.ApiEmail, this.options.ApiPassword))));
            this.client.Timeout = TimeSpan.FromSeconds(this.options.TimeoutInSeconds);

        } // End of the constructor

        #endregion

        #region Add methods

        /// <summary>
        /// Submit an account advise
        /// </summary>
        public async Task<DoxservrResponse<Account>> Teach(Account item)
        {
            // Create the response to return
            DoxservrResponse<Account> dr = new DoxservrResponse<Account>();

            // Convert the item to json
            string json = JsonConvert.SerializeObject(item, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            // Send data as application/json data
            using (StringContent content = new StringContent(json, Encoding.UTF8, "application/json"))
            {
                try
                {
                    // Get the response
                    HttpResponseMessage response = await this.client.PostAsync($"teach", content);

                    // Check the status code for the response
                    if (response.IsSuccessStatusCode == true)
                    {
                        // Get string data
                        string data = await response.Content.ReadAsStringAsync();

                        // Deserialize the content
                        dr.model = JsonConvert.DeserializeObject<Account>(data);
                    }
                    else
                    {
                        // Get string data
                        string data = await response.Content.ReadAsStringAsync();

                        // Add error data
                        dr.error = $"Teach. {data}";
                    }
                }
                catch (Exception ex)
                {
                    // Add exception data
                    dr.error = $"Teach. {ex.ToString()}";
                }
            }

            // Return the response
            return dr;

        } // End of the Teach method

        /// <summary>
        /// Submit a list with account advises
        /// </summary>
        public async Task<DoxservrResponse<IList<string>>> TeachWithList(IList<Account> items)
        {
            // Create the response to return
            DoxservrResponse<IList<string>> dr = new DoxservrResponse<IList<string>>();

            // Convert items to json
            string json = JsonConvert.SerializeObject(items, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            // Send data as application/json data
            using (StringContent content = new StringContent(json, Encoding.UTF8, "application/json"))
            {
                try
                {
                    // Get the response
                    HttpResponseMessage response = await this.client.PostAsync($"teach_list", content);

                    // Check the status code for the response
                    if (response.IsSuccessStatusCode == true)
                    {
                        // Get string data
                        string data = await response.Content.ReadAsStringAsync();

                        // Deserialize the content
                        dr.model = JsonConvert.DeserializeObject<IList<string>>(data);
                    }
                    else
                    {
                        // Get string data
                        string data = await response.Content.ReadAsStringAsync();
                       
                        // Add error data
                        dr.error = $"TeachWithList. {data}";
                    }
                }
                catch (Exception ex)
                {
                    // Add exception data
                    dr.error = $"TeachWithList. {ex.ToString()}";
                }
            }

            // Return the response
            return dr;

        } // End of the TeachWithList method

        #endregion

        #region Get methods

        /// <summary>
        /// Get an account advise
        /// </summary>
        public async Task<DoxservrResponse<Account>> Get(Account item)
        {
            // Create the response to return
            DoxservrResponse<Account> dr = new DoxservrResponse<Account>();

            // Convert the item to json
            string json = JsonConvert.SerializeObject(item, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            // Send data as application/json data
            using (StringContent content = new StringContent(json, Encoding.UTF8, "application/json"))
            {
                try
                {
                    // Get the response
                    HttpResponseMessage response = await this.client.PostAsync($"get", content);

                    // Check the status code for the response
                    if (response.IsSuccessStatusCode == true)
                    {
                        // Get string data
                        string data = await response.Content.ReadAsStringAsync();

                        // Deserialize the content
                        dr.model = JsonConvert.DeserializeObject<Account>(data);
                    }
                    else
                    {
                        // Get string data
                        string data = await response.Content.ReadAsStringAsync();

                        // Add error data
                        dr.error = $"Get. {data}";
                    }
                }
                catch(Exception ex)
                {
                    // Add exception data
                    dr.error = $"Get. {ex.ToString()}";
                }
            }

            // Return the response
            return dr;

        } // End of the Get method

        /// <summary>
        /// Get a list with account advises
        /// </summary>
        public async Task<DoxservrResponse<IList<Account>>> GetList(IList<Account> items)
        {
            // Create the response to return
            DoxservrResponse<IList<Account>> dr = new DoxservrResponse<IList<Account>>();

            // Convert items to json
            string json = JsonConvert.SerializeObject(items, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            // Send data as application/json data
            using (StringContent content = new StringContent(json, Encoding.UTF8, "application/json"))
            {
                try
                {
                    // Get the response
                    HttpResponseMessage response = await this.client.PostAsync($"get_list", content);

                    // Check the status code for the response
                    if (response.IsSuccessStatusCode == true)
                    {
                        // Get string data
                        string data = await response.Content.ReadAsStringAsync();

                        // Deserialize the content 
                        dr.model = JsonConvert.DeserializeObject<IList<Account>>(data);
                    }
                    else
                    {
                        // Get string data
                        string data = await response.Content.ReadAsStringAsync();

                        // Add error data
                        dr.error = $"GetList. {data}";
                    }
                }
                catch(Exception ex)
                {
                    // Add exception data
                    dr.error = $"GetList. {ex.ToString()}";
                }     
            }

            // Return the response
            return dr;

        } // End of the GetList method

        #endregion

    } // End of the class

} // End of the namespace