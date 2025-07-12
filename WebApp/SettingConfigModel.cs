namespace WebApp;

/// <summary>
/// 設定ファイル：設定情報
/// </summary>
public class SettingConfigModel
{
    /// <summary>
    /// すべてのユーザーをログイン必須とするか
    /// </summary>
    /// <remarks>デフォルトはtrue</remarks>
    public bool AllLogin { set; get; } = true;

    /// <summary>
    /// TOTP認証コードを実施するか
    /// </summary>
    public bool RequiredAuthentication { set; get; } = false;

    /// <summary>
    /// QRコード作成時のアプリケーション名
    /// </summary>
    public string QRAppName { set; get; } = "test";
}
