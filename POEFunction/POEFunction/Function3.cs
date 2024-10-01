using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Functions.Worker;

namespace POEFunction
{
    public static class QueueFunction
    {
        [FunctionName("EnqueueMessage")]
        public static async Task<IActionResult> EnqueueMessage(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [Queue("customer-queue", Connection = "AzureWebJobsStorage")] IAsyncCollector<string> customerQueue,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string message = data.message;

            await customerQueue.AddAsync(message);

            return new OkObjectResult("Message added to queue successfully");
        }
    }
}

