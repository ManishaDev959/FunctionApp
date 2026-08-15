using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using OrderProcessing.Functions.Services;

namespace OrderProcessing.Functions.Functions;

public class DownloadBlob
{
    private readonly IBlobStorageService _blobStorageService;

    public DownloadBlob(IBlobStorageService blobStorageService)
    {
        _blobStorageService = blobStorageService;
    }

    [Function("DownloadBlob")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get",
            Route = "DownloadBlob/{blobName}")]
        HttpRequestData req,
        string blobName)
    {
        var stream =
            await _blobStorageService.DownloadAsync(blobName);

        if (stream == null)
        {
            var notFound =
                req.CreateResponse(HttpStatusCode.NotFound);

            await notFound.WriteStringAsync(
                $"Blob '{blobName}' was not found.");

            return notFound;
        }

        var response =
            req.CreateResponse(HttpStatusCode.OK);

        response.Headers.Add(
            "Content-Type",
            "text/plain");

        await stream.CopyToAsync(
            response.Body);

        return response;
    }
}