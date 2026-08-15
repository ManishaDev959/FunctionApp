using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace OrderProcessing.Functions.Services;

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobContainerClient _containerClient;

    public BlobStorageService(
        BlobServiceClient blobServiceClient)
    {
        _containerClient =
            blobServiceClient.GetBlobContainerClient("orders");

        _containerClient.CreateIfNotExists();
    }

    public async Task<IEnumerable<string>> ListBlobsAsync()
    {
        var blobNames = new List<string>();

        await foreach (var blobItem in _containerClient.GetBlobsAsync())
        {
            blobNames.Add(blobItem.Name);
        }

        return blobNames;
    }

    public async Task UploadAsync(
        string blobName,
        Stream content,
        string contentType)
    {
        var blobClient =
            _containerClient.GetBlobClient(blobName);

        await blobClient.UploadAsync(
            content,
            new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = contentType
                }
            });
    }

    public async Task<Stream?> DownloadAsync(
        string blobName)
    {
        var blobClient =
            _containerClient.GetBlobClient(blobName);

        if (!await blobClient.ExistsAsync())
        {
            return null;
        }

        var response =
            await blobClient.DownloadStreamingAsync();

        return response.Value.Content;
    }

    public async Task DeleteAsync(
        string blobName)
    {
        var blobClient =
            _containerClient.GetBlobClient(blobName);

        await blobClient.DeleteIfExistsAsync();
    }
}