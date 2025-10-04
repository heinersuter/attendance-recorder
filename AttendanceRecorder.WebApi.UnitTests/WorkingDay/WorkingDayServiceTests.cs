using AttendanceRecorder.FileSystemStorage;
using AttendanceRecorder.WebApi.WorkingDay;
using Microsoft.Extensions.Options;
using Shouldly;

namespace AttendanceRecorder.WebApi.UnitTests.WorkingDay;

public class WorkingDayServiceTests
{
    private static readonly IOptions<LifeSignConfig> Options =
        Microsoft.Extensions.Options.Options.Create(new LifeSignConfig { UpdatePeriod = TimeSpan.FromSeconds(30) });

    [Test]
    public void Build_NoLifeSigns_SingleInactiveInterval()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.Now);
        var lifeSigns = Array.Empty<TimeOnly>().OrderBy(time => time.Hour);

        // Act
        var workingDayService = new WorkingDayService(Options);
        var workingDay = workingDayService.Build(today, lifeSigns);

        // Assert
        workingDay.Date.ShouldBe(today);
        workingDay.Intervals.Count.ShouldBe(1);
        workingDay.Intervals[0].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = false, Start = new TimeOnly(0, 0, 0), End = new TimeOnly(23, 59, 59),
        });
    }

    [Test]
    public void Build_SingleLifeSign_3Intervals()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.Now);
        var lifeSigns = new[] { new TimeOnly(8, 0, 0) }.OrderBy(time => time.Hour);

        // Act
        var workingDayService = new WorkingDayService(Options);
        var workingDay = workingDayService.Build(today, lifeSigns);

        // Assert
        workingDay.Date.ShouldBe(today);
        workingDay.Intervals.Count.ShouldBe(3);
        workingDay.Intervals[0].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = false, Start = new TimeOnly(0, 0, 0), End = new TimeOnly(7, 59, 59),
        });
        workingDay.Intervals[1].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = true, Start = new TimeOnly(8, 0, 0), End = new TimeOnly(8, 0, 0),
        });
        workingDay.Intervals[2].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = false, Start = new TimeOnly(8, 0, 1), End = new TimeOnly(23, 59, 59),
        });
    }

    [Test]
    public void Build_2CloseLifeSigns_3Intervals()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.Now);
        var lifeSigns = new[] { new TimeOnly(8, 0, 0), new TimeOnly(8, 0, 30) }.OrderBy(time => time.Hour);

        // Act
        var workingDayService = new WorkingDayService(Options);
        var workingDay = workingDayService.Build(today, lifeSigns);

        // Assert
        workingDay.Date.ShouldBe(today);
        workingDay.Intervals.Count.ShouldBe(3);
        workingDay.Intervals[0].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = false, Start = new TimeOnly(0, 0, 0), End = new TimeOnly(7, 59, 59),
        });
        workingDay.Intervals[1].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = true, Start = new TimeOnly(8, 0, 0), End = new TimeOnly(8, 0, 30),
        });
        workingDay.Intervals[2].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = false, Start = new TimeOnly(8, 0, 31), End = new TimeOnly(23, 59, 59),
        });
    }

    [Test]
    public void Build_2DistancedLifeSigns_5Intervals()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.Now);
        var lifeSigns = new[] { new TimeOnly(8, 0, 0), new TimeOnly(9, 0, 0) }.OrderBy(time => time.Hour);

        // Act
        var workingDayService = new WorkingDayService(Options);
        var workingDay = workingDayService.Build(today, lifeSigns);

        // Assert
        workingDay.Date.ShouldBe(today);
        workingDay.Intervals.Count.ShouldBe(5);
        workingDay.Intervals[0].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = false, Start = new TimeOnly(0, 0, 0), End = new TimeOnly(7, 59, 59),
        });
        workingDay.Intervals[1].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = true, Start = new TimeOnly(8, 0, 0), End = new TimeOnly(8, 0, 0),
        });
        workingDay.Intervals[2].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = false, Start = new TimeOnly(8, 0, 1), End = new TimeOnly(8, 59, 59),
        });
        workingDay.Intervals[3].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = true, Start = new TimeOnly(9, 0, 0), End = new TimeOnly(9, 0, 0),
        });
        workingDay.Intervals[4].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = false, Start = new TimeOnly(9, 0, 1), End = new TimeOnly(23, 59, 59),
        });
    }

    [Test]
    public void Build_2DistancedGroupsOfLifeSigns_5Intervals()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.Now);
        var lifeSigns =
            new[] { new TimeOnly(8, 0, 0), new TimeOnly(8, 0, 30), new TimeOnly(9, 0, 0), new TimeOnly(9, 0, 30) }
                .OrderBy(time => time.Hour);

        // Act
        var workingDayService = new WorkingDayService(Options);
        var workingDay = workingDayService.Build(today, lifeSigns);

        // Assert
        workingDay.Date.ShouldBe(today);
        workingDay.Intervals.Count.ShouldBe(5);
        workingDay.Intervals[0].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = false, Start = new TimeOnly(0, 0, 0), End = new TimeOnly(7, 59, 59),
        });
        workingDay.Intervals[1].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = true, Start = new TimeOnly(8, 0, 0), End = new TimeOnly(8, 0, 30),
        });
        workingDay.Intervals[2].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = false, Start = new TimeOnly(8, 0, 31), End = new TimeOnly(8, 59, 59),
        });
        workingDay.Intervals[3].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = true, Start = new TimeOnly(9, 0, 0), End = new TimeOnly(9, 0, 30),
        });
        workingDay.Intervals[4].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = false, Start = new TimeOnly(9, 0, 31), End = new TimeOnly(23, 59, 59),
        });
    }

    [Test]
    public void Build_SingleLifeSignCloseAfterStartOfDay_2Intervals()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.Now);
        var lifeSigns = new[] { new TimeOnly(0, 0, 30) }.OrderBy(time => time.Hour);

        // Act
        var workingDayService = new WorkingDayService(Options);
        var workingDay = workingDayService.Build(today, lifeSigns);

        // Assert
        workingDay.Date.ShouldBe(today);
        workingDay.Intervals.Count.ShouldBe(2);
        workingDay.Intervals[0].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = true, Start = new TimeOnly(0, 0, 0), End = new TimeOnly(0, 0, 30),
        });
        workingDay.Intervals[1].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = false, Start = new TimeOnly(0, 0, 31), End = new TimeOnly(23, 59, 59),
        });
    }

    [Test]
    public void Build_SingleLifeSignCloseBeforeEndOfDay_2Intervals()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.Now);
        var lifeSigns = new[] { new TimeOnly(23, 59, 30) }.OrderBy(time => time.Hour);

        // Act
        var workingDayService = new WorkingDayService(Options);
        var workingDay = workingDayService.Build(today, lifeSigns);

        // Assert
        workingDay.Date.ShouldBe(today);
        workingDay.Intervals.Count.ShouldBe(2);
        workingDay.Intervals[0].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = false, Start = new TimeOnly(0, 0, 0), End = new TimeOnly(23, 59, 29),
        });
        workingDay.Intervals[1].ShouldBeEquivalentTo(new IntervalDto
        {
            IsActive = true, Start = new TimeOnly(23, 59, 30), End = new TimeOnly(23, 59, 59),
        });
    }
}