namespace WebApp
{
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
  }
}