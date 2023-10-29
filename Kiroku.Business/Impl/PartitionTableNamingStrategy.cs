using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kiroku.Business.Impl
{
    public class PartitionTableNamingStrategy : IPartitionTableNamingStrategy
    {
        public string MakePartitionName(DateOnly toDate)
        {
            return toDate.ToString("dd-M-yyyy", CultureInfo.InvariantCulture);
        }

        public DateOnly ParsePartitionName(string partitionName)
        {
            return DateOnly.ParseExact(partitionName, "dd-M-yyyy", CultureInfo.InvariantCulture);
        }
    }
}
