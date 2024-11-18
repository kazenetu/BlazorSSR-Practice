using Microsoft.Extensions.Options;
using WebApp.DBAccess;
using WebApp.DBAccess.DB;
using WebApp.Repositories.Interfaces;

namespace WebApp.Repositories
{
  /// <summary>
  /// Repositoryのスーパークラス
  /// </summary>
  public class RepositoryBase : IRepositoryBase, IDisposable
  {
    /// <summary>
    /// データを永続化するDB用インターフェース
    /// </summary>
    protected IDatabase? db = null;

    /// <summary>
    /// DB情報
    /// </summary>
    private IOptions<DatabaseConfigModel>? config = null;

    /// <summary>
    /// DB設定取得用コンストラクタ
    /// </summary>
    /// <param name="config">DB設定取得</param>
    public RepositoryBase(IOptions<DatabaseConfigModel> config)
    {
      this.config = config;
      Initialize();
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Initialize()
    {
      // 設定されていない場合は終了
      if(config == null)
      {
        return;
      }

      // DatabaseFactoryから永続化対象のDBインスタンスを取得
      db = DatabaseFactory.Create(config.Value);
    }

    /// <summary>
    /// DBインスタンス設定用コンストラクタ
    /// </summary>
    /// <param name="db">外部から設定されたDBインスタンス</param>
    public RepositoryBase(IDatabase db)
    {
      this.db = db;
    }

    /// <summary>
    /// 破棄
    /// </summary>
    public void Dispose()
    {
      db?.Dispose();
    }

    /// <summary>
    /// トランザクション設定
    /// </summary>
    public void BeginTransaction()
    {
      db!.BeginTransaction();
    }

    /// <summary>
    /// コミット
    /// </summary>
    public void Commit()
    {
      db!.Commit();
    }

    /// <summary>
    /// ロールバック
    /// </summary>
    public void Rollback()
    {
      db!.Rollback();
    }

    /// <summary>
    /// DataRow[Name]をTに変換する
    /// </summary>
    /// <typeparam name="T">変換先の型</typeparam>
    /// <param name="value">変換対象</param>
    /// <returns>変換先の型(変換できない場合は初期値)</returns>
    protected T Parse<T>(object value) where T: notnull
    {
      object? result = null;
      object? defaultValue = null;
      
      if (typeof(T) == typeof(bool))
      {
        defaultValue = false;
        if(bool.TryParse(value.ToString(), out bool parseResult))
          result = parseResult;
      }
      if (typeof(T) == typeof(int))
      {
        defaultValue = 0;
        if(int.TryParse(value.ToString(), out int parseResult))
          result = parseResult;
      }
      if (typeof(T) == typeof(decimal))
      {
        defaultValue = 0m;
        if (decimal.TryParse(value.ToString(), out decimal parseResult))
          result = parseResult;
      }
      if (typeof(T) == typeof(float))
      {
        defaultValue = 0f;
        if (float.TryParse(value.ToString(), out float parseResult))
          result = parseResult;
      }
      if (typeof(T) == typeof(double))
      {
        defaultValue = 0f;
        if (double.TryParse(value.ToString(), out double parseResult))
          result = parseResult;
      }
      if (typeof(T) == typeof(string))
      {
        defaultValue = string.Empty;
        if (!string.IsNullOrEmpty(value.ToString()))
          result = value.ToString();
      }

      if (result is null) return (T)defaultValue!;
      return (T)result!;
    }
  }
}
