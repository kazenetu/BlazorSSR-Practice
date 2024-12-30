namespace WebApp.Models;

public class InputOrderModel
{
    public string? ProductName { set; get; }
    public decimal? StartUnitPrice { set; get; }
    public decimal? EndUnitPrice { set; get; }
    public decimal? StartTotalPrice { set; get; }
    public decimal? EndTotalPrice { set; get; }
}
