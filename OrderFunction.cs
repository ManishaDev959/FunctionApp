using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using OrderProcessing.Functions.DTOs;
using OrderProcessing.Functions.Services;

namespace OrderProcessing.Functions.Functions;

public class OrderFunction
{
    private readonly IOrderService _orderService;

    public OrderFunction(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [Function("CreateOrder")]
    public async Task<HttpResponseData> CreateOrder(
        [HttpTrigger(
            AuthorizationLevel.Function,
            "post",
            Route = "orders")]
        HttpRequestData req)
    {
        var request =
            await JsonSerializer.DeserializeAsync<CreateOrderRequest>(
                req.Body);

        if (request == null)
        {
            var badRequest = req.CreateResponse(
                HttpStatusCode.BadRequest);

            badRequest.WriteStringAsync("Invalid request.");

            return badRequest;
        }

        if (request.CustomerId <= 0 ||
            request.ProductId <= 0 ||
            request.Quantity <= 0)
        {
            var badRequest = req.CreateResponse(
                HttpStatusCode.BadRequest);

            badRequest.WriteStringAsync(
                "CustomerId, ProductId and Quantity must be greater than zero.");

            return badRequest;
        }

        var order =
            await _orderService.CreateOrderAsync(request);

        var response =
            req.CreateResponse(HttpStatusCode.Created);

        await response.WriteAsJsonAsync(order);

        return response;
    }

    [Function("GetOrder")]
    public async Task<HttpResponseData> GetOrder(
        [HttpTrigger(
            AuthorizationLevel.Function,
            "get",
            Route = "orders/{id:int}")]
        HttpRequestData req,
        int id)
    {
        var order =
            await _orderService.GetOrderAsync(id);

        if (order == null)
        {
            var notFound = req.CreateResponse(
                HttpStatusCode.NotFound);

            notFound.WriteStringAsync("Order not found.");

            return notFound;
        }

        var response =
            req.CreateResponse(HttpStatusCode.OK);

        await response.WriteAsJsonAsync(order);

        return response;
    }
}