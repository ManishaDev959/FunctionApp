using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using OrderProcessing.Functions.Services;

namespace OrderProcessing.Functions.Functions;

public class UploadBlob
{
    private readonly IBlobStorageService _blobStorageService;

    public UploadBlob(IBlobStorageService blobStorageService)
    {
        _blobStorageService = blobStorageService;
    }

    [Function("UploadBlob")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")]
        HttpRequestData req)
    {
        var response =
            req.CreateResponse();

        if (!req.Headers.TryGetValues(
                "x-file-name",
                out var fileNames))
        {
            response.StatusCode =
                System.Net.HttpStatusCode.BadRequest;

            await response.WriteStringAsync(
                "Please provide x-file-name header.");

            return response;
        }

        var fileName = fileNames.First();

        await _blobStorageService.UploadAsync(
            fileName,
            req.Body,
            "application/octet-stream");

        response.StatusCode =
            System.Net.HttpStatusCode.OK;

        await response.WriteStringAsync(
            $"Blob '{fileName}' uploaded successfully.");

        return response;
    }
}