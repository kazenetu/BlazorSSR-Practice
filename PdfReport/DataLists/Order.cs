using PdfReport.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PdfReport.DataLists
{
    /// <summary>
    /// 注文帳票用データ
    /// </summary>
    public class Order :IData
    {
        /// <summary>
        /// 番号(1〜)
        /// </summary>
        private int _no;

        /// <summary>
        /// 商品名
        /// </summary>
        private string _productName;

        /// <summary>
        /// 単価
        /// </summary>
        private decimal _unitPrice;

        /// <summary>
        /// 数量
        /// </summary>
        private decimal _qty;

        /// <summary>
        /// 合計金額
        /// </summary>
        private decimal _totalPrice;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="no">番号(1〜)</param>
        /// <param name="productName">商品名</param>
        /// <param name="unitPrice">単価</param>
        /// <param name="qty">数量</param>
        public Order(int no, string productName, decimal unitPrice, decimal qty)
        {
            _no = no;
            _productName = productName;
            _qty = qty;
            _unitPrice = unitPrice;
            _totalPrice = _unitPrice * _qty;
        }

        /// <summary>
        /// カラム番号によって文字列を返す
        /// </summary>
        /// <param name="colIndex">カラム番号</param>
        /// <returns>対象の文字列</returns>
        public (Type type, object value) GetColumn(int colIndex)
        {
            switch(colIndex)
            {
                case 0:
                    return GetColumnResult(_no);
                case 1:
                    return GetColumnResult(_productName);
                case 2:
                    return GetColumnResult(_unitPrice);
                case 3:
                    return GetColumnResult(_qty);
                case 4 :
                    return GetColumnResult(_totalPrice);
                default:
                    return (typeof(string), string.Empty);
            };
        }

        /// <summary>
        /// カラム番号によってサマリした文字列を返す
        /// </summary>
        /// <param name="dataList">データリスト</param>
        /// <param name="colIndex">カラム番号</param>
        /// <returns>対象の文字列</returns>
        public (Type type, object value) GetColumnTotal(List<IData> dataList, int colIndex)
        {
            var orders = dataList.Select(data=>(Order)data);
            switch(colIndex)
            {
                case 2:
                    return GetColumnResult(orders.Sum((item) => item._unitPrice));
                case 3:
                    return GetColumnResult(orders.Sum((item) => item._qty));
                case 4 :
                    return GetColumnResult(orders.Sum((item) => item._totalPrice));
                default:
                    return (typeof(string), string.Empty);
            };
        }

        /// <summary>
        /// IDataとして返すタプルを返す
        /// </summary>
        /// <param name="target">対象フィールド</param>
        /// <returns>Typeと値のタプル</returns>
        private (Type type, object value) GetColumnResult(object target)
        {
            return (target.GetType(), target);
        }
    }
}
