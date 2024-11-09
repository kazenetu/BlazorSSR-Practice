using System;
using System.Collections.Generic;

namespace PdfReport.Interfaces
{
    /// <summary>
    /// データ用インターフェイス
    /// </summary>
    public interface IData
    {
        /// <summary>
        /// カラム番号によって文字列を返す
        /// </summary>
        /// <param name="colIndex">カラム番号</param>
        /// <returns>値と型のタプル</returns>
        (Type type,object value)  GetColumn(int colmunIndex);

        /// <summary>
        /// カラム番号によってサマリした文字列を返す
        /// </summary>
        /// <param name="dataList">データリスト</param>
        /// <param name="colIndex">カラム番号</param>
        /// <returns>値と型のタプル</returns>
        (Type type,object value) GetColumnTotal(List<IData> dataList, int colIndex);
    }
}