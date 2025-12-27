// 引数チェック
using System.Data;
using DB;
using Create;

if (args.Length < 3)
{
    Console.WriteLine("dotnet run DBモード 接続文字列 テーブル名 [ベースURL] [ハイフンモード]");
    Console.WriteLine("　DBモード：SQLite/DB/PostgreSQL");
    Console.WriteLine("　[ベースURL]：一覧URL");
    Console.WriteLine("　[ハイフンモード]：true/false");
    return;
}

// DB取得
var dbMode = args[0];
IDB db = dbMode switch
{
    "SQLite" => new SQLiteDB(),
    "PostgreSQL" => new PostgeSQLDB(),
    _ => throw new ArgumentOutOfRangeException(nameof(dbMode), $"Not expected direction value: {dbMode}"),
};

// スキーマ取得
var schemaTable = db.GetSchema(args[1], args[2]);

// スケルトンコード作成
var outputPath = Environment.CurrentDirectory+"/Output";
var createFiles = new CreateFils(outputPath, schemaTable.Columns, args[2]);
createFiles.Create();

