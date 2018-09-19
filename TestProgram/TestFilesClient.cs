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

namespace TestProgram
{
    [TestClass]
    public class TestFilesClient
    {
        #region Variables

        private IConfigurationRoot configuration { get; set; }
        private IDoxservrFilesClient dox_client { get; set; }
        private ILogger logger { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new test instance
        /// </summary>
        public TestFilesClient()
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
            services.AddHttpClient<IDoxservrFilesClient, DoxservrFilesClient>();

            // Build a service provider
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            // Configure logging
            ILoggerFactory loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            loggerFactory.AddConsole(configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddFile("Logs/test-{Date}.txt");

            // Get references
            this.logger = loggerFactory.CreateLogger<IDoxservrFilesClient>();
            this.dox_client = serviceProvider.GetService<IDoxservrFilesClient>();

        } // End of the constructor

        #endregion

        /// <summary>
        /// Test to send a file
        /// </summary>
        [TestMethod]
        public async Task TestSend()
        {
            // Use a file stream
            using (FileStream stream = File.OpenRead("D:\\Bilder\\1960.jpg"))
            {
                // Send a file
                DoxservrResponse<FileDocument> dr = await this.dox_client.Send(stream, "fredde@jfsbokforing.se,info@bokforingstips.se", "1960.jpg");

                // Log the error
                if (dr.model == null)
                {
                    this.logger.LogError(dr.error);
                }

                // Test evaluation
                Assert.AreNotEqual(null, dr.model);
            }

        } // End of the TestSend method

        [TestMethod]
        public async Task TestSignWithDoxCertificate()
        {
            // Sign a file
            DoxservrResponse<Signature> dr = await this.dox_client.Sign("c44e1589-63e1-41eb-bcbf-44ee5bff4aff", "1", "2018-09-19", "SHA-1", "Pkcs1");

            // Log the error
            if (dr.model == null)
            {
                this.logger.LogError(dr.error);
            }

            // Test evaluation
            Assert.AreNotEqual(null, dr.model);

        } // End of the TestSignWithDoxCertificate method

        [TestMethod]
        public async Task TestSignWithOwnCertificate()
        {
            // Create a signature post
            Signature post = new Signature
            {
                algorithm = "SHA-256",
                padding = "Pkcs1",
                data = "dox@annytab.se,2018-09-19,XOLMHnuGXoGVl/hI1+5kLA==",
                value = "",
                certificate = ""
            };

            // Load a certificate and sign data
            X509Store certStore = null;
            X509Certificate2 certificate = null;

            try
            {
                // Open certificate from certificate store
                certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                certStore.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadOnly);
                X509Certificate2Collection certCollection = certStore.Certificates.Find(X509FindType.FindBySubjectName, "quiz-walk.com", false);

                // Get the certificate
                certificate = certCollection[0];

                // Sign the document
                using (RSA rsa = certificate.GetRSAPrivateKey())
                {
                    byte[] digest = rsa.SignData(Encoding.UTF8.GetBytes(post.data), Tools.GetHashAlgorithmName(post.algorithm), Tools.GetRSASignaturePadding(post.padding));
                    post.value = Convert.ToBase64String(digest);
                }

                // Set the certificate
                post.certificate = Convert.ToBase64String(certificate.RawData);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (certificate != null)
                {
                    certificate.Dispose();
                }
                if (certStore != null)
                {
                    certStore.Dispose();
                }
            }

            // Sign a file
            DoxservrResponse<Signature> dr = await this.dox_client.Sign("c44e1589-63e1-41eb-bcbf-44ee5bff4aff", "0", "2018-09-19", post.algorithm, post.padding, post.value, post.certificate);

            // Log the error
            if (dr.model == null)
            {
                this.logger.LogError(dr.error);
            }

            // Test evaluation
            Assert.AreNotEqual(null, dr.model);

        } // End of the TestSignWithOwnCertificate method

        [TestMethod]
        public async Task TestMarkAsClosed()
        {
            // Mark a file as closed
            DoxservrResponse<bool> dr = await this.dox_client.MarkAsClosed("c44e1589-63e1-41eb-bcbf-44ee5bff4aff");

            // Log the error
            if (dr.model == false)
            {
                this.logger.LogError(dr.error);
            }

            // Test evaluation
            Assert.AreNotEqual(false, dr.model);

        } // End of the TestMarkAsClosed method

        [TestMethod]
        public async Task TestResetStatus()
        {
            // Mark a file as closed
            DoxservrResponse<bool> dr = await this.dox_client.ResetStatus("c44e1589-63e1-41eb-bcbf-44ee5bff4aff");

            // Log the error
            if (dr.model == false)
            {
                this.logger.LogError(dr.error);
            }

            // Test evaluation
            Assert.AreNotEqual(false, dr.model);

        } // End of the TestMarkAsClosed method

        [TestMethod]
        public async Task TestGetPage()
        {
            // Get a page
            DoxservrResponse<FileDocuments> dr = await this.dox_client.GetPage("", -1, -1, -1, 2);

            // Log the error
            if (dr.model == null)
            {
                this.logger.LogError(dr.error);
            }

            // Test evaluation
            Assert.AreNotEqual(null, dr.model);

        } // End of the TestGetPage method

        [TestMethod]
        public async Task TestGetFile()
        {
            // Use a file stream
            using (FileStream fileStream = File.OpenWrite("D:\\Bilder\\dox-car-01.jpg"))
            {
                // Get the file
                DoxservrResponse<bool> dr = await this.dox_client.GetFile("c44e1589-63e1-41eb-bcbf-44ee5bff4aff", fileStream);

                // Log the error
                if (dr.model == false)
                {
                    this.logger.LogError(dr.error);
                }

                // Test evaluation
                Assert.AreNotEqual(false, dr.model);
            }

        } // End of the TestGetFile method

        [TestMethod]
        public async Task TestDelete()
        {
            // Delete a file
            DoxservrResponse<bool> dr = await this.dox_client.Delete("c44e1589-63e1-41eb-bcbf-44ee5bff4aff");

            // Log the error
            if (dr.model == false)
            {
                this.logger.LogError(dr.error);
            }

            // Test evaluation
            Assert.AreNotEqual(false, dr.model);

        } // End of the TestDelete method

    } // End of the class

} // End of the namespace