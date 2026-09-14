using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CLOUD.POE._1.Assignments
{
    public class UploadAssignmentFile
    {
        private readonly ILogger<UploadAssignmentFile> _logger;

        public UploadAssignmentFile(ILogger<UploadAssignmentFile> logger)
        {
            _logger = logger;
        }

        [Function("Uploadassignmentfile")]
        public async Task<IActionResult> Run(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "post",
                Route = "Uploadassignmentfile")] HttpRequest req)
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

            if (req.ContentLength == 0)
            {
                return new BadRequestObjectResult(
                    "Please provide the assignment file in the request body.");
            }

            string connectionString =
                Environment.GetEnvironmentVariable("AzureWebJobsStorage")
                ?? "UseDevelopmentStorage=true";

            BlobServiceClient blobServiceClient =
                new BlobServiceClient(connectionString);

            BlobContainerClient containerClient =
                blobServiceClient.GetBlobContainerClient("assignments");

            await containerClient.CreateIfNotExistsAsync(
                PublicAccessType.None);

            string blobName =
                $"{studentNumber}/{fileName}";

            BlobClient blobClient =
                containerClient.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync();

            BlobUploadOptions options = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = string.IsNullOrWhiteSpace(req.ContentType)
                        ? "application/octet-stream"
                        : req.ContentType
                }
            };

            await blobClient.UploadAsync(
                req.Body,
                options);

            _logger.LogInformation(
                "Assignment uploaded: {BlobName}",
                blobName);

            return new OkObjectResult(new
            {
                message = "Assignment file uploaded successfully.",
                studentNumber,
                fileName,
                blobName
            });
        }
    }
}