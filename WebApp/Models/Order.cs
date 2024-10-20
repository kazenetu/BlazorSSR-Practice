namespace WebApp.Models
{
    /// <summary>
    /// 注文情報
    /// </summary>
    /// <param name="No">番号(1〜)</param>
    /// <param name="ProductName">商品名</param>
    /// <param name="UnitPrice">単価</param>
    /// <param name="Qty">数量</param>
    /// <param name="TotalPrice">合計金額</param>
    public record Order(int No, string ProductName, decimal UnitPrice, decimal Qty, decimal TotalPrice);
}
