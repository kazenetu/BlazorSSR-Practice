namespace WebApp.Models
{
    public class OrderModel
    {
        /// <summary>
        /// 番号(1〜)
        /// </summary>
        public int No { set; get; }

        /// <summary>
        /// 商品名
        /// </summary>
        public string ProductName { set; get; }

        /// <summary>
        /// 単価
        /// </summary>
        public decimal UnitPrice { set; get; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal Qty { set; get; }

        /// <summary>
        /// 合計金額
        /// </summary>
        public decimal TotalPrice { set; get; }

        /// <summary>
        /// バージョン番号(楽観ロック用)
        /// </summary>
        public int Version { set; get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="no">番号(1〜)</param>
        /// <param name="productName">商品名</param>
        /// <param name="unitPrice">単価</param>
        /// <param name="qty">数量</param>
        /// <param name="totalPrice">合計金額</param>
        /// <param name="version">バージョン番号(楽観ロック用)</param>
        public OrderModel(int no, string productName, decimal unitPrice, decimal qty, decimal totalPrice, int version = 0)
        {
            No = no;
            ProductName = productName;
            UnitPrice = unitPrice;
            Qty = qty;
            TotalPrice = totalPrice;
            Version = version;
        }
    }
}
