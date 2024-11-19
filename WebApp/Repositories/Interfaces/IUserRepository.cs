using WebApp.Models;

namespace WebApp.Repositories.Interfaces
{
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
        public UserModel? GetUser(string userID);


        /// <summary>
        /// パスワードチェック
        /// </summary>
        /// <param name="userID">ユーザーID</param>
        /// <param name="password">パスワード</param>
        /// <returns>パスワード一致か否か</returns>
        public bool EqalsPassword(string userID, string password);

        /// <summary>
        /// パスワード作成
        /// </summary>
        /// <param name="password">パスワード</param>
        /// <returns>暗号化済パスワードとソルトのタプル</returns>
        public (string encryptedPassword, string salt) CreateEncryptedPassword(string password);
    }
}
