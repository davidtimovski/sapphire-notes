using System;

namespace SapphireNotes.Utils
{
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
                if (secDiff < 60)
                {
                    return "just now";
                }

                if (secDiff < 120)
                {
                    return "1 minute ago";
                }

                if (secDiff < (60 * 60))
                {
                    return string.Format("{0} minutes ago", Math.Floor((double)secDiff / 60));
                }

                if (secDiff < (2 * 60 * 60))
                {
                    return "1 hour ago";
                }

                if (secDiff < (24 * 60 * 60))
                {
                    return string.Format("{0} hours ago", Math.Floor((double)secDiff / 3600));
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
                return string.Format("{0} weeks ago", Math.Ceiling((double)dayDiff / 7));
            }

            return dateTime.ToString();
        }
    }
}
