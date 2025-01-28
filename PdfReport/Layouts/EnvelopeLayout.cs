using PdfReport.Interfaces;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
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
        /// 郵便の文字数
        /// </summary>
        private const int PostNoCount = 7;

        /// <summary>
        /// 住所の最大文字数
        /// </summary>
        private const int AddressMaxCount = 24;

        /// <summary>
        /// 宛先の最大文字数
        /// </summary>
        private const int AddressNameMaxCount = 10;

        /// <summary>
        /// 漢数字リスト
        /// </summary>
        private static string[] KanSujiList = { "〇", "一", "二", "三", "四", "五", "六", "七", "八", "九" };

        /// <summary>
        /// フォント：郵便番号
        /// </summary> <summary>
        private XFont? FontPostNo;

        /// <summary>
        /// フォント：住所
        /// </summary> <summary>
        private XFont? FontAddress;

        /// <summary>
        /// フォント：宛先
        /// </summary> <summary>
        private XFont? FontAddressName;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EnvelopeLayout()
        {
            // 日本語フォント設定
            FontPostNo =
                new XFont("Gen Shin Gothic",
                    25,
                    XFontStyleEx.BoldItalic,
                    new XPdfFontOptions(PdfFontEmbedding
                            .EmbedCompleteFontFile));
            FontAddress =
                new XFont("Gen Shin Gothic",
                    15,
                    XFontStyleEx.BoldItalic,
                    new XPdfFontOptions(PdfFontEmbedding
                            .EmbedCompleteFontFile));
            FontAddressName =
                new XFont("Gen Shin Gothic",
                    30,
                    XFontStyleEx.BoldItalic,
                    new XPdfFontOptions(PdfFontEmbedding
                            .EmbedCompleteFontFile));
        }

        /// <summary>
        /// 帳票作成
        /// </summary>
        /// <param name="document">PdfDocumentインスタンス</param>
        /// <param name="items">データリスト</param>
        /// <returns>成功/失敗</returns>
        public bool Create(PdfDocument document, List<IData> items)
        {
            var result = false;

            // ページ単位に描画
            foreach (var item in items)
            {
                result = CreatePage(document, item);
                if(!result) break;
            }

            return result;
        }

        /// <summary>
        ///  ページ作成
        /// </summary>
        /// <param name="document">PdfDocumentインスタンス</param>
        /// <param name="item">データ</param>
        /// <returns>成功/失敗</returns>
        private bool CreatePage(PdfDocument document, IData item)
        {
            //　チェック：郵便番号の文字数が規定数以外はエラー
            var postNo = item.GetColumn(0).value.ToString().Replace("-", string.Empty);
            if (postNo.Length != PostNoCount) return false;

            //　チェック：住所の文字数が最大数より大きい場合はエラー
            var address = GetVerticalWriting(item.GetColumn(1).value.ToString());
            if (address.Replace(Environment.NewLine, string.Empty).Length > AddressMaxCount) return false;

            //　チェック：宛名の文字数が最大数より大きい場合はエラー
            var addressName = GetVerticalWriting(item.GetColumn(2).value.ToString());
            if (addressName.Replace(Environment.NewLine, string.Empty).Length > AddressNameMaxCount) return false;

            // Create an empty page in this document.
            var page = document.AddPage();

            // 長型3号
            //page.Orientation = PdfSharp.PageOrientation.Landscape;
            page.Height = XUnit.FromPoint(666.14184);
            page.Width = XUnit.FromPoint(340.15752);

            // Get an XGraphics object for drawing on this page.
            var gfx = XGraphics.FromPdfPage(page);

            // 縦書き用インスタンス取得
            var tf = new XTextFormatter(gfx);

            // 文字描画：郵便番号
            var rectPostNo = new XRect(180, 27, 300 + 100, page.Height.Point);
            DrawPostNo(gfx, FontPostNo!, rectPostNo, postNo);

            // 文字描画：住所
            var rectAddress = new XRect(page.Width.Point - 40, 100, page.Width.Point, page.Height.Point);
            tf
                .DrawString(address,
                FontAddress!,
                XBrushes.Black,
                rectAddress,
                XStringFormats.TopLeft);

            // 文字描画：宛名
            var rectAddressName = new XRect(160 - 15, 100, 160 + 100, page.Height.Point);
            tf
                .DrawString(addressName + "様",
                FontAddressName!,
                XBrushes.Black,
                rectAddressName,
                XStringFormats.TopLeft);

            return true;
        }

        /// <summary>
        /// 郵便番号の描画
        /// </summary>
        /// <param name="gfx">描画用インスタンス</param>
        /// <param name="fontPostNo">郵便番号用フォント</param>
        /// <param name="rect">対象範囲</param>
        /// <param name="yubinNo">郵便番号文字列</param>
        private void DrawPostNo(XGraphics gfx, XFont fontPostNo, XRect rect, string yubinNo)
        {
            var numberIndex = 0;
            foreach (var number in yubinNo)
            {
                // 左位置の設定
                var left = rect.Left + 20 * numberIndex;

                // ４桁目以降は3桁と4桁の間を開ける
                if (numberIndex > 2)
                    left += 3;

                // 郵便番号の1文字を描画
                gfx.DrawString(number.ToString(),
                fontPostNo,
                XBrushes.Black,
                new XRect(left, rect.Top, left + 50, rect.Height),
                XStringFormats.TopLeft);

                // 郵便番号の桁数を進める
                numberIndex++;
            }
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
                if (Regex.IsMatch(text, @"^[0-9]+$"))
                {
                    // 数字は漢数字に変換
                    text = KanSujiList[int.Parse(text)];
                }
                if (text == "-")
                {
                    text = "｜";
                }
                if (text == "ー")
                {
                    text = "｜";
                }
                result += $"{text}{Environment.NewLine}";

                wordIndex++;
            }
            return result;
        }
    }
}
