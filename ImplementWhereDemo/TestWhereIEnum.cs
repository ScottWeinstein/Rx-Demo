using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RXDemo
{
    public class TestWhereIEnum
    {
        private IEnumerable<int> wholeNums;
        private IEnumerable<int> repeats;

        public TestWhereIEnum()
        {
            Random rnd = new Random();
            repeats = Enumerable.Repeat(rnd.Next(int.MaxValue - 100), 10);
            wholeNums = Enumerable.Range(0, 100);
            Enumerable.Empty<int>();

        }

        [Fact]
        public void TestRealVsYeild()
        {
            var expected = wholeNums.Where(x => x > wholeNums.First() + 5);
            var actual = RXDemo.CustomWhereCombinator.CustomWhere(wholeNums, x => x > wholeNums.First() + 5);
            Assert.Equal(expected.ToArray(), actual.ToArray());
        }

        [Fact]
        public void TestRealVPedantic()
        {
            var expected = wholeNums.Where(x => x > wholeNums.First() + 5);
            var actual = RXDemo.CustomWhereCombinator.CustomWhereNoYield(wholeNums, x => x > wholeNums.First() + 5);
            Assert.Equal(expected.ToArray(), actual.ToArray());
        }

        [Fact]
        public void TestRealVsYeildRepeatsMatch()
        {
            var expected = repeats.Where(x => x == repeats.First());
            var actual = RXDemo.CustomWhereCombinator.CustomWhere(repeats, x => x == repeats.First());
            Assert.Equal(expected.ToArray(), actual.ToArray());
        }

        [Fact]
        public void TestRealVPedanticRepeatsMatch()
        {
            var expected = repeats.Where(x => x == repeats.First());
            var actual = RXDemo.CustomWhereCombinator.CustomWhereNoYield(repeats, x => x == repeats.First());
            Assert.Equal(expected.ToArray(), actual.ToArray());
        }



    }
}
