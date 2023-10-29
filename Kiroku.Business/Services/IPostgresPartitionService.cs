using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kiroku.Business.Services
{
    public enum CreateNextPartitionResult
    {
        Created, NoNeed
    }
    public interface IPostgresPartitionService
    {
        Task<CreateNextPartitionResult> CreateNextPartition();
        Task DeleteOldPartitions();

    }
}
