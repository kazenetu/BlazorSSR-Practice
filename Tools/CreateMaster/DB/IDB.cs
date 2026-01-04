using System.Data;

namespace DB;

public interface IDB
{
    /// <summary>
    /// スキーマDataTableを返す
    /// </summary>
    /// <param name="connectionString">接続文字列(SQLiteの場合はファイルパス)</param>
    /// <param name="tableName">指定テーブル名</param>
    /// <returns>スキーマDataTable</returns>
    DataTable GetSchema(string connectionString, string tableName);
}