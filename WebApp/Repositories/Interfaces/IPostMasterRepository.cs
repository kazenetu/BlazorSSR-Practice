using WebApp.Models;

namespace WebApp.Repositories.Interfaces;

/// <summary>
/// 郵便番号・住所用インターフェース
/// </summary>
public interface IPostMasterRepository : IRepositoryBase
{
    /// <summary>
    /// 郵便番号検索
    /// </summary>
    /// <param name="postCd">郵便番号</param>
    /// <returns>郵便番号・住所 クラス</returns>
    /// <remarks>住所を設定</remarks>
    PostMasterModel SearchPostCd(string postCd);

    /// <summary>
    /// 住所検索
    /// </summary>
    /// <param name="address">住所文字列</param>
    /// <returns>郵便番号・住所 クラス</returns>
    /// <remarks>郵便番号を設定</remarks>
    PostMasterModel SearchAddress(string address);

    /// <summary>
    /// 住所カナ取得
    /// </summary>
    /// <param name="address">住所文字列</param>
    /// <returns>住所カナ文字列</returns>
    string GetAddressKana(string address);
}
