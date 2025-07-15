namespace WebApp.Extentions;

public static class DateTimeExtensions
{
    /// <summary>
    /// 日本時間に変換
    /// </summary>
    /// <param name="utcTime">Utc時間</param>
    /// <returns>日本時間</returns>
    public static DateTime ToJstTime(this DateTime utcTime)
    {
        TimeZoneInfo tokyoTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Asia/Tokyo");
        DateTime dateTimeInTokyo = TimeZoneInfo.ConvertTime(utcTime, tokyoTimeZoneInfo);
        return dateTimeInTokyo;
    }
}