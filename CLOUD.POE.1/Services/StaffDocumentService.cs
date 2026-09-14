using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CLOUD.POE._1.Models;
using Microsoft.AspNetCore.Http;

namespace CLOUD.POE._1.Services
{
    public class StaffDocumentService
    {
        private readonly BlobContainerClient _containerClient;

        public StaffDocumentService()
        {
            string connectionString =
                Environment.GetEnvironmentVariable("StaffDocumentsConnection")
                ?? "UseDevelopmentStorage=true";

            _containerClient = new BlobContainerClient(
                connectionString,
                "staff-docs");
        }

        private async Task EnsureContainerExistsAsync()
        {
            await _containerClient.CreateIfNotExistsAsync(
                PublicAccessType.None);
        }

        public async Task UploadAsync(IFormFile file)
        {
            await EnsureContainerExistsAsync();

            string fileName = Path.GetFileName(file.FileName);

            BlobClient blobClient =
                _containerClient.GetBlobClient(fileName);

            // Replace an existing document with the same name.
            await blobClient.DeleteIfExistsAsync();

            using Stream stream = file.OpenReadStream();

            BlobUploadOptions options = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = string.IsNullOrWhiteSpace(file.ContentType)
                        ? "application/octet-stream"
                        : file.ContentType
                }
            };

            await blobClient.UploadAsync(stream, options);
        }

        public async Task<List<StaffDocumentInfo>> ListAsync()
        {
            await EnsureContainerExistsAsync();

            List<StaffDocumentInfo> documents = new();

            await foreach (BlobItem blob in _containerClient.GetBlobsAsync())
            {
                documents.Add(new StaffDocumentInfo
                {
                    FileName = blob.Name,
                    Size = blob.Properties.ContentLength ?? 0,
                    LastModified = blob.Properties.LastModified
                });
            }

            return documents;
        }

        public async Task<Stream?> DownloadAsync(string fileName)
        {
            await EnsureContainerExistsAsync();

            string safeFileName = Path.GetFileName(fileName);

            BlobClient blobClient =
                _containerClient.GetBlobClient(safeFileName);

            if (!await blobClient.ExistsAsync())
            {
                return null;
            }

            BlobDownloadStreamingResult download =
                await blobClient.DownloadStreamingAsync();

            return download.Content;
        }
    }
}