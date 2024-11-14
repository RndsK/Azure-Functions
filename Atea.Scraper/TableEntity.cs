using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;

namespace Atea.Scraper
{
    internal class TableEntity : ITableEntity
    {
        public TableEntity( bool success)
        {
            Success = success;
        }

        public required string PartitionKey { get; set; }
        public required string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public bool Success { get; set; }


    }
}
