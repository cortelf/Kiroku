using Kiroku.Business.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kiroku.Business.UnitTests
{
    [TestClass]
    public class PartitionTableNamingStrategyTest
    {
        readonly PartitionTableNamingStrategy _strategy = new();

        [TestMethod]
        public void MakePartitionName_DateProvided_EqualsWithConst()
        {
            var date = new DateOnly(2023, 10, 20);
            var stringRepresentation = "20-10-2023";

            var result = _strategy.MakePartitionName(date);

            Assert.AreEqual(result, stringRepresentation);
        }


        [TestMethod]
        public void ParsePartitionName_StringProvided_EqualsWithConst()
        {
            var date = new DateOnly(2023, 10, 20);
            var stringRepresentation = "20-10-2023";

            var result = _strategy.ParsePartitionName(stringRepresentation);

            Assert.AreEqual(result, date);
        }

        [TestMethod]
        public void ParsePartitionName_NotADateStringProvided_FormatException()
        {
            var stringRepresentation = "error";

            Assert.ThrowsException<FormatException>(() => _strategy.ParsePartitionName(stringRepresentation));
        }
    }
}
