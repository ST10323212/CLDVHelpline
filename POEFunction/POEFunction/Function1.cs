using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudStorageFunctions
{
    public static class TableStorageFunction
    {
        [FunctionName("AddProduct")]
        public static async Task<IActionResult> AddProduct(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [Microsoft.Azure.WebJobs.Table("Products")] TableClient tableClient,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            var product = new TableEntity(partitionKey: "ProductsPartition", rowKey: Guid.NewGuid().ToString())
            {
                { "ProductName", data.ProductName },
                { "Description", data.Description },
                { "Price", data.Price }
            };

            await tableClient.AddEntityAsync(product);

            return new OkObjectResult("Product added successfully");
        }
    }
}


