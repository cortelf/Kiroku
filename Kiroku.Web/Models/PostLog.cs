using System.Text.Json;

namespace Kiroku.Web.Models
{
    public class PostLog
    {
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public LogLevel Level { get; set; } = LogLevel.Trace;
        public JsonDocument Properties { get; set; } = JsonDocument.Parse("{}");
        public required string EventCode { get; set; }

        public void Dispose() => Properties?.Dispose();
    }
}
