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
        DataTable? result = null;

        var connection = $"Data Source={connectionString}";
        using (var conn = new SqliteConnection(connection))
        {
            conn.Open();

            result = GetTable(conn, tableName);

            conn.Close();
        }

        return result;
    }

    /// <summary>
    /// DB検索、スキーマDataTableを返す
    /// </summary>
    /// <param name="conn">コネクションインスタンス</param>
    /// <param name="tableName">指定テーブル名</param>
    /// <returns>スキーマDataTable</returns>
    private DataTable GetTable(SqliteConnection conn, string tableName)
    {
        var result = new DataTable();

        // SQL発行
        using (SqliteCommand command = conn.CreateCommand())
        {
            // 登録プロパティリスト
            var createTableParsedList = new List<CreateTableParsedColumn>();

            // 主キーリスト
            var constraintKeys = new List<string>();

            // SQL作成
            command.CommandText = $"SELECT sql FROM sqlite_master WHERE type='table' AND name='{tableName}'";

            using (SqliteDataReader reader = command.ExecuteReader())
            {
                // CREATE TABLE文解析、登録プロパティリスト、主キーリストに格納
                while (reader.Read())
                {
                    // CREATE TABLE取得
                    var sql = reader["sql"].ToString();

                    // 改行単位の配列作成
                    var sqlLines = (sql ?? "").Split(Environment.NewLine);
                    for (int i = 0; i < sqlLines?.Length; i++)
                    {
                        // CREATE TABLEと最後は対象外
                        if (i == 0 || i == sqlLines.Length - 1) continue;

                        var line = sqlLines[i];

                        // 主キーチェック
                        if (line.Contains("constraint"))
                        {
                            // ()の中身を文字列取得
                            var pattern = @"\(([^)]*)\)";

                            var matches = Regex.Matches(line, pattern);
                            foreach (Match match in matches)
                            {
                                var keys = match.Groups[1].Value.Split(",");
                                constraintKeys.AddRange(keys);
                            }
                            continue;
                        }

                        // プロパティ
                        // 行をスペースで配列作成
                        var cells = line.Split(' ');

                        // 行の要素を取得、再生成
                        var lineElements = new List<string>();
                        for (int j = 0; j < cells.Length; j++)
                        {
                            var cell = cells[j];
                            if (cell != "," && !string.IsNullOrEmpty(cell))
                                lineElements.Add(cell);
                        }
                        var name = lineElements[0];
                        var type = CreateTableParsedColumn.GetType(lineElements[1]);
                        var option = string.Join(string.Empty, lineElements[2..]);
                        createTableParsedList.Add(new CreateTableParsedColumn(name, type, option.ToLower()));
                    }
                }

                // Tabeleカラムを作成
                foreach (var createTableParsed in createTableParsedList)
                {
                    var temp = new DataColumn(createTableParsed.ColumnName, createTableParsed.ColumnType);
                    if (constraintKeys.Contains(createTableParsed.ColumnName))
                        temp.Unique = true;

                    if (createTableParsed.Option.Contains("notnull"))
                        temp.AllowDBNull = false;
                    else temp.AllowDBNull = true;

                    if (createTableParsed.Option.Contains("default"))
                    {
                        var value = createTableParsed.Option.Replace("default", string.Empty);
                        if (createTableParsed.ColumnType == typeof(bool) && int.TryParse(value, out var intVal))
                        {
                            temp.DefaultValue = intVal == 1;
                        }
                        else
                            temp.DefaultValue = value;
                    }

                    result.Columns.Add(temp);
                }


            }
        }

        return result;
    }
}