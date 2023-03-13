namespace QChat.Common.ExtentionMethods;

public static class Convertor
{
    public static Guid ToGuid(this string? guid)
    {
        if (Guid.TryParse(guid, out Guid result))
            return result;
        return Guid.Empty;
    }
}
