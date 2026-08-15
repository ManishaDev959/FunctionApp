using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using OrderProcessing.Functions.Services;

namespace OrderProcessing.Functions.Functions;

public class DeleteBlob
{
    private readonly IBlobStorageService _blobStorageService;

    public DeleteBlob(IBlobStorageService blobStorageService)
    {
        _blobStorageService = blobStorageService;
    }

    [Function("DeleteBlob")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "delete",
            Route = "DeleteBlob/{blobName}")]
        HttpRequestData req,
        string blobName)
    {
        await _blobStorageService.DeleteAsync(blobName);

        var response =
            req.CreateResponse(HttpStatusCode.OK);

        await response.WriteStringAsync(
            $"Blob '{blobName}' deleted successfully.");

        return response;
    }
}