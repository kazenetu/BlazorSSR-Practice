namespace WebApp.Models;

public class WeatherForecastModel
{
    /// <summary>
    /// 天気予報モデル
    /// </summary>
    /// <param name="targetDate">対象日付</param>
    /// <param name="temperatureC">温度(摂氏)</param>
    /// <param name="summary">天気概要</param>
    public WeatherForecastModel(DateOnly targetDate, int temperatureC, string? summary)
    {
        TargetDate = targetDate;
        TemperatureC = temperatureC;
        Summary = summary;
    }

    /// <summary>
    /// 対象日付
    /// </summary>
    public DateOnly TargetDate { get; set; }

    /// <summary>
    /// 温度(摂氏)
    /// </summary>
    public int TemperatureC { get; set; }

    /// <summary>
    /// 天気概要
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// 温度(華氏)
    /// </summary>
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
