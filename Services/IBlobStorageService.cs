namespace OrderProcessing.Functions.Services;

public interface IBlobStorageService
{
    Task UploadAsync(
        string blobName,
        Stream content,
        string contentType);

    Task<IEnumerable<string>> ListBlobsAsync();
    Task<Stream?> DownloadAsync(string blobName);

    Task DeleteAsync(string blobName);
}