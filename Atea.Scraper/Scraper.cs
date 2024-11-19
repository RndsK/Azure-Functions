using System.Net.Http.Json;
using System.Text.Json;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Atea.Scraper
{
    public class Scraper
    {
        private readonly ILogger _logger;

        public Scraper(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Scraper>();
        }

        // https://restcountries.com/v3.1/lang/spanish
        [Function("Scraper")]
        public async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer)
        {
           var client = new HttpClient();
           client.BaseAddress = new Uri("https://restcountries.com");
           var response = await client.GetAsync("/v3.1/lang/spanish");
           var result = await response.Content.ReadFromJsonAsync<ICollection<Country>>();
           var payload = JsonSerializer.Serialize(result);

           var tableServiceClient = new TableServiceClient("UseDevelopmentStorage=true");
           await tableServiceClient.CreateTableIfNotExistsAsync("atea");
           var tableClient = tableServiceClient.GetTableClient("atea");
           var key = Guid.NewGuid();
           await tableClient.AddEntityAsync(new TableEntity(response.IsSuccessStatusCode)
           {
               PartitionKey = key.ToString(),
               RowKey = key.ToString(),
           });

           var blobServiceClient = new BlobServiceClient("UseDevelopmentStorage=true");
           var blobContainerClient = blobServiceClient.GetBlobContainerClient("atea");
           await blobContainerClient.CreateIfNotExistsAsync();

           await blobContainerClient.UploadBlobAsync($"{key}.json", BinaryData.FromString(payload));

            _logger.LogInformation("action performed");
        }
    }
}
