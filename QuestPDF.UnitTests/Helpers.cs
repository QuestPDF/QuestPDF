using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests
{
    public static class Helpers
    {
        public static Random Random = new Random();
        
        public static Size RandomSize => new Size(Random.Next(200, 400), Random.Next(100, 200));

        public static void ShouldEqual(this IEnumerable<OperationBase> commands, IEnumerable<OperationBase> expected)
        {
            commands.Should().HaveSameCount(expected);
            
            commands
                .Zip(expected)
                .ToList()
                .ForEach(x =>
                {
                    x.First.Should().BeOfType(x.Second.GetType());
                    x.First.Should().BeEquivalentTo(x.Second, y => y.RespectingRuntimeTypes());
                });
        }
    }
}