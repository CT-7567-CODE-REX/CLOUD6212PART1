using CLOUD.POE._1.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace CLOUD.POE._1.Documents
{
    public class DocumentFunctions
    {
        private readonly StaffDocumentService _documentService;

        public DocumentFunctions(
            StaffDocumentService documentService)
        {
            _documentService = documentService;
        }

        [Function("UploadStaffDocument")]
        public async Task<IActionResult> UploadStaffDocument(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "post",
                Route = "documents/upload")]
            HttpRequest request)
        {
            // File uploads must use multipart/form-data.
            if (!request.HasFormContentType)
            {
                return new BadRequestObjectResult(
                    new
                    {
                        message =
                            "Request must use multipart/form-data."
                    }
                );
            }

            IFormCollection form =
                await request.ReadFormAsync();

            // Postman will send the uploaded file with the key "file".
            IFormFile? file =
                form.Files.GetFile("file");

            if (file == null || file.Length == 0)
            {
                return new BadRequestObjectResult(
                    new
                    {
                        message =
                            "A staff document must be selected."
                    }
                );
            }

            await _documentService.UploadAsync(file);

            return new OkObjectResult(
                new
                {
                    message =
                        "Staff document uploaded successfully.",
                    fileName = file.FileName,
                    size = file.Length
                }
            );
        }

        [Function("ListStaffDocuments")]
        public async Task<IActionResult> ListStaffDocuments(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "get",
                Route = "documents")]
            HttpRequest request)
        {
            var documents =
                await _documentService.ListAsync();

            return new OkObjectResult(documents);
        }

        [Function("DownloadStaffDocument")]
        public async Task<IActionResult> DownloadStaffDocument(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "get",
                Route = "documents/download/{fileName}")]
            HttpRequest request,
            string fileName)
        {
            Stream? stream =
                await _documentService.DownloadAsync(fileName);

            if (stream == null)
            {
                return new NotFoundObjectResult(
                    new
                    {
                        message =
                            "Staff document not found."
                    }
                );
            }

            string contentType =
                GetContentType(fileName);

            return new FileStreamResult(
                stream,
                contentType)
            {
                FileDownloadName = fileName
            };
        }

        private static string GetContentType(
            string fileName)
        {
            string extension =
                Path.GetExtension(fileName)
                    .ToLowerInvariant();

            return extension switch
            {
                ".pdf" => "application/pdf",
                ".txt" => "text/plain",

                ".docx" =>
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",

                ".xlsx" =>
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",

                _ => "application/octet-stream"
            };
        }
    }
}