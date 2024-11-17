using Microsoft.AspNetCore.Components.Forms;
using System.Text;

namespace WebApp.Models.Parts
{
    /// <summary>
    /// ファイルアップロード用Model
    /// </summary>
    public class UpLoadeModel
    {
        /// <summary>
        /// ブラウザから取得したファイル情報
        /// </summary>
        private IBrowserFile? _formFile;

        /// <summary>
        /// ファイル情報がnullか否か
        /// </summary>
        public bool IsNull => _formFile is null;

        /// <summary>
        /// ContentType取得
        /// </summary>
        public string ContentType => _formFile?.ContentType??string.Empty;

        /// <summary>
        /// ファイルサイズe取得
        /// </summary>
        public long FileSize => _formFile?.Size??0;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="formFile">ブラウザから取得したファイル情報</param>
        public UpLoadeModel(IBrowserFile? formFile)
        {
            _formFile = formFile;
        }

        /// <summary>
        /// 1行読み込み
        /// </summary>
        /// <returns>文字列</returns>
        public async IAsyncEnumerable<string> ReadLineAsync()
        {
            // ブラウザ取得情報がなければ終了
            if(_formFile is null) yield break;

            // ファイル内容の取得
            using (var reader = new StreamReader(_formFile!.OpenReadStream()))
            {
                while (true)
                {
                    string? result = await reader.ReadLineAsync();

                    // ファイル終了であれば終了
                    if(result is null) yield break;

                    // 一行を返す
                    yield return result;
                }
            }
        }
    }
}
