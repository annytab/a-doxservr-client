# a-doxservr-client
This is a client library for the Doxservr (<a href="https://www.doxservr.com">www.doxservr.com</a>) Api. This library can be used to connect to the Doxservr Api and to call api methods in the Doxservr Api.

If you want to test the client with the test program, add a appsettings.Development.json (copy of appsettings.json) or modify the appsettings.json file.

This library is available as a NuGet package: <a href="https://www.nuget.org/packages/Annytab.Doxservr.Client/">a-doxservr-client (NuGet Gallery)</a>

You can add DoxservrAccountsClient and DoxservrFilesClient to IHttpClientFactory in ASP.NET Core 2.1 or later version. See the TestProgram for examples on how to use clients.

```
// Create api options
services.Configure<DoxservrOptions>(configuration.GetSection("DoxservrOptions"));
services.Configure<AzureBlobOptions>(configuration.GetSection("AzureBlobOptions"));

// Add client
services.AddHttpClient<IDoxservrAccountsClient, DoxservrAccountsClient>().ConfigurePrimaryHttpMessageHandler(() =>
                new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });
services.AddHttpClient<IDoxservrFilesClient, DoxservrFilesClient>().ConfigurePrimaryHttpMessageHandler(() =>
                new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });
services.AddHttpClient<IAzureBlobsClient, AzureBlobsClient>().ConfigurePrimaryHttpMessageHandler(() =>
                new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });
```

You can also create clients with the constructor by adding a HttpClient and IOptions.

```
// Create api options
IOptions<DoxservrOptions> dox_options = Options.Create<DoxservrOptions>(new DoxservrOptions
{
	ApiEmail = "demo@demo.se",
	ApiHost = "https://www.doxservr.com",
	ApiPassword = "my_password"
});
IOptions<AzureBlobOptions> azure_options = Options.Create<AzureBlobOptions>(new AzureBlobOptions
{
	DegreeOfParallelism = 4,
	MaxRetries = 3
});

// Create clients
IDoxservrAccountsClient accounts_client = new DoxservrAccountsClient(this.client_factory.CreateClient(), dox_options);
IDoxservrFilesClient files_client = new DoxservrFilesClient(this.client_factory.CreateClient(), dox_options);
IAzureBlobsClient azure_client = new AzureBlobsClient(this.client_factory.CreateClient(), azure_options);
```

Documentation (English): <a href="https://www.doxservr.com/home/api">Doxservr Api</a><br />
Documentation (Swedish): <a href="https://www.doxservr.se/home/api">Doxservr Api</a>
