using PdfReport.Interfaces;
using PdfReport.Layout;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PdfReport
{
    /// <summary>
    /// レイアウト種別
    /// </summary>
    public enum LayoutKinds
    {
        None,
        Order,
    }

    /// <summary>
    /// PdfReportyエントリクラス
    /// </summary>
    public class PdfMain
    {
        /// <summary>
        /// 帳票レイアウト
        /// </summary>
        private ILayout? _layout = null;

        /// <summary>
        /// 出力用データリスト
        /// </summary>
        private List<IData>? _dataList = null;

        /// <summary>
        /// PDFsharpドキュメントインスタンス
        /// </summary>
        private PdfDocument? _document = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="layoutKind">帳票レイアウト種別</param>
        /// <param name="dataList">出力用データリスト</param>
        public PdfMain(LayoutKinds layoutKind, List<IData> dataList)
        {
            switch (layoutKind)
            {
                case LayoutKinds.Order:
                    SetParams(new OrderLayout(), dataList);
                    break;

                default:
                    throw new Exception("帳票レイアウトが指定されていません");
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="layout">帳票レイアウト</param>
        /// <param name="dataList">出力用データリスト</param>
        public PdfMain(ILayout layout, List<IData> dataList)
        {
            SetParams(layout, dataList);
        }

        /// <summary>
        /// パラメータ「Stream」にPDF書き込み
        /// </summary>
        /// <param name="stream">出力用Stream</param>
        public void Create(Stream stream)
        {
            if (_layout is null)
                throw new Exception("帳票レイアウトが指定されていません。");

            if (_dataList is null || !_dataList.Any())
                throw new Exception("出力用データリストが指定されていません。");

            if (_document is null)
                throw new Exception("ドキュメントが生成されていません。");

            //帳票レイアウト書き込み
            if (!_layout.Create(_document, _dataList))
                throw new Exception("帳票レイアウト失敗");

            // 出力用Streamに出力
            _document.Save(stream);
        }

        /// <summary>
        /// パラメータ設定
        /// </summary>
        /// <param name="layout">帳票レイアウト</param>
        /// <param name="dataList">出力用データリスト</param>
        private void SetParams(ILayout? layout, List<IData> dataList)
        {
            _layout = layout;
            _dataList = dataList;

            _document = new PdfDocument();
        }
    }
}
