using System.Linq;
using Com.Revo.Games.KaboomEngine.Kaboom;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KaboomEngineTests.KaboomTests.KaboomRandomTests
{
    public partial class KaboomRandomTests
    {
        [TestMethod]
        public void Next_Statistics()
        {
            var sut = new KaboomRandom();
            var results = Enumerable.Range(0, 100000)
                                    .Select(i => sut.Next(100))
                                    .GroupBy(i => i)
                                    .ToDictionary(g => g.Key, g => g.Count());

            for(int i=0; i<100; i++)
                if (!results.ContainsKey(i))
                    results[i] = 0;

            results[99].Should().BeLessThan(results[0]);
            results[75].Should().BeLessThan(results[25]);
            results[25].Should().BeLessThan(results[10]);
            results[60].Should().BeLessThan(results[40]);
            results[80].Should().BeGreaterThan(results[99]);
            results[80].Should().BeLessThan(results[40]);
        }
    }
}