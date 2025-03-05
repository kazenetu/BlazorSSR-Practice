namespace WebApp.Models;

public class InputOrderModel
{
    public string? ProductName { set; get; }
    public decimal? StartUnitPrice { set; get; }
    public decimal? EndUnitPrice { set; get; }
    public decimal? StartTotalPrice { set; get; }
    public decimal? EndTotalPrice { set; get; }

    /// <summary>
    /// 複製
    /// </summary>
    /// <returns>複製オブジェクト</returns>
    public InputOrderModel Copy()
    {
        return (InputOrderModel)this.MemberwiseClone();
    }
}
