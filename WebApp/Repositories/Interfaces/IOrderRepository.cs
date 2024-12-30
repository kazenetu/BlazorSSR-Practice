using WebApp.Models;

namespace WebApp.Repositories.Interfaces;

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

    /// <summary>
    /// 注文キーリストを取得
    /// </summary>
    /// <returns>注文リスト</returns>
    List<OrderModel> GetOderKeyList();

    /// <summary>
    /// 注文情報を取得
    /// </summary>
    /// <param name="productName">製品名</param>
    /// <returns>注文情報</returns>
    OrderModel GetOder(string productName);

    /// <summary>
    /// レコード数を返す
    /// </summary>
    /// <param name="inputModel">検索条件</param>
    /// <returns>対象レコード数</returns>
    int GetTotalRecord(InputOrderModel inputModel);

    /// <summary>
    /// 対象レコード配列を返す
    /// </summary>
    /// <param name="inputModel">検索条件</param>
    /// <param name="startIndex">取得開始レコードインデックス</param>
    /// <param name="recordCount">取得レコード数</param>
    /// <returns>指定範囲のレコード配列</returns>
    OrderModel[] GetList(InputOrderModel inputModel, int startIndex, int recordCount);

    /// <summary>
    /// 注文情報の登録
    /// </summary>
    /// <param name="target">登録対象</param>
    /// <param name="userId">ユーザーID</param>
    /// <param name="programId">プログラムID</param>
    /// <returns>成功/失敗</returns>
    bool Save(OrderModel target, string userId, string programId);

    /// <summary>
    /// 注文情報リストの登録(バルクインサート)
    /// </summary>
    /// <param name="targets">登録対象リスト</param>
    /// <param name="userId">ユーザーID</param>
    /// <param name="programId">プログラムID</param>
    /// <returns>成功/失敗</returns>
    bool Save(IReadOnlyList<OrderModel> targets, string userId, string programId);
}
