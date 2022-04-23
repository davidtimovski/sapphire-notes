using System;
using System.Globalization;

namespace SapphireNotes.Utils;

public static class DateTimeUtil
{
    public static string GetRelativeDate(DateTime dateTime)
    {
        TimeSpan timeSpan = DateTime.Now.Subtract(dateTime);

        int dayDiff = (int)timeSpan.TotalDays;
        int secDiff = (int)timeSpan.TotalSeconds;

        if (dayDiff < 0)
        {
            throw new ArgumentException("DateTime argument should be in the past.");
        }

        if (dayDiff == 0)
        {
            switch (secDiff)
            {
                case < 60:
                    return "just now";
                case < 120:
                    return "1 minute ago";
                case < 60 * 60:
                    return $"{Math.Floor((double) secDiff / 60)} minutes ago";
                case < 2 * 60 * 60:
                    return "1 hour ago";
                case < 24 * 60 * 60:
                    return $"{Math.Floor((double) secDiff / 3600)} hours ago";
            }
        }

        if (dayDiff == 1)
        {
            return "yesterday";
        }

        if (dayDiff < 7)
        {
            return $"{dayDiff} days ago";
        }

        if (dayDiff < 31)
        {
            return $"{Math.Ceiling((double) dayDiff / 7)} weeks ago";
        }

        if (dateTime.Year == DateTime.Now.Year)
        {
            return dateTime.ToString("MMM-dd HH:mm", CultureInfo.InvariantCulture);
        }

        return dateTime.ToString("yyyy-MMM-dd HH:mm", CultureInfo.InvariantCulture);
    }
}
