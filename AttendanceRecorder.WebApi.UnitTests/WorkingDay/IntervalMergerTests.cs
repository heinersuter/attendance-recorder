using AttendanceRecorder.FileSystemStorage;
using AttendanceRecorder.WebApi.WorkingDay;
using Shouldly;

namespace AttendanceRecorder.WebApi.UnitTests.WorkingDay;

public class IntervalMergerTests
{
    private static readonly LifeSignConfig Config = new() { UpdatePeriod = TimeSpan.FromSeconds(30) };

    [Test]
    public void MergeActiveIntervals_NoIntervalsNoMerges_NoInterval()
    {
        // Arrange
        var initialIntervals = Array.Empty<IntervalDto>();
        var merges = Array.Empty<(TimeOnly Start, TimeOnly End)>();

        // Act
        var intervalsMerger = new IntervalsMerger(Config);
        var intervals = intervalsMerger.MergeActiveIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBe(0);
    }

    [Test]
    public void MergeActiveIntervals_SingleIntervalNoMerges_Unchanged()
    {
        // Arrange
        var initialIntervals =
            new[] { new IntervalDto { Start = new TimeOnly(8, 0, 0), End = new TimeOnly(9, 0, 0), IsActive = true } };
        var merges = Array.Empty<(TimeOnly, TimeOnly)>();

        // Act
        var intervalsMerger = new IntervalsMerger(Config);
        var intervals = intervalsMerger.MergeActiveIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBe(1);
        intervals[0].Start.ShouldBe(new TimeOnly(8, 0, 0));
        intervals[0].End.ShouldBe(new TimeOnly(9, 0, 0));
    }

    [Test]
    public void MergeActiveIntervals_NoIntervalsSingleMerge_MergeCreatesInterval()
    {
        // Arrange
        var initialIntervals = Array.Empty<IntervalDto>();
        var merges = new[] { (new TimeOnly(8, 0, 0), new TimeOnly(9, 0, 0)) };

        // Act
        var intervalsMerger = new IntervalsMerger(Config);
        var intervals = intervalsMerger.MergeActiveIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBe(1);
        intervals[0].Start.ShouldBe(new TimeOnly(8, 0, 0));
        intervals[0].End.ShouldBe(new TimeOnly(9, 0, 0));
    }

    [Test]
    public void MergeActiveIntervals_IntervalAndMergeNotOverlapping_2Intervals()
    {
        // Arrange
        var initialIntervals =
            new[] { new IntervalDto { Start = new TimeOnly(1, 0, 0), End = new TimeOnly(2, 0, 0), IsActive = true } };
        var merges = new[] { (new TimeOnly(3, 0, 0), new TimeOnly(4, 0, 0)) };

        // Act
        var intervalsMerger = new IntervalsMerger(Config);
        var intervals = intervalsMerger.MergeActiveIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBe(2);
        intervals[0].Start.ShouldBe(new TimeOnly(1, 0, 0));
        intervals[0].End.ShouldBe(new TimeOnly(2, 0, 0));
        intervals[1].Start.ShouldBe(new TimeOnly(3, 0, 0));
        intervals[1].End.ShouldBe(new TimeOnly(4, 0, 0));
    }

    [Test]
    public void MergeActiveIntervals_StartOfMergeInInterval_MergedCorrectly()
    {
        // Arrange
        var initialIntervals =
            new[] { new IntervalDto { Start = new TimeOnly(8, 0, 0), End = new TimeOnly(9, 0, 0), IsActive = true } };
        var merges = new[] { (new TimeOnly(8, 30, 0), new TimeOnly(9, 30, 0)) };

        // Act
        var intervalsMerger = new IntervalsMerger(Config);
        var intervals = intervalsMerger.MergeActiveIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBe(1);
        intervals[0].Start.ShouldBe(new TimeOnly(8, 0, 0));
        intervals[0].End.ShouldBe(new TimeOnly(9, 30, 0));
    }

    [Test]
    public void MergeActiveIntervals_EndOfMergeInInterval_MergedCorrectly()
    {
        // Arrange
        var initialIntervals =
            new[] { new IntervalDto { Start = new TimeOnly(8, 0, 0), End = new TimeOnly(9, 0, 0), IsActive = true } };
        var merges = new[] { (new TimeOnly(7, 30, 0), new TimeOnly(8, 30, 0)) };

        // Act
        var intervalsMerger = new IntervalsMerger(Config);
        var intervals = intervalsMerger.MergeActiveIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBe(1);
        intervals[0].Start.ShouldBe(new TimeOnly(7, 30, 0));
        intervals[0].End.ShouldBe(new TimeOnly(9, 0, 0));
    }

    [Test]
    public void MergeActiveIntervals_MergeCovers1Interval_SingleInterval()
    {
        // Arrange
        var initialIntervals =
            new[] { new IntervalDto { Start = new TimeOnly(8, 0, 0), End = new TimeOnly(9, 0, 0), IsActive = true } };
        var merges = new[] { (new TimeOnly(7, 30, 0), new TimeOnly(9, 30, 0)) };

        // Act
        var intervalsMerger = new IntervalsMerger(Config);
        var intervals = intervalsMerger.MergeActiveIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBe(1);
        intervals[0].Start.ShouldBe(new TimeOnly(7, 30, 0));
        intervals[0].End.ShouldBe(new TimeOnly(9, 30, 0));
    }

    [Test]
    public void MergeActiveIntervals_MergeCovers2Intervals_SingleInterval()
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
        var intervals = intervalsMerger.MergeActiveIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBe(1);
        intervals[0].Start.ShouldBe(new TimeOnly(7, 30, 0));
        intervals[0].End.ShouldBe(new TimeOnly(11, 30, 0));
    }

    [Test]
    public void MergeActiveIntervals_MergesCompletelyInsideInterval_SingleUnchangedInterval()
    {
        // Arrange
        var initialIntervals =
            new[] { new IntervalDto { Start = new TimeOnly(8, 0, 0), End = new TimeOnly(11, 0, 0), IsActive = true } };
        var merges = new[] { (new TimeOnly(9, 0, 0), new TimeOnly(10, 0, 0)) };

        // Act
        var intervalsMerger = new IntervalsMerger(Config);
        var intervals = intervalsMerger.MergeActiveIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBe(1);
        intervals[0].Start.ShouldBe(new TimeOnly(8, 0, 0));
        intervals[0].End.ShouldBe(new TimeOnly(11, 0, 0));
    }

    [Test]
    public void MergeInactiveIntervals_NoIntervalsNoMerges_NoInterval()
    {
        // Arrange
        var initialIntervals = Array.Empty<IntervalDto>();
        var merges = Array.Empty<(TimeOnly Start, TimeOnly End)>();

        // Act
        var intervalsMerger = new IntervalsMerger(Config);
        var intervals = intervalsMerger.MergeInactiveIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBe(0);
    }

    [Test]
    public void MergeInactiveIntervals_SingleIntervalsNoMerges_KeepSingleInterval()
    {
        // Arrange
        var initialIntervals =
            new[] { new IntervalDto { Start = new TimeOnly(8, 0, 0), End = new TimeOnly(9, 0, 0), IsActive = true } };
        var merges = Array.Empty<(TimeOnly Start, TimeOnly End)>();

        // Act
        var intervalsMerger = new IntervalsMerger(Config);
        var intervals = intervalsMerger.MergeInactiveIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBe(1);
        intervals[0].Start.ShouldBe(new TimeOnly(8, 0, 0));
        intervals[0].End.ShouldBe(new TimeOnly(9, 0, 0));
    }

    [Test]
    public void MergeInactiveIntervals_SingleIntervalsNoMatchingMerges_KeepSingleInterval()
    {
        // Arrange
        var initialIntervals =
            new[] { new IntervalDto { Start = new TimeOnly(8, 0, 0), End = new TimeOnly(9, 0, 0), IsActive = true } };
        var merges = new[] { (new TimeOnly(9, 0, 1), new TimeOnly(9, 59, 59)) };

        // Act
        var intervalsMerger = new IntervalsMerger(Config);
        var intervals = intervalsMerger.MergeInactiveIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBe(1);
        intervals[0].Start.ShouldBe(new TimeOnly(8, 0, 0));
        intervals[0].End.ShouldBe(new TimeOnly(9, 0, 0));
    }

    [Test]
    public void MergeInactiveIntervals_SingleIntervalsCompletelyCoveredByMerge_NoInterval()
    {
        // Arrange
        var initialIntervals =
            new[] { new IntervalDto { Start = new TimeOnly(8, 0, 0), End = new TimeOnly(9, 0, 0), IsActive = true } };
        var merges = new[] { (new TimeOnly(7, 0, 1), new TimeOnly(9, 59, 59)) };

        // Act
        var intervalsMerger = new IntervalsMerger(Config);
        var intervals = intervalsMerger.MergeInactiveIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBe(0);
    }

    [Test]
    public void MergeInactiveIntervals_SingleIntervalsExactMatchingMerge_NoInterval()
    {
        // Arrange
        var initialIntervals =
            new[] { new IntervalDto { Start = new TimeOnly(8, 0, 0), End = new TimeOnly(9, 0, 0), IsActive = true } };
        var merges = new[] { (new TimeOnly(8, 0, 0), new TimeOnly(9, 0, 0)) };

        // Act
        var intervalsMerger = new IntervalsMerger(Config);
        var intervals = intervalsMerger.MergeInactiveIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBe(0);
    }

    [Test]
    public void MergeInactiveIntervals_SingleIntervalsMergeIside_NoInterval()
    {
        // Arrange
        var initialIntervals =
            new[] { new IntervalDto { Start = new TimeOnly(8, 0, 0), End = new TimeOnly(9, 0, 0), IsActive = true } };
        var merges = new[] { (new TimeOnly(8, 15, 1), new TimeOnly(8, 44, 59)) };

        // Act
        var intervalsMerger = new IntervalsMerger(Config);
        var intervals = intervalsMerger.MergeInactiveIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBe(2);
        intervals[0].Start.ShouldBe(new TimeOnly(8, 0, 0));
        intervals[0].End.ShouldBe(new TimeOnly(8, 15, 0));
        intervals[1].Start.ShouldBe(new TimeOnly(8, 45, 0));
        intervals[1].End.ShouldBe(new TimeOnly(9, 0, 0));
    }

    [Test]
    public void MergeInactiveIntervals_TwoIntervalsCoveredByMerge_NoInterval()
    {
        // Arrange
        var initialIntervals =
            new[]
            {
                new IntervalDto { Start = new TimeOnly(8, 0, 0), End = new TimeOnly(9, 0, 0), IsActive = true },
                new IntervalDto { Start = new TimeOnly(10, 0, 0), End = new TimeOnly(11, 0, 0), IsActive = true },
            };
        var merges = new[] { (new TimeOnly(7, 0, 0), new TimeOnly(12, 0, 0)) };

        // Act
        var intervalsMerger = new IntervalsMerger(Config);
        var intervals = intervalsMerger.MergeInactiveIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBe(0);
    }

    [Test]
    public void MergeInactiveIntervals_SingleIntervalsIntersectionAtEnd_ShorterInterval()
    {
        // Arrange
        var initialIntervals =
            new[] { new IntervalDto { Start = new TimeOnly(8, 0, 0), End = new TimeOnly(9, 0, 0), IsActive = true } };
        var merges = new[] { (new TimeOnly(8, 30, 1), new TimeOnly(9, 59, 59)) };

        // Act
        var intervalsMerger = new IntervalsMerger(Config);
        var intervals = intervalsMerger.MergeInactiveIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBe(1);
        intervals[0].Start.ShouldBe(new TimeOnly(8, 0, 0));
        intervals[0].End.ShouldBe(new TimeOnly(8, 30, 0));
    }

    [Test]
    public void MergeInactiveIntervals_SingleIntervalsIntersectionAtStart_ShorterInterval()
    {
        // Arrange
        var initialIntervals =
            new[] { new IntervalDto { Start = new TimeOnly(8, 0, 0), End = new TimeOnly(9, 0, 0), IsActive = true } };
        var merges = new[] { (new TimeOnly(7, 0, 1), new TimeOnly(8, 29, 59)) };

        // Act
        var intervalsMerger = new IntervalsMerger(Config);
        var intervals = intervalsMerger.MergeInactiveIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBe(1);
        intervals[0].Start.ShouldBe(new TimeOnly(8, 30, 0));
        intervals[0].End.ShouldBe(new TimeOnly(9, 0, 0));
    }

    [Test]
    public void MergeInactiveIntervals_ManyIntervalsManyMerges_CorrectResult()
    {
        // Arrange
        var initialIntervals = new[]
        {
            new IntervalDto { Start = new TimeOnly(1, 0, 0), End = new TimeOnly(2, 0, 0), IsActive = true },
            new IntervalDto { Start = new TimeOnly(3, 0, 0), End = new TimeOnly(4, 0, 0), IsActive = true },
            new IntervalDto { Start = new TimeOnly(5, 0, 0), End = new TimeOnly(6, 0, 0), IsActive = true },
            new IntervalDto { Start = new TimeOnly(7, 0, 0), End = new TimeOnly(8, 0, 0), IsActive = true },
            new IntervalDto { Start = new TimeOnly(9, 0, 0), End = new TimeOnly(10, 0, 0), IsActive = true },
            new IntervalDto { Start = new TimeOnly(11, 0, 0), End = new TimeOnly(12, 0, 0), IsActive = true },
        };
        var merges = new[]
        {
            (new TimeOnly(1, 0, 0), new TimeOnly(3, 29, 59)), (new TimeOnly(7, 30, 1), new TimeOnly(12, 0, 0)),
        };

        // Act
        var intervalsMerger = new IntervalsMerger(Config);
        var intervals = intervalsMerger.MergeInactiveIntervals(initialIntervals, merges).ToList();

        // Assert
        intervals.Count.ShouldBe(3);
        intervals[0].Start.ShouldBe(new TimeOnly(3, 30, 0));
        intervals[0].End.ShouldBe(new TimeOnly(4, 0, 0));
        intervals[1].Start.ShouldBe(new TimeOnly(5, 0, 0));
        intervals[1].End.ShouldBe(new TimeOnly(6, 0, 0));
        intervals[2].Start.ShouldBe(new TimeOnly(7, 0, 0));
        intervals[2].End.ShouldBe(new TimeOnly(7, 30, 0));
    }
}