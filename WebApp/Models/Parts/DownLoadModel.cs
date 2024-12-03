namespace WebApp.Models.Parts;

/// <summary>
/// ダウンロード用モデル
/// </summary>
/// <param name="FileName">ファイル名</param>
/// <param name="Data">データ</param>
/// <param name="ErrorMessage">エラーメッセージ</param>
public record DownLoadModel(string FileName, object? Data, string ErrorMessage = "");
