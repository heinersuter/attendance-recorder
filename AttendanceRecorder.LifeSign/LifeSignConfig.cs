namespace AttendanceRecorder.LifeSign;

public class LifeSignConfig
{
    public required string Directory { get; init; }
    public required TimeSpan UpdatePeriod { get; init; }

}