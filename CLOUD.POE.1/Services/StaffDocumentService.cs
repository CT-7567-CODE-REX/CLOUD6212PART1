using Azure;
using Azure.Storage.Files.Shares;
using CLOUD.POE._1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace CLOUD.POE._1.Services
{
    public class StaffDocumentService
    {
        private readonly ShareClient _shareClient;

        public StaffDocumentService(IConfiguration configuration)
        {
            // Read the real Azure Storage connection string.
            string connectionString =
                configuration["StaffDocumentsConnection"]
                ?? throw new InvalidOperationException(
                    "StaffDocumentsConnection is not configured."
                );

            // Connect to the staff-docs Azure File Share.
            _shareClient = new ShareClient(
                connectionString,
                "staff-docs"
            );
        }

        private async Task EnsureShareExistsAsync()
        {
            // Create the share if it does not already exist.
            await _shareClient.CreateIfNotExistsAsync();
        }

        public async Task UploadAsync(IFormFile file)
        {
            await EnsureShareExistsAsync();

            // Use the root folder of staff-docs.
            var directoryClient =
                _shareClient.GetRootDirectoryClient();

            // Keep only the file name.
            string safeFileName =
                Path.GetFileName(file.FileName);

            var fileClient =
                directoryClient.GetFileClient(safeFileName);

            // Replace a file if the same name already exists.
            await fileClient.DeleteIfExistsAsync();

            // Azure Files requires the file size before uploading data.
            await fileClient.CreateAsync(file.Length);

            await using Stream stream =
                file.OpenReadStream();

            // Upload the file from the beginning.
            await fileClient.UploadRangeAsync(
                new HttpRange(0, file.Length),
                stream
            );
        }

        public async Task<List<StaffDocumentInfo>> ListAsync()
        {
            await EnsureShareExistsAsync();

            var directoryClient =
                _shareClient.GetRootDirectoryClient();

            List<StaffDocumentInfo> documents = new();

            // Read all files stored in staff-docs.
            await foreach (
                var item in
                directoryClient.GetFilesAndDirectoriesAsync())
            {
                if (item.IsDirectory)
                {
                    continue;
                }

                var fileClient =
                    directoryClient.GetFileClient(item.Name);

                var properties =
                    await fileClient.GetPropertiesAsync();

                documents.Add(
                    new StaffDocumentInfo
                    {
                        FileName = item.Name,
                        Size = properties.Value.ContentLength,
                        LastModified =
                            properties.Value.LastModified
                    }
                );
            }

            return documents;
        }

        public async Task<Stream?> DownloadAsync(
            string fileName)
        {
            await EnsureShareExistsAsync();

            var directoryClient =
                _shareClient.GetRootDirectoryClient();

            string safeFileName =
                Path.GetFileName(fileName);

            var fileClient =
                directoryClient.GetFileClient(safeFileName);

            // Return null when the requested document does not exist.
            if (!await fileClient.ExistsAsync())
            {
                return null;
            }

            var download =
                await fileClient.DownloadAsync();

            return download.Value.Content;
        }
    }
}