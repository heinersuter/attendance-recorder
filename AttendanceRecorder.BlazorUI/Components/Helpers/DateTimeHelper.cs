namespace AttendanceRecorder.BlazorUi.Components.Helpers;

public static class DateTimeHelper
{
    public static DateOnly ToDateOnly(this DateTimeOffset dateTimeOffset)
    {
        return DateOnly.FromDateTime(dateTimeOffset.Date);
    }
}