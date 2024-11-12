using PdfReport.Interfaces;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using PdfSharp.Quality;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PdfReport.Layout
{
    /// <summary>
    /// 注文帳票
    /// </summary>
    public class OrderLayout : ILayout
    {
        /// <summary>
        /// 帳票作成
        /// </summary>
        /// <param name="document">PdfDocumentインスタンス</param>
        /// <param name="items">データリスト</param>
        /// <returns>成功/失敗</returns>
        public bool Create(PdfDocument document, List<IData> items)
        {
            // Create an empty page in this document.
            var page = document.AddPage();

            //page.Size = PageSize.Letter;
            // Get an XGraphics object for drawing on this page.
            var gfx = XGraphics.FromPdfPage(page);

            // 日本語フォント設定
            var font =
                new XFont("Gen Shin Gothic",
                    20,
                    XFontStyleEx.BoldItalic,
                    new XPdfFontOptions(PdfFontEmbedding
                            .EmbedCompleteFontFile));

            // 枠線設定
            var pen = new XPen(XColors.Black, 3);

            // 矩形設定
            var width = page.Width.Point;
            var height = page.Height.Point;
            var rect = new XRect((width - 200) / 2, 10, 200, 40);

            // 矩形描画
            gfx.DrawRectangle (pen, rect);

            // 文字描画
            rect.X += 5;
            rect.Y += 5;
            rect.Height -= 5;
            rect.Width -= 5;
            var tf = new XTextFormatter(gfx);
            gfx
                .DrawString("注文書",
                font,
                XBrushes.Black,
                rect,
                XStringFormats.Center);

            // 表形式描画確認
            var singlePen = new XPen(XColors.Gray, 1);
            var headerBrush = XBrushes.Blue;

            // 日本語フォント設定
            var gridFont =
                new XFont("Gen Shin Gothic",
                    10,
                    XFontStyleEx.BoldItalic,
                    new XPdfFontOptions(PdfFontEmbedding
                            .EmbedCompleteFontFile));
            var headerNames =
                new List<string> { "No.", "製品名", "単価", "数量", "金額" };
            var headerWidth = new List<int> { 25, 250, 80, 50, 150 };

            var startX = (((int) page.Width.Value) - headerWidth.Sum()) / 2;
            var y = 100;
            var isDrawHeader = false;
            foreach (var order in items)
            {
                var addY = 0;

                // 描画開始Xを設定
                var x = startX;

                // ヘッダ
                if (!isDrawHeader)
                {
                    addY = 20;

                    // ヘッダ
                    for (int col = 0; col < headerWidth.Count; col++)
                    {
                        var gridRect = new XRect(x, y, headerWidth[col], 20);
                        gfx.DrawRectangle (singlePen, headerBrush, gridRect);
                        gfx
                            .DrawString($"{headerNames[col]}",
                            gridFont,
                            XBrushes.White,
                            gridRect,
                            XStringFormats.Center);
                        x += headerWidth[col];
                    }
                    y += addY;
                    isDrawHeader = true;

                    // Xの初期化
                    x = startX;
                }

                // データ
                addY = 20;
                for (int col = 0; col < headerWidth.Count; col++)
                {
                    var gridRect = new XRect(x, y, headerWidth[col], 20);
                    gfx.DrawRectangle (singlePen, gridRect);
                    gridRect.X += 5;
                    gridRect.Width -= 10;
                    gfx
                        .DrawString(GetString(order, col),
                        gridFont,
                        XBrushes.Black,
                        gridRect,
                        GetAlignment(col));
                    x += headerWidth[col];
                }
                y += addY;
            }

            // サマリ行
            var totalX = startX;
            var totalGridRect =
                new XRect(totalX, y, headerWidth[0] + headerWidth[1], 20);
            gfx.DrawRectangle (singlePen, totalGridRect);
            gfx
                .DrawString("合計",
                gridFont,
                XBrushes.Black,
                totalGridRect,
                XStringFormats.Center);
            totalX += headerWidth[0] + headerWidth[1];
            for (int col = 2; col < headerWidth.Count; col++)
            {
                var gridRect = new XRect(totalX, y, headerWidth[col], 20);
                gfx.DrawRectangle (singlePen, gridRect);
                gridRect.X += 5;
                gridRect.Width -= 10;
                gfx
                    .DrawString(GetTotalString(items, col),
                    gridFont,
                    XBrushes.Black,
                    gridRect,
                    GetAlignment(col));
                totalX += headerWidth[col];
            }

            return true;
        }

        /// <summary>
        /// カラム番号によって文字列を返す
        /// </summary>
        /// <param name="order">注文情報</param>
        /// <param name="colIndex">カラム番号</param>
        /// <returns>対象の文字列</returns>
        private string GetString(IData order, int colIndex)
        {
            var colmunItem = order.GetColumn(colIndex);

            switch(colIndex)
            {
                case 0:
                    return GetValue(colmunItem, string.Empty);
                case 2:
                    return GetValue(colmunItem, "C");
                case 3:
                    return GetValue(colmunItem, "#,#");
                case 4 :
                    return GetValue(colmunItem, "C");
                default:
                    return GetValue(colmunItem, string.Empty);
            };
        }

        /// <summary>
        /// カラム番号によって文字列を返す
        /// </summary>
        /// <param name="orders">注文情報リスト</param>
        /// <param name="colIndex">カラム番号</param>
        /// <returns>対象の文字列</returns>
        private string GetTotalString(List<IData> orders, int colIndex)
        {
            if(!orders.Any())
                return string.Empty;

            var colmunItem = orders[0].GetColumnTotal(orders, colIndex);

            switch(colIndex)
            {
                case 2:
                    return GetValue(colmunItem, "C");
                case 3:
                    return GetValue(colmunItem, "#,#");
                case 4 :
                    return GetValue(colmunItem, "C");
                default:
                        return string.Empty;
            };
        }

        /// <summary>
        /// 型情報ごとの文字列取得
        /// </summary>
        /// <param name="target">取得対象</param>
        /// <param name="param">ToStrigする際の書式文字列</param>
        /// <returns></returns>
        private string GetValue((Type type,object value) target,string param)
        {
            if(target.type == typeof(int))
                return ((int)target.value).ToString();
            if(target.type == typeof(decimal))
                return ((decimal)target.value).ToString(param);
            if(target.type == typeof(string))
                return ((string)target.value).ToString();

            return string.Empty;
        }

        /// <summary>
        /// カラム番号によってXStringFormatsを返す
        /// </summary>
        /// <param name="order">注文情報</param>
        /// <returns>XStringFormats</returns>
        private XStringFormat GetAlignment(int colIndex)
        {
            switch(colIndex)
            {
                case 0:
                    var no0 = XStringFormats.Center;
                    no0.Alignment = XStringAlignment.Far;
                    return no0;
                case 1:
                    var no1 = XStringFormats.Center;
                    no1.Alignment = XStringAlignment.Near;
                    return no1;
                case 2:
                    var no2 = XStringFormats.Center;
                    no2.Alignment = XStringAlignment.Far;
                    return no2;
                case 3:
                    var no3 = XStringFormats.Center;
                    no3.Alignment = XStringAlignment.Far;
                    return no3;
                case 4 :
                    var no4 = XStringFormats.Center;
                    no4.Alignment = XStringAlignment.Far;
                    return no4;
                default:
                    return XStringFormats.TopLeft;
            };
        }
    }
}
