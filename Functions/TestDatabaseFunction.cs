using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using OrderProcessing.Functions.Services;

namespace OrderProcessing.Functions.Functions;

public class TestDatabaseFunction
{
    private readonly IOrderService _orderService;
    private readonly ILogger<TestDatabaseFunction> _logger;

    public TestDatabaseFunction(
        IOrderService orderService,
        ILogger<TestDatabaseFunction> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    [Function("TestDatabase")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "test-db")]
        HttpRequestData req)
    {
        _logger.LogInformation("Testing Azure SQL database connection...");

        try
        {
            var canConnect =
                await _orderService.TestDatabaseConnectionAsync();

            var response = req.CreateResponse();

            if (canConnect)
            {
                response.StatusCode =
                    System.Net.HttpStatusCode.OK;

                await response.WriteStringAsync(
                    "Successfully connected to Azure SQL Database.");
            }
            else
            {
                response.StatusCode =
                    System.Net.HttpStatusCode.InternalServerError;

                await response.WriteStringAsync(
                    "Could not connect to Azure SQL Database.");
            }

            return response;
        }

        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error connecting to Azure SQL Database.");

            var response = req.CreateResponse(
                System.Net.HttpStatusCode.InternalServerError);

            await response.WriteStringAsync(
                $"Database connection failed:\n{ex}");

            return response;
        }
        //catch (Exception ex)
        //{
        //    _logger.LogError(
        //        ex,
        //        "Error connecting to Azure SQL Database.");

        //    var response = req.CreateResponse(
        //        System.Net.HttpStatusCode.InternalServerError);

        //    await response.WriteStringAsync(
        //        $"Database connection failed: {ex.Message}");

        //    return response;
        //}
    }
}