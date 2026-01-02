## マスター用ソースコード生成ツール
マスタ一覧ページ・編集ページのスケルトンコード生成するツール。

### 生成ファイル
カレントパスにOutputディレクトリに下記を生成する
* Components/Pages/{クラス名}List.razor
* Components/Pages/{クラス名}Edit.razor
* DI/DI{クラス名}.cs
* Models/{クラス名}Model.cs
* Models/Input{クラス名}Model.cs  
* Repositories/{ClassName}Repository.cs
* Repositories/IRepository/I{ClassName}Repository.cs

### コマンド
```sh
#ルートパス上で実行
dotnet run --project Tools/CreateMaster/CreateMaster.csproj <DBモード>　<接続文字列> <テーブル名>
```

<h4>○必須パラメータ</h4>  

|パラメータ名|設定値|概要|
|---|---|---|
|DBモード|SQLite<br>PostgreSQL|DBの設定|
|接続文字列|SQLite：SQLiteファイル(.db)のパス<br>PostgreSQL：Server=<サーバー>;Port=<ポート番号>;User Id=<ユーザID>;Password=<パスワード>;Database=<DB名>>|SQLiteはファイルパスを指定|
|テーブル名|マスターテーブル名|物理テーブル名|

<h4>○オプションパラメータ</h4>  

|パラメータ名|設定値|概要|
|---|---|---|
|--url_prefix<br>-u|例）user|@pageに設定するURLのプレフィックス(省略時:クラス名)|
|--use_hyphen<br>-useHyp|例）false|@urlの値を作成する際、ハイフンかアンダーバーか設定(省略時:true)<br>true:ハイフン(-)<br>false:アンダーバー(_)|

### 実行例
  ```sh
   #SQLite　WebApp/assets/Test.db(相対パス)　対象テーブル m_user(ユーザーマスタ)
   dotnet run --project Tools/CreateMaster/CreateMaster.csproj  SQLite "WebApp/assets/Test.db" m_user


   #PostgreSQL　Docker立ち上げ必須　対象テーブル m_user(ユーザーマスタ)
   dotnet run --project Tools/CreateMaster/CreateMaster.csproj PostgreSQL "Server=localhost;Port=5433;User Id=test;Password=test;Database=testDB" m_user

   #オプションあり：「URLプレフィックス：user、アンダーバー設定」SQLite　WebApp/assets/Test.db(相対パス)　対象テーブル m_user(ユーザーマスタ)
   dotnet run --project Tools/CreateMaster/CreateMaster.csproj  SQLite "WebApp/assets/Test.db" m_user --url_prefix user --use_hyphen false
 ```
