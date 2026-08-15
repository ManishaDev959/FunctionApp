using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using OrderProcessing.Functions.Services;

namespace OrderProcessing.Functions.Functions;

public class ListBlobs
{
    private readonly IBlobStorageService _blobStorageService;

    public ListBlobs(IBlobStorageService blobStorageService)
    {
        _blobStorageService = blobStorageService;
    }

    [Function("ListBlobs")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get")]
        HttpRequestData req)
    {
        var blobs =
            await _blobStorageService.ListBlobsAsync();

        var response =
            req.CreateResponse(HttpStatusCode.OK);

        await response.WriteStringAsync(
            JsonSerializer.Serialize(blobs));

        return response;
    }
}