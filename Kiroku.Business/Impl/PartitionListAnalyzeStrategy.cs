using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kiroku.Business.Impl
{
    public class PartitionListAnalyzeStrategy : IPartitionListAnalyzeStrategy
    {
        public DateOnly GetLastPartitionsDate(IEnumerable<DateOnly> dates)
        {
            if (!dates.Any()) return DateOnly.FromDateTime(DateTime.UtcNow);

            var ordered = dates.OrderByDescending(d => d).ToList();
            return ordered.First();
        }
        public bool CheckCreatingNewPartitionIsRequired(DateOnly lastToDate)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            return lastToDate.DayNumber - today.DayNumber <= 1;
        }
    }
}
