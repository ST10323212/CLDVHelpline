using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;

namespace POEFunction
{
    public static class BlobStorageFunction
    {
        [FunctionName("UploadBlob")]
        public static async Task<IActionResult> UploadBlob(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [Blob("product-images", Connection = "AzureWebJobsStorage")] BlobContainerClient containerClient,
            ILogger log)
        {
            var formData = await req.ReadFormAsync();
            var file = formData.Files["file"];

            if (file == null)
            {
                return new BadRequestObjectResult("No file was uploaded.");
            }

            var blobClient = containerClient.GetBlobClient(file.FileName);
            await using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, true);

            return new OkObjectResult($"File {file.FileName} uploaded successfully");
        }
    }
}

