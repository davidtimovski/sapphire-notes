using System;
using System.Globalization;

namespace SapphireNotes.Utils;

public static class DateTimeUtil
{
    public static string GetRelativeDate(DateTime dateTime)
    {
        var timeSpan = DateTime.Now.Subtract(dateTime);
        var dayDiff = (int)timeSpan.TotalDays;
        var secDiff = (int)timeSpan.TotalSeconds;
        
        switch (dayDiff)
        {
            case < 0:
                throw new ArgumentException("DateTime argument should be in the past.");
            case 0:
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

                break;
            case 1:
                return "yesterday";
        }

        return dayDiff switch
        {
            < 7 => $"{dayDiff} days ago",
            < 31 => $"{Math.Ceiling((double) dayDiff / 7)} weeks ago",
            _ => dateTime.ToString(dateTime.Year == DateTime.Now.Year 
                    ? "MMM-dd HH:mm" 
                    : "yyyy-MMM-dd HH:mm",
                CultureInfo.InvariantCulture)
        };
    }
}
