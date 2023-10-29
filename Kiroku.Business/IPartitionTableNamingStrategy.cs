using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kiroku.Business
{
    public interface IPartitionTableNamingStrategy
    {
        /// <summary>
        /// Converts a PostgreSQL section name to a date
        /// </summary>
        /// <param name="name">Section name</param>
        /// <returns>Section's TO Date</returns>
        DateOnly ParsePartitionName(string partitionName);
        string MakePartitionName(DateOnly toDate);
    }
}
