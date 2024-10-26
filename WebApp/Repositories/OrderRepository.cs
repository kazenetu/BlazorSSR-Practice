using Microsoft.Extensions.Options;
using System.Text;
using System.Data;
using WebApp.DBAccess;
using WebApp.Repositories.Interfaces;
using WebApp.Models;

namespace WebApp.Repositories
{
  /// <summary>
  /// 注文用リポジトリ
  /// </summary>
  public class OrderRepository : RepositoryBase, IOrderRepository
  {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public OrderRepository(IOptions<DatabaseConfigModel> config) : base(config)
    {
    }

    #region 取得

    /// <summary>
    /// 注文リストを取得
    /// </summary>
    /// <returns>注文リスト</returns>
    public List<OrderModel> GetOderList()
    {
      return Find(null);
    }

    /// <summary>
    /// 注文リストを取得：メイン部分
    /// </summary>
    /// <param name="productName">商品名</param>
    /// <returns>注文リスト</returns>
    /// <remarks>メイン処理</remarks>
    private List<OrderModel> Find(string? productName)
    {
      var result = new List<OrderModel>();

      // パラメータ初期化
      db!.ClearParam();

      var sql = new StringBuilder();

      sql.AppendLine("select ROW_NUMBER() OVER(ORDER BY productName) AS NoId,* from t_order" );

      // 商品名指定
      if(productName is not null)
      {
        sql.AppendLine("WHERE");
        sql.AppendLine("  productName = @product_name");

        // Param設定
        db.AddParam("@product_name", productName);
      }
      sql.AppendLine("ORDER BY productName");

      var sqlResult = db.Fill(sql.ToString());
      foreach (DataRow row in sqlResult.Rows)
      {
        // 注文
        var no = Parse<int>(row["NoId"]);
        var product = Parse<string>(row["productName"]);
        var unitPrice = Parse<decimal>(row["unitPrice"]);
        var qty = Parse<decimal>(row["qty"]);
        var totalPrice = unitPrice * qty;
        result.Add(new OrderModel(no, product, unitPrice, qty, totalPrice));
      }
      return result;
    }

    #endregion

  }
}
