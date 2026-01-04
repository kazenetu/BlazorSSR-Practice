using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using Npgsql;

namespace DB;

public class PostgeSQLDB : IDB
{
    /// <summary>
    /// スキーマDataTableを返す
    /// </summary>
    /// <param name="connectionString">接続文字列</param>
    /// <param name="tableName">指定テーブル名</param>
    /// <returns>スキーマDataTable</returns>
    public DataTable GetSchema(string connectionString, string tableName)
    {
        DataTable? result = null;

        var connection = connectionString;
        using (var conn = new NpgsqlConnection(connection))
        {
            conn.Open();

            result = GetTable(conn, tableName);

            conn.Close();
        }

        return result;
    }

    /// <summary>
    /// 対象テーブルの主キーを返す
    /// </summary>
    /// <param name="conn">コネクションインスタンス</param>
    /// <param name="tableName">指定テーブル名</param>
    /// <returns>主キーリスト</returns>
    private List<string> GetConstraintKeys(NpgsqlConnection conn, string tableName)
    {
        var result = new List<string>();

        // SQL発行
        using (NpgsqlCommand command = conn.CreateCommand())
        {
            // SQL作成
            var sql = new StringBuilder();
            sql.AppendLine(@"
                SELECT
                    kcu.column_name
                FROM
                    information_schema.table_constraints AS tc
                JOIN
                    information_schema.key_column_usage AS kcu
                ON
                    tc.constraint_name = kcu.constraint_name
                    AND tc.table_schema = kcu.table_schema
                    AND tc.table_name = kcu.table_name
                WHERE
                    tc.constraint_type = 'PRIMARY KEY'
                    AND tc.table_schema = 'public'
            ");
            sql.AppendLine($"AND tc.table_name = '{tableName}'");
            command.CommandText = sql.ToString();

            using (NpgsqlDataReader reader = command.ExecuteReader())
            {
                // 主キーを取得
                while (reader.Read())
                {
                    var colName = reader["column_name"].ToString() ?? string.Empty;
                    if (string.IsNullOrEmpty(colName)) continue;

                    result.Add(colName);
                }
            }
        }
        return result;
    }

    /// <summary>
    /// DB検索、スキーマDataTableを返す
    /// </summary>
    /// <param name="conn">コネクションインスタンス</param>
    /// <param name="tableName">指定テーブル名</param>
    /// <returns>スキーマDataTable</returns>
    private DataTable GetTable(NpgsqlConnection conn, string tableName)
    {
        var result = new DataTable();

        // 主キーリスト
        var constraintKeys = GetConstraintKeys(conn ,tableName);

        // SQL発行
        using (NpgsqlCommand command = conn.CreateCommand())
        {
            // 登録プロパティリスト
            var createTableParsedList = new List<CreateTableParsedColumn>();

            // SQL作成
            var sql = new StringBuilder();
            sql.AppendLine(@"
                SELECT 
                    column_name, 
                    data_type, 
                    is_nullable, 
                    column_default 
                FROM 
                    information_schema.columns 
                WHERE 
                    table_schema = 'public'
            ");
            sql.AppendLine($"AND table_name = '{tableName}'");
            sql.AppendLine("ORDER BY ordinal_position");
            command.CommandText = sql.ToString();

            using (NpgsqlDataReader reader = command.ExecuteReader())
            {
                // カラム登録
                while (reader.Read())
                {
                    // DBカラムスキーマ取得
                    var name = reader["column_name"].ToString()??string.Empty;
                    var type = CreateTableParsedColumn.GetType(reader["data_type"].ToString()??string.Empty);
                    var isNull = (reader["is_nullable"].ToString()??string.Empty).ToUpper();
                    var defaultValue = reader["column_default"].ToString()??string.Empty;

                    // カラム設定
                    var temp = new DataColumn(name, type);
                    if (constraintKeys.Contains(name))
                        temp.Unique = true;

                    if (isNull.Contains("NO"))
                        temp.AllowDBNull = false;
                    else
                        temp.AllowDBNull = true;


                    if (!string.IsNullOrEmpty(defaultValue))
                        temp.DefaultValue = defaultValue;

                    result.Columns.Add(temp);
                }
            }
        }

        return result;
    }
}