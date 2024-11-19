using System.Globalization;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ListLogs
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function("Function1")]
        public async Task <IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {

            if (!DateTimeOffset.TryParse(req.Query["from"], out var from) || !DateTimeOffset.TryParse(req.Query["to"], out var to))
            {
                return new BadRequestObjectResult("Invalid 'from' or 'to' date.");
            }

            var tableServiceClient = new TableServiceClient("UseDevelopmentStorage=true");
            await tableServiceClient.CreateTableIfNotExistsAsync("atea");
            var tableClient = tableServiceClient.GetTableClient("atea");

            var records = tableClient.Query<TableEntity>(item =>
                item.Timestamp.Value >= from && item.Timestamp.Value <= to);

            var logs = records.ToList();

            _logger.LogInformation("C# HTTP trigger function processed a request.");

            return new OkObjectResult(logs);
        }
    }
}
