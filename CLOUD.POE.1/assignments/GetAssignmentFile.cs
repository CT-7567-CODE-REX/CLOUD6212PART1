using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CLOUD.POE._1.Assignments
{
    public class GetAssignmentFile
    {
        private readonly ILogger<GetAssignmentFile> _logger;

        public GetAssignmentFile(ILogger<GetAssignmentFile> logger)
        {
            _logger = logger;
        }

        [Function("Getassignmentfile")]
        public async Task<IActionResult> Run(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "get",
                Route = "Getassignmentfile")] HttpRequest req)
        {
            string? studentNumber =
                req.Query["studentNumber"].FirstOrDefault();

            string? fileName =
                req.Query["fileName"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(studentNumber) ||
                string.IsNullOrWhiteSpace(fileName))
            {
                return new BadRequestObjectResult(
                    "Please provide studentNumber and fileName.");
            }

            fileName = Path.GetFileName(fileName);

            string connectionString =
                Environment.GetEnvironmentVariable("AzureWebJobsStorage")
                ?? "UseDevelopmentStorage=true";

            BlobServiceClient blobServiceClient =
                new BlobServiceClient(connectionString);

            BlobContainerClient containerClient =
                blobServiceClient.GetBlobContainerClient("assignments");

            string blobName =
                $"{studentNumber}/{fileName}";

            BlobClient blobClient =
                containerClient.GetBlobClient(blobName);

            if (!await blobClient.ExistsAsync())
            {
                return new NotFoundObjectResult(
                    "Assignment file was not found.");
            }

            var download =
                await blobClient.DownloadStreamingAsync();

            string contentType =
                download.Value.Details.ContentType
                ?? "application/octet-stream";

            _logger.LogInformation(
                "Assignment downloaded: {BlobName}",
                blobName);

            return new FileStreamResult(
                download.Value.Content,
                contentType)
            {
                FileDownloadName = fileName
            };
        }
    }
}