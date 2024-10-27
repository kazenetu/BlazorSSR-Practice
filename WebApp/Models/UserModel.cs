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
    public record UserModel(string ID, string Password, string Salt, string Fullname, bool AdminRole, bool Disabled=true, int Version=1);
}
