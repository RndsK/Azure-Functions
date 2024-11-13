using System;
using System.Net.Http.Json;
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
           var result = await response.Content.ReadFromJsonAsync<object>();
           _logger.LogInformation(response.StatusCode.ToString());
        }
    }
}
