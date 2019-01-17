using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
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
        private IAzureBlobsClient azure_client { get; set; }
        private ILogger logger { get; set; }

        // Progress
        private decimal progress { get; set; }
        private Int64 file_size { get; set; }

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

            // Add logging and options as services
            services.AddLogging(logging => {
                logging.AddConfiguration(configuration.GetSection("Logging"));
                logging.AddConsole();
                logging.AddDebug();
            });
            services.AddOptions();

            // Create api options
            services.Configure<DoxservrOptions>(configuration.GetSection("DoxservrOptions"));
            services.Configure<AzureBlobOptions>(configuration.GetSection("AzureBlobOptions"));

            // Add repositories
            services.AddHttpClient<IDoxservrFilesClient, DoxservrFilesClient>().ConfigurePrimaryHttpMessageHandler(() =>
                new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });
            services.AddHttpClient<IAzureBlobsClient, AzureBlobsClient>().ConfigurePrimaryHttpMessageHandler(() =>
                new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });

            // Build a service provider
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            // Configure file logging
            ILoggerFactory loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            loggerFactory.AddFile("Logs/log-{Date}.txt");

            // Get references
            this.logger = loggerFactory.CreateLogger<IDoxservrFilesClient>();
            this.dox_client = serviceProvider.GetService<IDoxservrFilesClient>();
            this.azure_client = serviceProvider.GetService<IAzureBlobsClient>();

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

        /// <summary>
        /// Test to send a file
        /// </summary>
        [TestMethod]
        public async Task TestSendWithBlocks()
        {
            // Add progress and cancellation
            Progress<Int64> progress_indicator = new Progress<Int64>(ReportProgress);
            CancellationTokenSource cts = new CancellationTokenSource();

            // Set progress to 0
            this.progress = 0;

            // Use a file stream
            using (FileStream stream = File.OpenRead("D:\\ImportantFiles\\IslandOfBlocks20131122.zip"))
            {
                // Get the file size
                this.file_size = stream.Length;

                // Get an upload url
                DoxservrResponse<PutBlockData> dr_block = await this.dox_client.GetUploadUrl();

                // Log the error
                if (dr_block.model == null)
                {
                    this.logger.LogError(dr_block.error);
                    Assert.Fail();
                    return;
                }

                // Upload blocks
                DoxservrResponse<BlockListData> dr_blocks = await this.azure_client.UploadBlocks(stream, dr_block.model, 1 * 1024 * 1024, progress_indicator);

                //// Test to cancel request
                //Task<DoxservrResponse<BlockListData>> task = this.azure_client.UploadBlocks(stream, dr_block.model, 4 * 1024 * 1024, progress_indicator, cts.Token);
                //await Task.Delay(200);
                //cts.Cancel();
                //await task;
                //DoxservrResponse<BlockListData> dr_blocks = task.Result;

                this.logger.LogInformation("Finished uploading blocks!");

                // Log the error
                if (dr_blocks.model == null)
                {
                    this.logger.LogError(dr_blocks.error);
                    Assert.Fail();
                    return;
                }

                // Upload the block list
                DoxservrResponse<FileDocument> dr_file = await this.dox_client.UploadBlockList(dr_blocks.model, "fredde@jfsbokforing.se,info@bokforingstips.se", "IslandOfBlocks20131122.zip", 
                    "encoding", "standard", "sv", "0");

                // Log the error
                if (dr_file.model == null)
                {
                    this.logger.LogError(dr_file.error);
                    Assert.Fail();
                }
            }

        } // End of the TestSendWithBlocks method

        /// <summary>
        /// Report progress for block uploading
        /// </summary>
        public void ReportProgress(Int64 bytes_uploaded)
        {
            this.progress += bytes_uploaded;
            decimal percent = Math.Round((this.progress / this.file_size) * 100.00M, 2, MidpointRounding.AwayFromZero);
            this.logger.LogInformation(percent.ToString() + " %");

        } // End of the ReportProgress method

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
            using (FileStream fileStream = File.OpenWrite("D:\\Bilder\\001-islands.zip"))
            {
                // Get the file
                DoxservrResponse<bool> dr = await this.dox_client.GetFile("5e1a8deb-39a0-4ba3-bbd6-0bdfefb1d0bb", fileStream);

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