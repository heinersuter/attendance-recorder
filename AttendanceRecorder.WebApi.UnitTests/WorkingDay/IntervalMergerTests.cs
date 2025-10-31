using AttendanceRecorder.FileSystemStorage;
using AttendanceRecorder.WebApi.WorkingDay;
using Shouldly;

namespace AttendanceRecorder.WebApi.UnitTests.WorkingDay;

public class IntervalMergerTests
{
    private static readonly LifeSignConfig Config = new() { UpdatePeriod = TimeSpan.FromSeconds(30) };

    [Test]
    public void MergeIntervals_NoIntervalsNoMerges_NoInterval()
    {
        // Arrange
        var initialIntervals = Array.Empty<IntervalDto>();
        var merges = Array.Empty<(TimeOnly Start, TimeOnly End)>();

        // Act
        var intervalsMerger = new IntervalsMerger(Config);
        var intervals = intervalsMerger.MergeIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBe(0);
    }

    [Test]
    public void MergeIntervals_SingleIntervalNoMerges_Unchanged()
    {
        // Arrange
        var initialIntervals =
            new[] { new IntervalDto { Start = new TimeOnly(8, 0, 0), End = new TimeOnly(9, 0, 0), IsActive = true } };
        var merges = Array.Empty<(TimeOnly, TimeOnly)>();

        // Act
        var intervalsMerger = new IntervalsMerger(Config);
        var intervals = intervalsMerger.MergeIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBe(1);
        intervals[0].Start.ShouldBe(new TimeOnly(8, 0, 0));
        intervals[0].End.ShouldBe(new TimeOnly(9, 0, 0));
    }

    [Test]
    public void MergeIntervals_NoIntervalsSingleMerge_MergeCreatesInterval()
    {
        // Arrange
        var initialIntervals = Array.Empty<IntervalDto>();
        var merges = new[] { (new TimeOnly(8, 0, 0), new TimeOnly(9, 0, 0)) };

        // Act
        var intervalsMerger = new IntervalsMerger(Config);
        var intervals = intervalsMerger.MergeIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBe(1);
        intervals[0].Start.ShouldBe(new TimeOnly(8, 0, 0));
        intervals[0].End.ShouldBe(new TimeOnly(9, 0, 0));
    }

    [Test]
    public void MergeIntervals_IntervalAndMergeNotOverlapping_2Intervals()
    {
        // Arrange
        var initialIntervals =
            new[] { new IntervalDto { Start = new TimeOnly(1, 0, 0), End = new TimeOnly(2, 0, 0), IsActive = true } };
        var merges = new[] { (new TimeOnly(3, 0, 0), new TimeOnly(4, 0, 0)) };

        // Act
        var intervalsMerger = new IntervalsMerger(Config);
        var intervals = intervalsMerger.MergeIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBe(2);
        intervals[0].Start.ShouldBe(new TimeOnly(1, 0, 0));
        intervals[0].End.ShouldBe(new TimeOnly(2, 0, 0));
        intervals[1].Start.ShouldBe(new TimeOnly(3, 0, 0));
        intervals[1].End.ShouldBe(new TimeOnly(4, 0, 0));
    }

    [Test]
    public void MergeIntervals_StartOfMergeInInterval_MergedCorrectly()
    {
        // Arrange
        var initialIntervals =
            new[] { new IntervalDto { Start = new TimeOnly(8, 0, 0), End = new TimeOnly(9, 0, 0), IsActive = true } };
        var merges = new[] { (new TimeOnly(8, 30, 0), new TimeOnly(9, 30, 0)) };

        // Act
        var intervalsMerger = new IntervalsMerger(Config);
        var intervals = intervalsMerger.MergeIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBe(1);
        intervals[0].Start.ShouldBe(new TimeOnly(8, 0, 0));
        intervals[0].End.ShouldBe(new TimeOnly(9, 30, 0));
    }

    [Test]
    public void MergeIntervals_EndOfMergeInInterval_MergedCorrectly()
    {
        // Arrange
        var initialIntervals =
            new[] { new IntervalDto { Start = new TimeOnly(8, 0, 0), End = new TimeOnly(9, 0, 0), IsActive = true } };
        var merges = new[] { (new TimeOnly(7, 30, 0), new TimeOnly(8, 30, 0)) };

        // Act
        var intervalsMerger = new IntervalsMerger(Config);
        var intervals = intervalsMerger.MergeIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBe(1);
        intervals[0].Start.ShouldBe(new TimeOnly(7, 30, 0));
        intervals[0].End.ShouldBe(new TimeOnly(9, 0, 0));
    }

    [Test]
    public void MergeIntervals_MergeCovers1Interval_SingleInterval()
    {
        // Arrange
        var initialIntervals =
            new[] { new IntervalDto { Start = new TimeOnly(8, 0, 0), End = new TimeOnly(9, 0, 0), IsActive = true } };
        var merges = new[] { (new TimeOnly(7, 30, 0), new TimeOnly(9, 30, 0)) };

        // Act
        var intervalsMerger = new IntervalsMerger(Config);
        var intervals = intervalsMerger.MergeIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBe(1);
        intervals[0].Start.ShouldBe(new TimeOnly(7, 30, 0));
        intervals[0].End.ShouldBe(new TimeOnly(9, 30, 0));
    }

    [Test]
    public void MergeIntervals_MergeCovers2Intervals_SingleInterval()
    {
        // Arrange
        var initialIntervals =
            new[]
            {
                new IntervalDto { Start = new TimeOnly(8, 0, 0), End = new TimeOnly(9, 0, 0), IsActive = true },
                new IntervalDto { Start = new TimeOnly(10, 0, 0), End = new TimeOnly(11, 0, 0), IsActive = true },
            };
        var merges = new[] { (new TimeOnly(7, 30, 0), new TimeOnly(11, 30, 0)) };

        // Act
        var intervalsMerger = new IntervalsMerger(Config);
        var intervals = intervalsMerger.MergeIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBe(1);
        intervals[0].Start.ShouldBe(new TimeOnly(7, 30, 0));
        intervals[0].End.ShouldBe(new TimeOnly(11, 30, 0));
    }

    [Test]
    public void MergeIntervals_MergesCompletelyInsideInterval_SingleUnchangedInterval()
    {
        // Arrange
        var initialIntervals =
            new[] { new IntervalDto { Start = new TimeOnly(8, 0, 0), End = new TimeOnly(11, 0, 0), IsActive = true } };
        var merges = new[] { (new TimeOnly(9, 0, 0), new TimeOnly(10, 0, 0)) };

        // Act
        var intervalsMerger = new IntervalsMerger(Config);
        var intervals = intervalsMerger.MergeIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBe(1);
        intervals[0].Start.ShouldBe(new TimeOnly(8, 0, 0));
        intervals[0].End.ShouldBe(new TimeOnly(11, 0, 0));
    }
}