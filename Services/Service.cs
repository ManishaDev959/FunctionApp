using Microsoft.EntityFrameworkCore;
using OrderProcessing.Functions.Data;
using OrderProcessing.Functions.DTOs;
using OrderProcessing.Functions.Models;
using System.Text;
using System.Text.Json;

namespace OrderProcessing.Functions.Services;

public class OrderService : IOrderService
{
    private readonly OrderDbContext _context;
    private readonly IBlobStorageService _blobStorageService;

    public OrderService(OrderDbContext context, IBlobStorageService blobStorageService)
    {
        _context = context;
        _blobStorageService = blobStorageService;
    }


    //public async Task<bool> TestDatabaseConnectionAsync()
    //{
    //    try
    //    {
    //        return await _context.Database.CanConnectAsync();
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"DATABASE ERROR: {ex}");
    //        throw;
    //    }
    //}


    public async Task<bool> TestDatabaseConnectionAsync()
    {
        try
        {
            await _context.Database.OpenConnectionAsync();

            Console.WriteLine("DATABASE CONNECTION SUCCESSFUL");

            await _context.Database.CloseConnectionAsync();

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("DATABASE ERROR:");
            Console.WriteLine(ex.ToString());

            return false;
        }
    }

    public async Task<OrderResponse> CreateOrderAsync(
        CreateOrderRequest request)
    {
        var order = new Order
        {
            CustomerId = request.CustomerId,
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            TotalAmount = request.Quantity * 1000,
            Status = "Pending"
        };

        _context.Orders.Add(order);

        await _context.SaveChangesAsync();

        var orderResponse = new OrderResponse
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            ProductId = order.ProductId,
            Quantity = order.Quantity,
            TotalAmount = order.TotalAmount,
            Status = order.Status
        };

        var json = JsonSerializer.Serialize(orderResponse, new JsonSerializerOptions { WriteIndented = true });

        using var stream = new MemoryStream(
                Encoding.UTF8.GetBytes(json));

        // 6. Upload JSON to Blob Storage
        var blobName = $"order-{order.Id}.json";

        await _blobStorageService.UploadAsync(
       blobName,
       stream,
       "application/json");

        return orderResponse;


    }

    public async Task<OrderResponse?> GetOrderAsync(int id)
    {
        var order = await _context.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (order == null)
        {
            return null;
        }

        return new OrderResponse
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            ProductId = order.ProductId,
            Quantity = order.Quantity,
            TotalAmount = order.TotalAmount,
            Status = order.Status
        };
    }
}