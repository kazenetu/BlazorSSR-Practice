using PdfSharp.Pdf;
using PdfSharp.Quality;

namespace PdfReport.Utilites
{
    /// <summary>
    /// ユーティリティクラス
    /// </summary>
    public class Utility
    {
        /// <summary>
        /// 日本語フォントリゾルバーのグローバル登録
        /// </summary>
        public static void SetJapaneseFontResolver()
        {
            PdfSharp.Fonts.GlobalFontSettings.FontResolver = new JapaneseFontResolver();
        }

        /// <summary>
        /// ファイル保存用フルパスを返す
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetFullPath(string filename)
        {
            return PdfFileUtility.GetTempPdfFullFileName(filename);
        }
    }
}