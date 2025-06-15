using WebApp.Models;

namespace WebApp.Repositories.Interfaces;

/// <summary>
/// ユーザー用インターフェース
/// </summary>
public interface IUserRepository : IRepositoryBase
{
    /// <summary>
    /// ユーザーを取得
    /// </summary>
    /// <param name="userID">ユーザーID</param>
    /// <returns>ユーザー</returns>
    UserModel? GetUser(string userID);

    /// <summary>
    /// ユーザーの保存
    /// </summary>
    /// <param name="target">登録対象</param>
    /// <param name="userId">ユーザーID</param>
    /// <param name="programId">プログラムID</param>
    /// <returns>成功/失敗</returns>
    /// <remarks>パスワードは平文を設定すること。Saltは未設定</remarks>
    bool Save(UserModel target, string userId, string programId);

    /// <summary>
    /// シークレット文字列設定
    /// </summary>
    /// <param name="target">登録対象</param>
    /// <param name="userId">ユーザーID</param>
    /// <param name="programId">プログラムID</param>
    /// <returns>成功/失敗</returns>
    bool SaveSecrets(UserModel target, string userId, string programId);

    /// <summary>
    /// パスワードチェック
    /// </summary>
    /// <param name="userID">ユーザーID</param>
    /// <param name="password">パスワード</param>
    /// <returns>パスワード一致か否か</returns>
    bool EqalsPassword(string userID, string password);

    /// <summary>
    /// パスワード作成
    /// </summary>
    /// <param name="password">パスワード</param>
    /// <returns>暗号化済パスワードとソルトのタプル</returns>
    (string encryptedPassword, string salt) CreateEncryptedPassword(string password);
}
