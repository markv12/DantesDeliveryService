using System;

public static class DateUtility
{
    public static long CurrentEpocTime {
        get {
            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return (long)t.TotalMilliseconds;
        }
    }

    public static string AgoStringForMilliseconds(double milliseconds) {
        const int SECOND = 1;
        const int MINUTE = 60 * SECOND;
        const int HOUR = 60 * MINUTE;
        const int DAY = 24 * HOUR;
        const int MONTH = 30 * DAY;

        TimeSpan ts = TimeSpan.FromMilliseconds(milliseconds);
        double delta = Math.Abs(ts.TotalSeconds);

        if (delta < 1 * MINUTE)
            return ts.Seconds == 1 ? Localizer.GetText("one_second_ago") : FormLoc("seconds_ago", ts.Seconds);

        if (delta < 2 * MINUTE)
            return Localizer.GetText("a_minute_ago");

        if (delta < 45 * MINUTE)
            return FormLoc("minutes_ago", ts.Minutes);

        if (delta < 90 * MINUTE)
            return Localizer.GetText("an_hour_ago");

        if (delta < 24 * HOUR)
            return FormLoc("hours_ago", ts.Hours);

        if (delta < 48 * HOUR)
            return Localizer.GetText("yesterday");

        if (delta < 30 * DAY)
            return FormLoc("days_ago", ts.Days);

        if (delta < 12 * MONTH) {
            int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
            return months <= 1 ? Localizer.GetText("one_month_ago") : FormLoc("months_ago", months);
        } else {
            int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
            return years <= 1 ? Localizer.GetText("one_year_ago") : FormLoc("years_ago", years);
        }
    }

    private static string FormLoc(string key, int number) {
        return string.Format(Localizer.GetText(key), number);
    }
}
