// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Felicity.Util;

public static class ResetUtils
{
    private const int DailyResetHour = 17;

    private static int ConvertDayOfWeekToInt(DayOfWeek dayOfWeek)
    {
        return dayOfWeek switch
        {
            DayOfWeek.Sunday => 7,
            DayOfWeek.Monday => 1,
            DayOfWeek.Tuesday => 2,
            DayOfWeek.Wednesday => 3,
            DayOfWeek.Thursday => 4,
            DayOfWeek.Friday => 5,
            DayOfWeek.Saturday => 6,
            _ => throw new ArgumentOutOfRangeException(nameof(dayOfWeek), dayOfWeek, null)
        };
    }

    public static DateTime GetNextDailyReset()
    {
        var currentDate = DateTime.UtcNow;
        var dateInQuestion = new DateTime(
            currentDate.Year,
            currentDate.Month,
            currentDate.Day,
            DailyResetHour, 0, 0);
        return dateInQuestion < currentDate ? dateInQuestion.AddDays(1) : dateInQuestion;
    }

    public static DateTime GetNextWeeklyReset(int day)
    {
        var currentDate = DateTime.UtcNow;

        var dateInQuestion = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, DailyResetHour, 0, 0);

        var currentDay = ConvertDayOfWeekToInt(dateInQuestion.DayOfWeek);

        if (currentDay < day)
            dateInQuestion = dateInQuestion.AddDays(day - currentDay);
        else if (currentDay > day) dateInQuestion = dateInQuestion.AddDays(7 - (currentDay - day));

        return dateInQuestion;
    }

    public static DateTime GetNextWeeklyReset(DayOfWeek day)
    {
        var dayNumber = ConvertDayOfWeekToInt(day);
        return GetNextWeeklyReset(dayNumber);
    }
}
