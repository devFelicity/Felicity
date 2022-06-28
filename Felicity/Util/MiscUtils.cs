namespace Felicity.Util;

public static class MiscUtils
{
    public static double GetTimestamp(this DateTime dateTime)
    {
        return dateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
    }
}