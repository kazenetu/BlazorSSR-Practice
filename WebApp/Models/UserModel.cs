namespace WebApp.Models
{
    /// <summary>
    /// ユーザー情報
    /// </summary>
    /// <param name="ID">ユーザーID</param>
    /// <param name="Password">パスワード</param>
    /// <param name="Salt">ソルト</param>
    /// <param name="Fullname">ユーザー名称</param>
    /// <param name="AdminRole">管理者権限</param>
    /// <param name="Disabled">無効</param>
    /// <param name="Version">更新バージョン</param>
    public record UserModel(string ID, string Password, string Salt, string Fullname, bool AdminRole, bool Disabled = true, int Version = 1)
    {
        /// <summary>
        /// セッション名：ユーザー名称
        /// </summary>
        public const string SessionFullName = "SessionFullName";

        /// <summary>
        /// セッション名：管理者権限
        /// </summary>
        public const string SessionAdminRole = "SessionAdminRole";

        /// <summary>
        /// セッション名：ログイン済
        /// </summary>
        public const string SessionLoggedIn = "SessionLoggedIn";

        /// <summary>
        /// セッション名：ユーザー情報
        /// </summary>
        public const string SessionUser = "SessionUser";
    }
}
