using System;
using System.IO;
using System.Text;
using System.Net.Http;
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
    public class TestFiles : IDisposable
    {
        #region Variables

        private IConfigurationRoot configuration { get; set; }
        private IFilesRepository file_repository { get; set; }
        private HttpClient client { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new test instance
        /// </summary>
        public TestFiles()
        {
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
            services.AddScoped<IFilesRepository, FilesRepository>();

            // Build a service provider
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            // Configure logging
            ILoggerFactory loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            loggerFactory.AddConsole(configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddFile("Logs/test-{Date}.txt");

            // Get a reference to the file repository and a client
            this.file_repository = serviceProvider.GetService<IFilesRepository>();
            this.client = this.file_repository.GetClient();

        } // End of the constructor

        #endregion

        /// <summary>
        /// Test to send a file
        /// </summary>
        [TestMethod]
        public async Task TestSend()
        {
            // Create a file metadata post
            FileMetadata post = null;

            // Set the file path
            string path = "D:\\Bilder\\1960.jpg";

            using (FileStream stream = File.OpenRead(path))
            {
                post = await this.file_repository.Send(this.client, stream, "fredde@jfsbokforing.se,info@bokforingstips.se", "1960.jpg");
            }

            // Test evaluation
            Assert.AreNotEqual(null, post);

        } // End of the TestSend method

        [TestMethod]
        public async Task TestCreateInvoice()
        {
            // Create an invoice
            bool success = await this.file_repository.CreateInvoice(this.client, 1);

            // Test evaluation
            Assert.AreEqual(true, success);

        } // End of the TestCreateInvoice method

        [TestMethod]
        public async Task TestSignWithDoxCertificate()
        {
            // Create a reference to a signature
            Signature post = null;

            // Sign the file
            post = await this.file_repository.Sign(this.client, "9bccb95f-a6e9-49d4-adff-4a666666fd9d", "1", "2017-11-01", "SHA-1", "Pkcs1");

            // Test evaluation
            Assert.AreNotEqual(null, post);

        } // End of the TestSignWithDoxCertificate method

        [TestMethod]
        public async Task TestSignWithOwnCertificate()
        {
            // Create a signature post
            Signature post = new Signature
            {
                algorithm = "SHA-256",
                padding = "Pkcs1",
                data = "dox@annytab.se,2017-11-01,XOLMHnuGXoGVl/hI1+5kLA==",
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

            // Sign the file
            post = await this.file_repository.Sign(this.client, "9bccb95f-a6e9-49d4-adff-4a666666fd9d", "0", "2017-11-01", post.algorithm, post.padding, post.value, post.certificate);

            // Test evaluation
            Assert.AreNotEqual(null, post);

        } // End of the TestSignWithOwnCertificate method

        [TestMethod]
        public async Task TestMarkAsClosed()
        {
            // Create an invoice
            bool success = await this.file_repository.MarkAsClosed(this.client, "9bccb95f-a6e9-49d4-adff-4a666666fd9d");

            // Test evaluation
            Assert.AreEqual(true, success);

        } // End of the TestMarkAsClosed method

        [TestMethod]
        public async Task TestGetList()
        {
            // Get a list
            FilesMetadata tuple = await this.file_repository.GetList(this.client);

            // Test evaluation
            Assert.AreNotEqual(0, tuple.posts.Count);

        } // End of the TestGetList method

        [TestMethod]
        public async Task TestGetFile()
        {
            // Create a file stream
            using (FileStream fileStream = File.OpenWrite("D:\\Bilder\\000001.json"))
            {
                // Get the file
                bool success = await this.file_repository.GetFile(this.client, "703a9717-ea9f-4484-add5-0a97d6576dd6", fileStream);

                // Test evaluation
                Assert.AreEqual(true, success);
                Assert.AreNotEqual(0, fileStream.Length);
            }

        } // End of the TestGetFile method

        [TestMethod]
        public async Task TestDelete()
        {
            // Create an invoice
            bool success = await this.file_repository.Delete(this.client, "9bccb95f-a6e9-49d4-adff-4a666666fd9d");

            // Test evaluation
            Assert.AreEqual(true, success);

        } // End of the TestDelete method

        #region IDisposable Support

        // To detect redundant calls
        private bool disposedValue = false; 

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Dispose of the client
                    if(this.client != null)
                    {
                        this.client.Dispose();
                    }
                }

                disposedValue = true;
            }

        } // End of the Dispose method

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);

        } // End of the Dispose method

        #endregion

    } // End of the class

} // End of the namespace