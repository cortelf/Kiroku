using Kiroku.Business.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kiroku.Business.UnitTests
{
    [TestClass]
    public class PartitionListAnalyzeStrategyTest
    {
        readonly PartitionListAnalyzeStrategy _strategy = new();

        [TestMethod]
        public void GetLastPartitionsDate_EmptyCollection_TodayDate()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var result = _strategy.GetLastPartitionsDate(new List<DateOnly>());

            Assert.AreEqual(today, result);
        }


        [TestMethod]
        public void GetLastPartitionsDate_SomeDates_LastDate()
        {
            var last = new DateOnly(2024, 12, 12);
            var dates = new DateOnly[] { new DateOnly(2021, 1, 1), last, new DateOnly(2024, 1, 1), };

            var result = _strategy.GetLastPartitionsDate(dates);

            Assert.AreEqual(last, result);
        }

        [TestMethod]
        public void CheckCreatingNewPartitionIsRequired_Today_True()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var result = _strategy.CheckCreatingNewPartitionIsRequired(today);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CheckCreatingNewPartitionIsRequired_Tomorrow_True()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1);

            var result = _strategy.CheckCreatingNewPartitionIsRequired(today);

            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        public void CheckCreatingNewPartitionIsRequired_2AndMoreDays_False(int days)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(days);

            var result = _strategy.CheckCreatingNewPartitionIsRequired(today);

            Assert.IsFalse(result);
        }


    }
}
