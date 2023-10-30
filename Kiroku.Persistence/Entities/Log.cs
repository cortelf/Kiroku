using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Kiroku.Persistence.Entities
{

    [Table("logs")]
    [Keyless]
    public class Log : IDisposable
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("level")]
        public required LogLevel Level { get; set; }
        [Column("project_id")]
        public required string ProjectId { get; set; }
        [Column("instance_id")]
        public string? InstanceId { get; set; }
        [Column("event_code")]
        public required string EventCode { get; set; }
        [Column("time")]
        public DateTime Time { get; set; }
        [Column("data")]
        public required JsonDocument Data { get; set; }

        public void Dispose() => Data?.Dispose();

    }
}
