using Kiroku.Business.Services;
using Kiroku.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kiroku.Web.Controllers
{
    [ApiController]
    [Route("/logs")]
    public class LogsController : ControllerBase
    {
        private readonly ILogService _logService;
        public LogsController(ILogService logService)
        {
            _logService = logService;
        }

        [HttpPost("{projectId}")]
        [HttpPost("{projectId}/{instanceId?}")]
        public async Task CreateLog(PostLog body, string projectId, string? instanceId)
        {
            await _logService.WriteLog(new Business.Models.CreateLogRecord()
            {
                EventCode = body.EventCode,
                InstanceId = instanceId,
                ProjectId = projectId,
                Level = body.Level,
                Properties = body.Properties,
                Timestamp = body.Timestamp
            });
        }

        [HttpPost("batch/{projectId}")]
        [HttpPost("batch/{projectId}/{instanceId?}")]
        public async Task CreateLogBatch(List<PostLog> body, string projectId, string? instanceId)
        {
            await _logService.WriteLogBatch(body.Select(x => new Business.Models.CreateLogRecord()
            {
                EventCode = x.EventCode,
                InstanceId = instanceId,
                ProjectId = projectId,
                Level = x.Level,
                Properties = x.Properties,
                Timestamp = x.Timestamp
            }).ToList());
        }
    }
}
