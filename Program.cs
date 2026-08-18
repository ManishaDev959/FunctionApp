using Azure.Identity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderProcessing.Functions.Data;
using OrderProcessing.Functions.Services;
using Azure.Storage.Blobs;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

var sqlServer = builder.Configuration["SqlServer"];
var sqlDatabase = builder.Configuration["SqlDatabase"];
var sqlUsername = builder.Configuration["SqlUsername"];
var sqlPassword = builder.Configuration["SqlPassword"];

//var connectionString =
//    $"Server={sqlServer};" +
//    $"Database={sqlDatabase};" +
//    $"User Id={sqlUsername};" +
//    $"Password={sqlPassword};" +
//    "Encrypt=True;" +
//    "TrustServerCertificate=False;";

var connectionString =
          Environment.GetEnvironmentVariable("AzureSqlConnection");


builder.Services.AddDbContext<OrderDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});


//var blobStorageConnection =
//    builder.Configuration["BlobStorageConnection"];

//builder.Services.AddSingleton(
//    new BlobServiceClient(blobStorageConnection));

builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();

builder.Build().Run();