using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage.Files.Shares;
using Microsoft.Azure.Functions.Worker;

namespace CloudStorageFunctions
{
    public static class FileShareFunction
    {
        [FunctionName("UploadFile")]
        public static async Task<IActionResult> UploadFile(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var formData = await req.ReadFormAsync();
            var file = formData.Files["file"];

            if (file == null)
            {
                return new BadRequestObjectResult("No file was uploaded.");
            }

            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            var shareClient = new ShareClient(connectionString, "fileshare");
            var directoryClient = shareClient.GetDirectoryClient("uploads");
            await directoryClient.CreateIfNotExistsAsync();

            var fileClient = directoryClient.GetFileClient(file.FileName);
            await using var stream = file.OpenReadStream();
            await fileClient.CreateAsync(stream.Length);
            await fileClient.UploadAsync(stream);

            return new OkObjectResult($"File {file.FileName} uploaded to File Share successfully");
        }
    }
}
