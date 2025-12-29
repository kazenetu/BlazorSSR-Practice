// 引数チェック
using System.Data;
using DB;
using Create;
using Common;

// パラメータ取得
var argManager = new ArgManagers(args);

// ヘルプモードの確認
if (argManager.GetRequiredArgCount() < 3)
{
    Console.WriteLine("dotnet run DBモード 接続文字列 テーブル名 [ベースURL] [ハイフンモード]");
    Console.WriteLine("　DBモード：SQLite/DB/PostgreSQL");
    Console.WriteLine("　[ベースURL]：一覧URL");
    Console.WriteLine("　[ハイフンモード]：true/false");
    return;
}

// DB取得
var dbMode = argManager.GetRequiredArg(0);
IDB db = dbMode switch
{
    "SQLite" => new SQLiteDB(),
    "PostgreSQL" => new PostgeSQLDB(),
    _ => throw new ArgumentOutOfRangeException(nameof(dbMode), $"Not expected direction value: {dbMode}"),
};

// スキーマ取得
var connectionString = argManager.GetRequiredArg(1);
var dbTableName = argManager.GetRequiredArg(2);
var schemaTable = db.GetSchema(connectionString!, dbTableName!);

//　オプション：プレフィックスURL
var urlPrefix = argManager.GetOptionArg(new List<string>() { "--url_prefix", "-u" });

//　オプション：ハイフンモード
var urlUseHyphen = true;
var  useHyphen = argManager.GetOptionArg(new List<string>() { "--use_hyphen", "-useHyp" });
if (!string.IsNullOrEmpty(useHyphen) && useHyphen.ToLower() == "false")
{
    urlUseHyphen = false;
}

// スケルトンコード作成
var outputPath = Environment.CurrentDirectory+"/Output";
var createFiles = new CreateFils(outputPath, schemaTable.Columns, dbTableName!, urlPrefix, urlUseHyphen);
createFiles.Create();

