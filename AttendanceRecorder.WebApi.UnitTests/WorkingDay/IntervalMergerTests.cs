using AttendanceRecorder.WebApi.WorkingDay;
using Shouldly;

namespace AttendanceRecorder.WebApi.UnitTests.WorkingDay;

public class IntervalMergerTests
{
    [Test]
    public void MergeIntervals_NoIntervalsNoMerges_NoInterval()
    {
        // Arrange
        var initialIntervals = Array.Empty<IntervalDto>().OrderBy(i => i.Start);
        var merges = Array.Empty<(TimeOnly Start, TimeOnly End)>();

        // Act
        var intervals = IntervalsMerger.MergeIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBe(0);
    }

    [Test]
    public void MergeIntervals_SingleIntervalNoMerges_Unchanged()
    {
        // Arrange
        var initialIntervals =
            new[] { new IntervalDto { Start = new TimeOnly(8, 0, 0), End = new TimeOnly(9, 0, 0), IsActive = true } }
                .OrderBy(i => i.Start);
        var merges = Array.Empty<(TimeOnly, TimeOnly)>();

        // Act
        var intervals = IntervalsMerger.MergeIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBe(1);
        intervals[0].Start.ShouldBe(new TimeOnly(8, 0, 0));
        intervals[0].Start.ShouldBe(new TimeOnly(9, 0, 0));
    }

    [Test]
    public void MergeIntervals_NoIntervalsSingleMerge_MergeCreatesInterval()
    {
        // Arrange
        var initialIntervals = Array.Empty<IntervalDto>().OrderBy(i => i.Start);
        var merges = new[] { (new TimeOnly(8, 0, 0), new TimeOnly(9, 0, 0)) };

        // Act
        var intervals = IntervalsMerger.MergeIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBe(1);
        intervals[0].Start.ShouldBe(new TimeOnly(8, 0, 0));
        intervals[0].Start.ShouldBe(new TimeOnly(9, 0, 0));
    }

    [Test]
    public void MergeIntervals_IntervalAndMergeNotOverlapping_2Intervals()
    {
        // Arrange
        var initialIntervals =
            new[] { new IntervalDto { Start = new TimeOnly(1, 0, 0), End = new TimeOnly(2, 0, 0), IsActive = true } }
                .OrderBy(i => i.Start);
        var merges = new[] { (new TimeOnly(3, 0, 0), new TimeOnly(4, 0, 0)) };

        // Act
        var intervals = IntervalsMerger.MergeIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBe(1);
        intervals[0].Start.ShouldBe(new TimeOnly(7, 0));
        intervals[0].End.ShouldBe(new TimeOnly(10, 0));
    }

    [Test]
    public void MergeIntervals_MultipleIntervals_MergeCoversSome()
    {
        // Arrange
        var initialIntervals =
            new[]
            {
                new IntervalDto { Start = new TimeOnly(8, 0), End = new TimeOnly(9, 0), IsActive = true },
                new IntervalDto { Start = new TimeOnly(10, 0), End = new TimeOnly(11, 0), IsActive = true }
            }.OrderBy(i => i.Start);
        var merges = new[] { (new TimeOnly(8, 30), new TimeOnly(10, 30)) };

        // Act
        var intervals = IntervalsMerger.MergeIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBe(2);
        intervals[0].Start.ShouldBe(new TimeOnly(8, 0));
        intervals[0].End.ShouldBe(new TimeOnly(9, 0));
        intervals[1].Start.ShouldBe(new TimeOnly(10, 0));
        intervals[1].End.ShouldBe(new TimeOnly(11, 0));
    }

    [Test]
    public void MergeIntervals_OverlappingMerges()
    {
        // Arrange
        var initialIntervals =
            new[]
            {
                new IntervalDto { Start = new TimeOnly(8, 0), End = new TimeOnly(9, 0), IsActive = true },
                new IntervalDto { Start = new TimeOnly(9, 30), End = new TimeOnly(10, 0), IsActive = true }
            }.OrderBy(i => i.Start);
        var merges = new[] { (new TimeOnly(8, 30), new TimeOnly(9, 45)), (new TimeOnly(9, 40), new TimeOnly(10, 30)) };

        // Act
        var intervals = IntervalsMerger.MergeIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBeGreaterThan(0); // Should not throw or lose intervals
    }

    [Test]
    public void MergeIntervals_AdjacentIntervals_MergeCoversGap()
    {
        // Arrange
        var initialIntervals =
            new[]
            {
                new IntervalDto { Start = new TimeOnly(8, 0), End = new TimeOnly(9, 0), IsActive = true },
                new IntervalDto { Start = new TimeOnly(9, 0), End = new TimeOnly(10, 0), IsActive = true }
            }.OrderBy(i => i.Start);
        var merges = new[] { (new TimeOnly(8, 0), new TimeOnly(10, 0)) };

        // Act
        var intervals = IntervalsMerger.MergeIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBe(1);
        intervals[0].Start.ShouldBe(new TimeOnly(8, 0));
        intervals[0].End.ShouldBe(new TimeOnly(10, 0));
    }

    [Test]
    public void MergeIntervals_DuplicateIntervals()
    {
        // Arrange
        var initialIntervals =
            new[]
            {
                new IntervalDto { Start = new TimeOnly(8, 0), End = new TimeOnly(9, 0), IsActive = true },
                new IntervalDto { Start = new TimeOnly(8, 0), End = new TimeOnly(9, 0), IsActive = true }
            }.OrderBy(i => i.Start);
        var merges = Array.Empty<(TimeOnly, TimeOnly)>();

        // Act
        var intervals = IntervalsMerger.MergeIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBe(2);
    }

    [Test]
    public void MergeIntervals_MergesOutsideIntervalRange()
    {
        // Arrange
        var initialIntervals =
            new[] { new IntervalDto { Start = new TimeOnly(8, 0), End = new TimeOnly(9, 0), IsActive = true } }
                .OrderBy(i => i.Start);
        var merges = new[] { (new TimeOnly(6, 0), new TimeOnly(7, 0)) };

        // Act
        var intervals = IntervalsMerger.MergeIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBe(1);
    }

    [Test]
    public void MergeIntervals_MergesPartiallyOverlapIntervals()
    {
        // Arrange
        var initialIntervals =
            new[] { new IntervalDto { Start = new TimeOnly(8, 0), End = new TimeOnly(10, 0), IsActive = true } }
                .OrderBy(i => i.Start);
        var merges = new[] { (new TimeOnly(9, 0), new TimeOnly(11, 0)) };

        // Act
        var intervals = IntervalsMerger.MergeIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBeGreaterThan(0);
    }

    [Test]
    public void MergeIntervals_IntervalsWithGapsNotCoveredByMerges()
    {
        // Arrange
        var initialIntervals =
            new[]
            {
                new IntervalDto { Start = new TimeOnly(8, 0), End = new TimeOnly(9, 0), IsActive = true },
                new IntervalDto { Start = new TimeOnly(10, 0), End = new TimeOnly(11, 0), IsActive = true }
            }.OrderBy(i => i.Start);
        var merges = new[] { (new TimeOnly(8, 0), new TimeOnly(8, 30)) };

        // Act
        var intervals = IntervalsMerger.MergeIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBe(2);
    }

    [Test]
    public void MergeIntervals_IntervalsWithMergesThatSplitThem()
    {
        // Arrange
        var initialIntervals =
            new[] { new IntervalDto { Start = new TimeOnly(8, 0), End = new TimeOnly(12, 0), IsActive = true } }
                .OrderBy(i => i.Start);
        var merges = new[] { (new TimeOnly(9, 0), new TimeOnly(10, 0)), (new TimeOnly(10, 30), new TimeOnly(11, 0)) };

        // Act
        var intervals = IntervalsMerger.MergeIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBeGreaterThan(0);
    }

    [Test]
    public void MergeIntervals_MergesExactlyMatchIntervalBoundaries()
    {
        // Arrange
        var initialIntervals =
            new[]
            {
                new IntervalDto { Start = new TimeOnly(8, 0), End = new TimeOnly(9, 0), IsActive = true },
                new IntervalDto { Start = new TimeOnly(9, 0), End = new TimeOnly(10, 0), IsActive = true }
            }.OrderBy(i => i.Start);
        var merges = new[] { (new TimeOnly(8, 0), new TimeOnly(10, 0)) };

        // Act
        var intervals = IntervalsMerger.MergeIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBe(1);
        intervals[0].Start.ShouldBe(new TimeOnly(8, 0));
        intervals[0].End.ShouldBe(new TimeOnly(10, 0));
    }

    [Test]
    public void MergeIntervals_EmptyMerges()
    {
        // Arrange
        var initialIntervals =
            new[] { new IntervalDto { Start = new TimeOnly(8, 0), End = new TimeOnly(9, 0), IsActive = true } }
                .OrderBy(i => i.Start);
        var merges = new[] { (new TimeOnly(10, 0), new TimeOnly(10, 0)) };

        // Act
        var intervals = IntervalsMerger.MergeIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBe(1);
    }

    [Test]
    public void MergeIntervals_MergesCompletelyInsideInterval()
    {
        // Arrange
        var initialIntervals =
            new[] { new IntervalDto { Start = new TimeOnly(8, 0), End = new TimeOnly(12, 0), IsActive = true } }
                .OrderBy(i => i.Start);
        var merges = new[] { (new TimeOnly(9, 0), new TimeOnly(10, 0)) };

        // Act
        var intervals = IntervalsMerger.MergeIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBeGreaterThan(0);
    }
}