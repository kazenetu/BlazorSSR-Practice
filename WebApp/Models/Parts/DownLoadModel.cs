namespace WebApp.Models.Parts;

/// <summary>
/// ダウンロード用モデル
/// </summary>
public class DownLoadModel
{
    /// <summary>
    /// ダウンロード用モデル
    /// </summary>
    /// <param name="fileName">ファイル名</param>
    /// <param name="data">データ</param>
    /// <param name="errorMessage">エラーメッセージ</param>
    public DownLoadModel(string fileName, object? data, string errorMessage = "")
    {
        FileName = fileName;
        Data = data;
        ErrorMessage = errorMessage;

        // エラーメッセージがない場合のみチェック
        if (string.IsNullOrEmpty(errorMessage))
        {
            // ファイル名がない場合は例外エラー
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            if (string.IsNullOrEmpty(fileNameWithoutExtension))
                    throw new Exception("ファイル名が指定されていません。");
            }
        }

    /// <summary>
    /// ファイル名
    /// </summary>
    public string FileName { get; init; }

    /// <summary>
    /// データ
    /// </summary>
    public object? Data { get; init; }

    /// <summary>
    /// エラーメッセージ
    /// </summary>
    public string ErrorMessage { get; init; }
}
