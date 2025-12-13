using System.Data;
using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;

namespace DB;

public class SQLiteDB : IDB
{
    /// <summary>
    /// スキーマDataTableを返す
    /// </summary>
    /// <param name="connectionString">接続文字列</param>
    /// <param name="tableName">指定テーブル名</param>
    /// <returns>スキーマDataTable</returns>
    public DataTable GetSchema(string connectionString, string tableName)
    {
        throw new NotImplementedException();
    }
   }
}