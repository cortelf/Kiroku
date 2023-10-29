using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kiroku.Business
{
    public interface IPartitionListAnalyzeStrategy
    {
        bool CheckCreatingNewPartitionIsRequired(DateOnly lastToDate);
        DateOnly GetLastPartitionsDate(IEnumerable<DateOnly> dates);
    }
}
