using AttendanceRecorder.FileSystemStorage;
using AttendanceRecorder.WebApi.WorkingDay;
using Shouldly;

namespace AttendanceRecorder.WebApi.UnitTests.WorkingDay;

public class IntervalCreatorTests
{
    private static readonly LifeSignConfig Config = new() { UpdatePeriod = TimeSpan.FromSeconds(30) };

    [Test]
    public void CreateActiveIntervals_NoLifeSigns_NoInterval()
    {
        // Arrange
        var lifeSigns = Array.Empty<TimeOnly>();

        // Act
        var intervalCreator = new IntervalCreator(Config);
        var intervals = intervalCreator.CreateActiveIntervals(lifeSigns).ToList();

        // Assert
        intervals.Count.ShouldBe(0);
    }

    [Test]
    public void CreateActiveIntervals_SingleLifeSign_SingleInterval()
    {
        // Arrange
        var lifeSigns = new[] { new TimeOnly(8, 0, 0) };

        // Act
        var intervalCreator = new IntervalCreator(Config);
        var intervals = intervalCreator.CreateActiveIntervals(lifeSigns).ToList();

        // Assert
        intervals.Count.ShouldBe(1);
        intervals[0].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = true, Start = new TimeOnly(8, 0, 0), End = new TimeOnly(8, 0, 0),
        });
    }

    [Test]
    public void CreateActiveIntervals_2CloseLifeSigns_SingleExtendedInterval()
    {
        // Arrange
        var lifeSigns = new[] { new TimeOnly(8, 0, 0), new TimeOnly(8, 0, 30) };

        // Act
        var intervalCreator = new IntervalCreator(Config);
        var intervals = intervalCreator.CreateActiveIntervals(lifeSigns).ToList();

        // Assert
        intervals.Count.ShouldBe(1);
        intervals[0].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = true, Start = new TimeOnly(8, 0, 0), End = new TimeOnly(8, 0, 30),
        });
    }

    [Test]
    public void CreateActiveIntervals_2DistancedLifeSigns_2Intervals()
    {
        // Arrange
        var lifeSigns = new[] { new TimeOnly(8, 0, 0), new TimeOnly(9, 0, 0) };

        // Act
        var intervalCreator = new IntervalCreator(Config);
        var intervals = intervalCreator.CreateActiveIntervals(lifeSigns).ToList();

        // Assert
        intervals.Count.ShouldBe(2);
        intervals[0].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = true, Start = new TimeOnly(8, 0, 0), End = new TimeOnly(8, 0, 0),
        });
        intervals[1].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = true, Start = new TimeOnly(9, 0, 0), End = new TimeOnly(9, 0, 0),
        });
    }

    [Test]
    public void CreateActiveIntervals_2DistancedGroupsOfLifeSigns_2Intervals()
    {
        // Arrange
        var lifeSigns =
            new[] { new TimeOnly(8, 0, 0), new TimeOnly(8, 0, 30), new TimeOnly(9, 0, 0), new TimeOnly(9, 0, 30) };

        // Act
        var intervalCreator = new IntervalCreator(Config);
        var intervals = intervalCreator.CreateActiveIntervals(lifeSigns).ToList();

        // Assert
        intervals.Count.ShouldBe(2);
        intervals[0].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = true, Start = new TimeOnly(8, 0, 0), End = new TimeOnly(8, 0, 30),
        });
        intervals[1].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = true, Start = new TimeOnly(9, 0, 0), End = new TimeOnly(9, 0, 30),
        });
    }

    [Test]
    public void CreateActiveIntervals_SingleLifeSignCloseAfterStartOfDay_2Intervals()
    {
        // Arrange
        var lifeSigns = new[] { new TimeOnly(0, 0, 30) };

        // Act
        var intervalCreator = new IntervalCreator(Config);
        var intervals = intervalCreator.CreateActiveIntervals(lifeSigns).ToList();

        // Assert
        intervals.Count.ShouldBe(1);
        intervals[0].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = true, Start = new TimeOnly(0, 0, 0), End = new TimeOnly(0, 0, 30),
        });
    }

    [Test]
    public void CreateActiveIntervals_SingleLifeSignCloseBeforeEndOfDay_IntervalEndsAtEndOfDay()
    {
        // Arrange
        var lifeSigns = new[] { new TimeOnly(23, 59, 30) };

        // Act
        var intervalCreator = new IntervalCreator(Config);
        var intervals = intervalCreator.CreateActiveIntervals(lifeSigns).ToList();

        // Assert
        intervals.Count.ShouldBe(1);
        intervals[0].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = true, Start = new TimeOnly(23, 59, 30), End = new TimeOnly(23, 59, 59),
        });
    }

    [Test]
    public void CreateInactiveIntervals_NoActiveIntervals_SingleInactiveInterval()
    {
        // Arrange
        var activeIntervals = Array.Empty<IntervalDto>();

        // Act
        var intervalCreator = new IntervalCreator(Config);
        var intervals = intervalCreator.CreateInactiveIntervals(activeIntervals).ToList();

        // Assert
        intervals.Count.ShouldBe(1);
        intervals[0].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = false, Start = new TimeOnly(0, 0, 0), End = new TimeOnly(23, 59, 59),
        });
    }

    [Test]
    public void CreateInactiveIntervals_SingleActiveInterval_3Intervals()
    {
        // Arrange
        var activeIntervals = new[]
        {
            new IntervalDto { IsActive = true, Start = new TimeOnly(8, 0, 0), End = new TimeOnly(8, 0, 0) },
        };

        // Act
        var intervalCreator = new IntervalCreator(Config);
        var intervals = intervalCreator.CreateInactiveIntervals(activeIntervals).ToList();

        // Assert
        intervals.Count.ShouldBe(3);
        intervals[0].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = false, Start = new TimeOnly(0, 0, 0), End = new TimeOnly(7, 59, 59),
        });
        intervals[1].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = true, Start = new TimeOnly(8, 0, 0), End = new TimeOnly(8, 0, 0),
        });
        intervals[2].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = false, Start = new TimeOnly(8, 0, 1), End = new TimeOnly(23, 59, 59),
        });
    }

    [Test]
    public void CreateInactiveIntervals_ActiveIntervalAtStartOfDay_2Intervals()
    {
        // Arrange
        var activeIntervals = new[]
        {
            new IntervalDto { IsActive = true, Start = new TimeOnly(0, 0, 0), End = new TimeOnly(0, 0, 30) },
        };

        // Act
        var intervalCreator = new IntervalCreator(Config);
        var intervals = intervalCreator.CreateInactiveIntervals(activeIntervals).ToList();

        // Assert
        intervals.Count.ShouldBe(2);
        intervals[0].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = true, Start = new TimeOnly(0, 0, 0), End = new TimeOnly(0, 0, 30),
        });
        intervals[1].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = false, Start = new TimeOnly(0, 0, 31), End = new TimeOnly(23, 59, 59),
        });
    }

    [Test]
    public void CreateInactiveIntervals_ActiveIntervalAtEndOfDay_2Intervals()
    {
        // Arrange
        var activeIntervals = new[]
        {
            new IntervalDto { IsActive = true, Start = new TimeOnly(23, 59, 30), End = new TimeOnly(23, 59, 59) },
        };

        // Act
        var intervalCreator = new IntervalCreator(Config);
        var intervals = intervalCreator.CreateInactiveIntervals(activeIntervals).ToList();

        // Assert
        intervals.Count.ShouldBe(2);
        intervals[0].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = false, Start = new TimeOnly(0, 0, 0), End = new TimeOnly(23, 59, 29),
        });
        intervals[1].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = true, Start = new TimeOnly(23, 59, 30), End = new TimeOnly(23, 59, 59),
        });
    }
}