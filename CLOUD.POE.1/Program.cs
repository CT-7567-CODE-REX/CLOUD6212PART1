using Azure.Monitor.OpenTelemetry.Exporter;
using CLOUD.POE._1.Services;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// Create the Azure Functions application.
var builder = FunctionsApplication.CreateBuilder(args);

// Enable ASP.NET Core HTTP support.
builder.ConfigureFunctionsWebApplication();

// Register the CoffeeNChill menu storage service.
builder.Services.AddSingleton<MenuStorageService>();

// Register the Azure File Share service.
builder.Services.AddSingleton<StaffDocumentService>();

// Enable Application Insights/OpenTelemetry only when configured.
if (!string.IsNullOrEmpty(
    Environment.GetEnvironmentVariable(
        "APPLICATIONINSIGHTS_CONNECTION_STRING")))
{
    builder.Services
        .AddOpenTelemetry()
        .UseFunctionsWorkerDefaults()
        .UseAzureMonitorExporter();
}

// Build and start the Functions application once.
builder.Build().Run();