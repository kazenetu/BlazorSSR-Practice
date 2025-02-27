using System.Data;
using Microsoft.Data.Sqlite;
using static WebApp.DBAccess.DB.DatabaseFactory;

namespace WebApp.DBAccess.DB;

/// <summary>
/// SQLiteラッパークラス
/// </summary>
public class SQLiteDB : IDatabase
{
    /// <summary>
    /// セーブポイント名
    /// </summary>
    private const string SavePointName = "SAVE";

    #region プライベートフィールド

    /// <summary>
    /// コネクションインスタンス
    /// </summary>
    private SqliteConnection? conn = null;

    /// <summary>
    /// トランザクションインスタンス
    /// </summary>
    private SqliteTransaction? tran = null;

    /// <summary>
    /// パラメータ
    /// </summary>
    protected readonly Dictionary<string, object> param;

    /// <summary>
    /// トランザクションが開いているか否か
    /// </summary>
    private bool isTran = false;

    /// <summary>
    /// セーブポイントを設定しているか否か
    /// </summary>
    private bool isSetSavePoint = false;

    #endregion

    #region プロパティ

    /// <summary>
    /// DB種類
    /// </summary>
    public DatabaseTypes DBType
    {
        get
        {
            return DatabaseTypes.sqlite;
        }
    }
    #endregion

    #region パブリックメソッド

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="connectionString">接続文字列</param>
    public SQLiteDB(string connectionString)
    {
        this.conn = this.GetConnection(connectionString);
        this.conn.Open();

        this.param = new Dictionary<string, object>();
    }

    /// <summary>
    /// SELECTした行の一部だけを取り出すSQLの文字列を返す(ORDER後に定義)
    /// </summary>
    /// <param name="limit">取り出す行数</param>
    /// <param name="offset">スキップ行</param>
    /// <returns>範囲指定のSQL</returns>
    public string GetLimitSQL(long limit, long offset)
    {
        var offsetSQL = $"OFFSET {offset} ";
        if (offset <= 0)
        {
            // マイナスの場合OFFSETを指定しない
            offsetSQL = string.Empty;
        }

        return $" LIMIT {limit} {offsetSQL}";
    }

    /// <summary>
    /// パラメータを追加
    /// </summary>
    /// <param name="key">キー</param>
    /// <param name="value">値</param>
    public virtual void AddParam(string key, object value)
    {
        switch (value)
        {
            case DateTime dt:
                this.param.Add(key, dt.ToString("yyyy-MM-dd HH:mm:ss"));
                break;
            case object obj:
                this.param.Add(key, obj);
                break;
        }
    }

    /// <summary>
    /// パラメータをクリア
    /// </summary>
    public void ClearParam()
    {
        this.param.Clear();
    }

    /// <summary>
    /// SQL実行（登録・更新・削除）
    /// </summary>
    /// <param name="sql">SQLステートメント</param>
    /// <returns>処理件数</returns>
    public int ExecuteNonQuery(string sql)
    {
        // SQL未設定
        if (sql == string.Empty)
        {
            return 0;
        }

        // SQL発行
        using (SqliteCommand command = conn!.CreateCommand())
        {
            command.CommandText = sql;

            foreach (var key in this.param.Keys)
            {
                command.Parameters.AddWithValue(key, this.param[key]);
            }

            return command.ExecuteNonQuery();
        }
    }

    /// <summary>
    /// 検索SQL実行
    /// </summary>
    /// <param name="sql">SQLステートメント</param>
    /// <returns>検索結果</returns>
    public DataTable Fill(string sql)
    {
        // SQL未設定
        if (sql == string.Empty)
        {
            return new DataTable();
        }

        // SQL発行
        using (SqliteCommand command = conn!.CreateCommand())
        {
            command.CommandText = sql;

            foreach (var key in this.param.Keys)
            {
                command.Parameters.AddWithValue(key, this.param[key]);
            }

            using (SqliteDataReader reader = command.ExecuteReader())
            {
                //スキーマ取得
                var result = this.GetShcema(reader);

                while (reader.Read())
                {
                    var addRow = result.NewRow();

                    foreach (DataColumn col in result.Columns)
                    {
                        addRow[col.ColumnName] = reader[col.ColumnName];
                    }

                    result.Rows.Add(addRow);
                }

                return result;
            }
        }
    }

    /// <summary>
    /// トランザクション設定
    /// </summary>
    public void BeginTransaction()
    {
        if (isTran && isSetSavePoint)
        {
            throw new Exception("トランザクションが設定できませんでした。");
        }

        if (isTran)
        {
            ClearParam();
            ExecuteNonQuery($"SAVEPOINT {SavePointName};");

            isSetSavePoint = true;
        }
        else
        {
            this.tran = this.conn!.BeginTransaction();
            this.isTran = true;
        }
    }

    /// <summary>
    /// コミット
    /// </summary>
    public void Commit()
    {
        if (!this.isTran)
        {
            return;
        }

        if (isSetSavePoint)
        {
            ClearParam();
            ExecuteNonQuery($"RELEASE SAVEPOINT {SavePointName};");
            isSetSavePoint = false;
        }
        else
        {
            this.tran!.Commit();
            this.isTran = false;
        }
    }

    /// <summary>
    /// ロールバック
    /// </summary>
    public void Rollback()
    {
        if (!this.isTran)
        {
            return;
        }

        if (isSetSavePoint)
        {
            ClearParam();
            ExecuteNonQuery($"ROLLBACK TO SAVEPOINT {SavePointName};");
            isSetSavePoint = false;
        }
        else
        {
            this.tran!.Rollback();
            this.isTran = false;
        }
    }

    /// <summary>
    /// 解放処理
    /// </summary>
    public void Dispose()
    {
        if (isSetSavePoint)
        {
            ClearParam();
            ExecuteNonQuery($"ROLLBACK TO SAVEPOINT {SavePointName};");
            isSetSavePoint = false;
        }
        if (this.isTran)
        {
            this.tran!.Rollback();
        }
        if (this.tran != null)
        {
            this.tran!.Dispose();
        }
        this.conn?.Close();
        this.conn?.Dispose();
    }

    #endregion

    #region プライベートメソッド

    /// <summary>
    /// コネクション生成
    /// </summary>
    /// <param name="connectionString">接続文字列</param>
    /// <returns>コネクションインスタンス</returns>
    protected virtual SqliteConnection GetConnection(string connectionString)
    {
        var resourcePath = AppContext.BaseDirectory;
        resourcePath = Path.Combine(resourcePath, connectionString);

        // DBファイルが存在するか確認
        if (!File.Exists(resourcePath))
        {
            throw new Exception($"DBファイル「{resourcePath}」がありません。");
        }

        // コネクション作成
        return new SqliteConnection($"Data Source={resourcePath}");
    }

    /// <summary>
    /// スキーマ取得
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    private DataTable GetShcema(SqliteDataReader reader)
    {
        var result = new DataTable();

        for (var i = 0; i < reader.FieldCount; i++)
        {
            result.Columns.Add(new DataColumn(reader.GetName(i)));
        }

        return result;
    }
    #endregion
}
