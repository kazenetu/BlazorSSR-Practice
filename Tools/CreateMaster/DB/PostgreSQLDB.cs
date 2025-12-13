using System.Data;

namespace DB;

public class PostgeSQLDB: IDB
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