// 引数チェック
using System.Data;
using DB;

if (args.Length < 3)
{
    Console.WriteLine("dotnet run DBモード 接続文字列 テーブル名");
    Console.WriteLine("　DBモード：SQLite/DB/PostgreSQL");
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

// HACK スケルトンコード作成
foreach(DataColumn col in schemaTable.Columns)
{
    Console.WriteLine($"{col.DataType} {col} \tnotnull:{col.AllowDBNull} \tdef:{col.DefaultValue}");
}