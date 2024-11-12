using System.Collections.Generic;
using PdfSharp.Pdf;

namespace PdfReport.Interfaces
{
    /// <summary>
    /// 帳票用インターフェイス
    /// </summary>
    public interface ILayout
    {
        /// <summary>
        /// 帳票作成
        /// </summary>
        /// <param name="document">PdfDocumentインスタンス</param>
        /// <param name="items">データリスト</param>
        /// <returns>成功/失敗</returns>
        bool Create(PdfDocument document, List<IData> items);
    }
}