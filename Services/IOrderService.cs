using OrderProcessing.Functions.DTOs;

namespace OrderProcessing.Functions.Services;

public interface IOrderService
{
    Task<bool> TestDatabaseConnectionAsync();
    Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request);

    Task<OrderResponse?> GetOrderAsync(int id);
}