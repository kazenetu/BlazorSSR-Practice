using PdfReport.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PdfReport.DataLists
{
    /// <summary>
    /// 封筒帳票用データ
    /// </summary>
    public class Envelope : IData
    {
        /// <summary>
        /// 郵便番号
        /// </summary>
        private string PostNo;

        /// <summary>
        /// 住所
        /// </summary>
        private string Address;

        /// <summary>
        /// 宛名
        /// </summary>
        private string AddressName;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="postNo">郵便番号</param>
        /// <param name="address">住所</param>
        /// <param name="addressName">宛先</param>
        public Envelope(string postNo, string address, string addressName)
        {
            PostNo = postNo;
            Address = address;
            AddressName = addressName;
        }

        /// <summary>
        /// カラム番号によって文字列を返す
        /// </summary>
        /// <param name="colIndex">カラム番号</param>
        /// <returns>対象の文字列</returns>
        public (Type type, object value) GetColumn(int colIndex)
        {
            switch (colIndex)
            {
                case 0:
                    return GetColumnResult(PostNo);
                case 1:
                    return GetColumnResult(Address);
                case 2:
                    return GetColumnResult(AddressName);
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
            var envelopes = dataList.Select(data => (Envelope)data);
            switch (colIndex)
            {
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
