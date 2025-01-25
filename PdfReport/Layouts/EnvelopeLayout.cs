using PdfReport.Interfaces;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using PdfSharp.Snippets;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PdfReport.Layouts
{
    /// <summary>
    /// 注文帳票
    /// </summary>
    public class EnvelopeLayout : ILayout
    {
        /// <summary>
        /// 帳票作成
        /// </summary>
        /// <param name="document">PdfDocumentインスタンス</param>
        /// <param name="items">データリスト</param>
        /// <returns>成功/失敗</returns>
        public bool Create(PdfDocument document, List<IData> items)
        {
            // ページ単位に描画
            foreach (var item in items)
            {
                CreatePage(document, item);
                break;
            }

            return true;
        }

        /// <summary>
        ///  ページ作成
        /// </summary>
        /// <param name="document">PdfDocumentインスタンス</param>
        /// <param name="item">データ</param>
        /// <returns>成功/失敗</returns>
        private bool CreatePage(PdfDocument document, IData item)
        {
            // Create an empty page in this document.
            var page = document.AddPage();

            // 長型3号
            //page.Orientation = PdfSharp.PageOrientation.Landscape;
            page.Height = XUnit.FromPoint(666.14184);
            page.Width = XUnit.FromPoint(340.15752);

            // Get an XGraphics object for drawing on this page.
            var gfx = XGraphics.FromPdfPage(page);

            // 日本語フォント設定
            var fontPostNo =
                new XFont("Gen Shin Gothic",
                    25,
                    XFontStyleEx.BoldItalic,
                    new XPdfFontOptions(PdfFontEmbedding
                            .EmbedCompleteFontFile));
            var fontAddress =
                new XFont("Gen Shin Gothic",
                    15,
                    XFontStyleEx.BoldItalic,
                    new XPdfFontOptions(PdfFontEmbedding
                            .EmbedCompleteFontFile));
            var fontAddressName =
                new XFont("Gen Shin Gothic",
                    30,
                    XFontStyleEx.BoldItalic,
                    new XPdfFontOptions(PdfFontEmbedding
                            .EmbedCompleteFontFile));

            // 縦書き用インスタンス取得
            var tf = new XTextFormatter(gfx);

            // 文字描画：郵便番号
            var rect = new XRect(230, 20, 300+100, page.Height.Point);
            tf
                .DrawString(item.GetColumn(0).value.ToString(),
                fontPostNo,
                XBrushes.Black,
                rect,
                XStringFormats.TopLeft);

            // 文字描画：住所
            rect = new XRect(page.Width.Point-80, 100, page.Width.Point, page.Height.Point);
            tf
                .DrawString(GetVerticalWriting(item.GetColumn(1).value.ToString()),
                fontAddress,
                XBrushes.Black,
                rect,
                XStringFormats.TopLeft);

            // 文字描画：宛名
            rect = new XRect(160-15, 100, 160+100, page.Height.Point);
            tf
                .DrawString(GetVerticalWriting(item.GetColumn(2).value.ToString() + "様"),
                fontAddressName,
                XBrushes.Black,
                rect,
                XStringFormats.TopLeft);


            return true;
        }

        /// <summary>
        /// 縦書き
        /// </summary>
        /// <param name="src">対象文字列</param>
        /// <returns>縦書き文字列</returns>
        private string GetVerticalWriting(string src)
        {
            var result = string.Empty;

            var wordIndex = 0;
            foreach (var word in src)
            {
                var text = word.ToString();
                var newLine = Environment.NewLine;
                var space = " ";
                if (Regex.IsMatch(text, @"^[0-9]+$"))
                {
                    if (wordIndex < src.Length-1 && Regex.IsMatch(src[wordIndex+1].ToString(), @"^[0-9]+$"))
                    {
                        newLine = string.Empty;
                    }
                }
                if (text == "-")
                {
                    text = "|";
                }
                if (text == "ー")
                {
                    text = "|";
                }
                result += $"{space}{text}{newLine}";

                wordIndex++;
            }
            return result;
        }
    }
}
