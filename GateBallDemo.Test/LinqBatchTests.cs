using System;
using System.Linq;
using FluentAssertions;
using GateBallDemo.Helpers;
using Xunit;

namespace GateBallDemo.Test
{
    public class LinqBatchTests
    {
        [Fact]
        public void GivenEmptySource_WhenBatched_ProducesEmptyEnumerator()
        {
            var source = Enumerable.Empty<object>();
            var result = source.Batch(5);
            result.Should().BeEmpty();
        }

        [Fact]
        public void GivenLongSequence_WhenBatched_ProducesMultipleBatches()
        {
            var source = Enumerable.Range(0, 9);
            var result = source.Batch(3).ToList();
            result.Should().HaveCount(3);
            result[0].Should().BeEquivalentTo(new[] {0, 1, 2}, config => config.WithStrictOrdering());
            result[1].Should().BeEquivalentTo(new[] {3, 4, 5}, config => config.WithStrictOrdering());
            result[2].Should().BeEquivalentTo(new[] {6, 7, 8}, config => config.WithStrictOrdering());
        }

        [Fact]
        public void GivenLessThanAFullBatch_WhenBatched_ProducesShortBatch()
        {
            var source = Enumerable.Range(5, 3);
            var result = source.Batch(9).ToList();
            result.Should().HaveCount(1);
            result[0].Count.Should().Be(3);
            result[0].Should().BeEquivalentTo(Enumerable.Range(5, 3), config => config.WithStrictOrdering());
        }

        [Fact]
        public void GivenLongSequenceWithIncompleteBatch_WhenBatched_ProducesMultipleBatchesWithShortFinal()
        {
            var source = Enumerable.Range(0, 8);
            var result = source.Batch(3).ToList();
            result.Should().HaveCount(3);
            result[0].Should().BeEquivalentTo(new[] {0, 1, 2}, config => config.WithStrictOrdering());
            result[1].Should().BeEquivalentTo(new[] {3, 4, 5}, config => config.WithStrictOrdering());
            result[2].Should().BeEquivalentTo(new[] {6, 7}, config => config.WithStrictOrdering());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void GivenNegativeBatchSize_WhenBatched_Throws(int batchSize)
        {
            Action action = () => Enumerable.Empty<object>().Batch(batchSize).ToList();
            action.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}
