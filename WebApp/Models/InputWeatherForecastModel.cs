namespace WebApp.Models;

public class InputWeatherForecastModel
{
    public DateOnly? StartDate { set; get; }
    public DateOnly? EndDate { set; get; }
    public int? StartTemperatureC { set; get; }
    public int? EndTemperatureC { set; get; }
}
