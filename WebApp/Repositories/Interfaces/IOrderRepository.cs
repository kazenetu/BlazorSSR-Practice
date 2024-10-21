using WebApp.Models;

namespace WebApp.Repositories.Interfaces
{
  /// <summary>
  /// 注文用インターフェース
  /// </summary>
  public interface IOrderRepository : IRepositoryBase
  {
    /// <summary>
    /// 注文リストを取得
    /// </summary>
    /// <returns>注文リスト</returns>
    List<OrderModel> GetOderList();
  }
}
