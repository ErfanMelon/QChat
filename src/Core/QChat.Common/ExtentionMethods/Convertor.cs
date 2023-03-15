namespace QChat.Common.ExtentionMethods;

public static class Convertor
{
    public static Guid ToGuid(this string? guid)
    {
        if (Guid.TryParse(guid, out Guid result))
            return result;
        return Guid.Empty;
    }
    public static string ToAproximateDate(this DateTime datetime)
    {
        var dt = DateTime.Now;

        string result = string.Empty;
        var timeSpan = dt.Subtract(datetime);

        if (timeSpan <= TimeSpan.FromSeconds(60))
            result = $"{timeSpan.Seconds} ثانیه قبل ";

        else if (timeSpan <= TimeSpan.FromMinutes(60))
            result = timeSpan.Minutes > 1 ? $"{timeSpan.Minutes} دقیقه قبل " : "دقایقی قبل";

        else if (timeSpan <= TimeSpan.FromHours(24))
            result = timeSpan.Hours > 1 ? $"{timeSpan.Hours} ساعت قبل " : "ساعتی پیش";

        else if (timeSpan <= TimeSpan.FromDays(30))
            result = timeSpan.Days > 1 ? $"{timeSpan.Days} روز قبل " : "دیروز";

        else if (timeSpan <= TimeSpan.FromDays(365))
            result = timeSpan.Days > 30 ? $"{timeSpan.Days / 30} ماه قبل " : "ماه قبل";

        else
            result = timeSpan.Days > 365 ? $"{timeSpan.Days / 365} سال قبل " : "سال پیش";

        return result;
    }
}
