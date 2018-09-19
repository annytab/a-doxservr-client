using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Annytab.Doxservr.Client.V1;
using System.Collections.Generic;

namespace TestProgram
{
    [TestClass]
    public class TestAccountsClient
    {
        #region Variables

        private IConfigurationRoot configuration { get; set; }
        private IDoxservrAccountsClient dox_client { get; set; }
        private ILogger logger { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new test instance
        /// </summary>
        public TestAccountsClient()
        {
            // Add configuration settings
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            builder.AddJsonFile($"appsettings.Development.json", optional: true);
            this.configuration = builder.Build();

            // Create a service collection
            IServiceCollection services = new ServiceCollection();

            // Add services for logging and for options
            services.AddLogging();
            services.AddOptions();

            // Create api options
            services.Configure<DoxservrOptions>(configuration.GetSection("DoxservrOptions"));

            // Add repositories
            services.AddHttpClient<IDoxservrAccountsClient, DoxservrAccountsClient>();

            // Build a service provider
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            // Configure logging
            ILoggerFactory loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            loggerFactory.AddConsole(configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddFile("Logs/test-{Date}.txt");

            // Get references
            this.logger = loggerFactory.CreateLogger<IDoxservrAccountsClient>();
            this.dox_client = serviceProvider.GetService<IDoxservrAccountsClient>();

        } // End of the constructor

        #endregion

        /// <summary>
        /// Test to teach the bot
        /// </summary>
        [TestMethod]
        public async Task TestTeach()
        {
            // Create a new account
            Account item = new Account
            {
                phrase = "telia-faktura",
                country_code = "SE",
                number = "6210",
                description = "Telekommunikation"
            };

            // Teach the bot
            DoxservrResponse<Account> dr = await this.dox_client.Teach(item);

            // Log the error
            if (dr.model == null)
            {
                this.logger.LogError(dr.error);
            }

            // Test evaluation
            Assert.AreNotEqual(null, dr.model);

        } // End of the TestTeach method

        [TestMethod]
        public async Task TestTeachWithList()
        {
            // Create a list
            IList<Account> items = new List<Account>();
            items.Add(new Account
            {
                phrase = "inköp av tjänst",
                country_code = "SE",
                number = "6500",
                description = "Övriga externa tjänster"
            });
            items.Add(new Account
            {
                phrase = "dator",
                country_code = "SE",
                number = "1250",
                description = "Datorer"
            });
            items.Add(new Account
            {
                phrase = "datorer",
                country_code = "SE",
                number = "1250",
                description = "Datorer"
            });

            // Teach the bot
            DoxservrResponse<IList<string>> dr = await this.dox_client.TeachWithList(items);

            // Log the error
            if (dr.model == null)
            {
                this.logger.LogError(dr.error);
            }
            else if (dr.model.Count > 0)
            {
                // Loop errors
                foreach(string str in dr.model)
                {
                    this.logger.LogError("TeachWithList: " + str);
                }
            }

            // Test evaluation
            Assert.AreNotEqual(null, dr.model);

        } // End of the TestTeachWithList method

        [TestMethod]
        public async Task TestGet()
        {
            // Create a new account
            Account item = new Account
            {
                phrase = "telia-faktura",
                country_code = "SE"
            };

            // Get an account advise
            DoxservrResponse<Account> dr = await this.dox_client.Get(item);

            // Log the error
            if (dr.model == null)
            {
                this.logger.LogError(dr.error);
            }

            // Test evaluation
            Assert.AreNotEqual(null, dr.model);

        } // End of the TestGet method

        [TestMethod]
        public async Task TestGetList()
        {
            // Create a list
            IList<Account> items = new List<Account>();
            items.Add(new Account
            {
                phrase = "inköp av tjänst",
                country_code = "SE"
            });
            items.Add(new Account
            {
                phrase = "dator",
                country_code = "SE"
            });
            items.Add(new Account
            {
                phrase = "datorer",
                country_code = "SE"
            });

            // Get an account advise
            DoxservrResponse<IList<Account>> dr = await this.dox_client.GetList(items);

            // Log the error
            if (dr.model == null)
            {
                this.logger.LogError(dr.error);
            }

            // Test evaluation
            Assert.AreNotEqual(null, dr.model);

        } // End of the TestGetList method

    } // End of the class

} // End of the namespace