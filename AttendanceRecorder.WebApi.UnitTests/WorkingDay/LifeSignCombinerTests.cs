using AttendanceRecorder.FileSystemStorage;
using AttendanceRecorder.WebApi.WorkingDay;
using Shouldly;

namespace AttendanceRecorder.WebApi.UnitTests.WorkingDay;

public class LifeSignCombinerTests
{
    private static readonly LifeSignConfig Config = new() { UpdatePeriod = TimeSpan.FromSeconds(30) };

    [Test]
    public void CombineLifeSigns_NoLifeSigns_SingleInactiveInterval()
    {
        // Arrange
        var lifeSigns = Array.Empty<TimeOnly>().OrderBy(time => time.Hour);

        // Act
        var lifeSignCombiner = new LifeSignCombiner(Config);
        var intervals = lifeSignCombiner.CombineLifeSigns(lifeSigns).ToList();

        // Assert
        intervals.Count.ShouldBe(1);
        intervals[0].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = false, Start = new TimeOnly(0, 0, 0), End = new TimeOnly(23, 59, 59),
        });
    }

    [Test]
    public void CombineLifeSigns_SingleLifeSign_3Intervals()
    {
        // Arrange
        var lifeSigns = new[] { new TimeOnly(8, 0, 0) }.OrderBy(time => time.Hour);

        // Act
        var lifeSignCombiner = new LifeSignCombiner(Config);
        var intervals = lifeSignCombiner.CombineLifeSigns(lifeSigns).ToList();

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
    public void CombineLifeSigns_2CloseLifeSigns_3Intervals()
    {
        // Arrange
        var lifeSigns = new[] { new TimeOnly(8, 0, 0), new TimeOnly(8, 0, 30) }.OrderBy(time => time.Hour);

        // Act
        var lifeSignCombiner = new LifeSignCombiner(Config);
        var intervals = lifeSignCombiner.CombineLifeSigns(lifeSigns).ToList();

        // Assert
        intervals.Count.ShouldBe(3);
        intervals[0].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = false, Start = new TimeOnly(0, 0, 0), End = new TimeOnly(7, 59, 59),
        });
        intervals[1].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = true, Start = new TimeOnly(8, 0, 0), End = new TimeOnly(8, 0, 30),
        });
        intervals[2].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = false, Start = new TimeOnly(8, 0, 31), End = new TimeOnly(23, 59, 59),
        });
    }

    [Test]
    public void CombineLifeSigns_2DistancedLifeSigns_5Intervals()
    {
        // Arrange
        var lifeSigns = new[] { new TimeOnly(8, 0, 0), new TimeOnly(9, 0, 0) }.OrderBy(time => time.Hour);

        // Act
        var lifeSignCombiner = new LifeSignCombiner(Config);
        var intervals = lifeSignCombiner.CombineLifeSigns(lifeSigns).ToList();

        // Assert
        intervals.Count.ShouldBe(5);
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
            IsActive = false, Start = new TimeOnly(8, 0, 1), End = new TimeOnly(8, 59, 59),
        });
        intervals[3].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = true, Start = new TimeOnly(9, 0, 0), End = new TimeOnly(9, 0, 0),
        });
        intervals[4].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = false, Start = new TimeOnly(9, 0, 1), End = new TimeOnly(23, 59, 59),
        });
    }

    [Test]
    public void CombineLifeSigns_2DistancedGroupsOfLifeSigns_5Intervals()
    {
        // Arrange
        var lifeSigns =
            new[] { new TimeOnly(8, 0, 0), new TimeOnly(8, 0, 30), new TimeOnly(9, 0, 0), new TimeOnly(9, 0, 30) }
                .OrderBy(time => time.Hour);

        // Act
        var lifeSignCombiner = new LifeSignCombiner(Config);
        var intervals = lifeSignCombiner.CombineLifeSigns(lifeSigns).ToList();

        // Assert
        intervals.Count.ShouldBe(5);
        intervals[0].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = false, Start = new TimeOnly(0, 0, 0), End = new TimeOnly(7, 59, 59),
        });
        intervals[1].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = true, Start = new TimeOnly(8, 0, 0), End = new TimeOnly(8, 0, 30),
        });
        intervals[2].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = false, Start = new TimeOnly(8, 0, 31), End = new TimeOnly(8, 59, 59),
        });
        intervals[3].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = true, Start = new TimeOnly(9, 0, 0), End = new TimeOnly(9, 0, 30),
        });
        intervals[4].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = false, Start = new TimeOnly(9, 0, 31), End = new TimeOnly(23, 59, 59),
        });
    }

    [Test]
    public void CombineLifeSigns_SingleLifeSignCloseAfterStartOfDay_2Intervals()
    {
        // Arrange
        var lifeSigns = new[] { new TimeOnly(0, 0, 30) }.OrderBy(time => time.Hour);

        // Act
        var lifeSignCombiner = new LifeSignCombiner(Config);
        var intervals = lifeSignCombiner.CombineLifeSigns(lifeSigns).ToList();

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
    public void CombineLifeSigns_SingleLifeSignCloseBeforeEndOfDay_2Intervals()
    {
        // Arrange
        var lifeSigns = new[] { new TimeOnly(23, 59, 30) }.OrderBy(time => time.Hour);

        // Act
        var lifeSignCombiner = new LifeSignCombiner(Config);
        var intervals = lifeSignCombiner.CombineLifeSigns(lifeSigns).ToList();

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