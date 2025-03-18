

using Microsoft.Extensions.DependencyInjection;

public static class ServicesExtensions
{
    public static IServiceCollection AddAzureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register Azure Blob Service Client via the Azure Clients builder.
        services.AddSingleton<BlobServiceClient>(provider =>
        {
            var credential = new DefaultAzureCredential();
            var accountName = configuration["Storage:AccountName"];
            ArgumentNullException.ThrowIfNullOrEmpty(accountName);
            var blobServiceClient = new BlobServiceClient(
                new Uri($"https://{accountName}.blob.core.windows.net"),
                credential);
            return blobServiceClient;
        });

        // Register BlobStorageProxy as IBlobStorage.
        services.AddSingleton<IBlobStorage>(provider =>
        {
            var blobServiceClient = provider.GetRequiredService<BlobServiceClient>();
            return new BlobStorageProxy(blobServiceClient, configuration);
        });

        // Register DocumentIntelligenceClient.
        services.AddSingleton<DocumentIntelligenceClient>(provider =>
        {
            var endpoint = configuration["DocumentIntelligence:Endpoint"];
            var apiKey = configuration["DocumentIntelligence:ApiKey"];
            return new DocumentIntelligenceClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
        });

        // Register DocumentIntelligenceProxy as IDocumentScanner.
        services.AddSingleton<IDocumentScanner, DocumentIntelligenceProxy>();

        // Register OpenAIClient.
        services.AddSingleton<OpenAIClient>(provider =>
        {
            var endpoint = configuration["AzureOpenAPI:Endpoint"];
            var apiKey = configuration["AzureOpenAPI:ApiKey"];
            return new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
        });

        services.AddSingleton<IOpenAI, OpenAIProxy>();

        return services;
    }
}

