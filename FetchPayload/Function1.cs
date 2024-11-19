using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FetchPayload
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function("Function1")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "payload/{logId}")] HttpRequest req, string logId)
        {
            var blobServiceClient = new BlobServiceClient("UseDevelopmentStorage=true");
            var blobContainerClient = blobServiceClient.GetBlobContainerClient("atea");

            var blobClient = blobContainerClient.GetBlobClient($"{logId}.json");

            var downloadContent = await blobClient.DownloadContentAsync();
            var content = downloadContent.Value.Content.ToString();

            _logger.LogInformation("action performed");

            return new OkObjectResult(content);
        }
    }
}
