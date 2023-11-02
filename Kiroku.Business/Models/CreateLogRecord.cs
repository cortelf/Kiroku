using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Kiroku.Business.Models
{
    public class CreateLogRecord: IDisposable
    {
        public DateTime Timestamp { get; set; }
        public LogLevel Level { get; set; }
        public JsonDocument Properties { get; set; } = JsonDocument.Parse("{}");
        public required string ProjectId { get; set; }
        public string? InstanceId { get; set; }
        public required string EventCode { get; set; }

        public void Dispose() => Properties?.Dispose();
    }
}
