# BlazorSSR-Practice
Blazor静的サーバーレンダリング (SSR) の実装確認

## 実行環境
* .NET8 SDK

## ライセンス
* [MITライセンス](LICENSE)  
* 使用パッケージは[THIRD-PARTY-NOTICES.txt](THIRD-PARTY-NOTICES.txt)を参照

## 実行方法
```dotnet run --project WebApp/WebApp.csproj```

## 注意
* レイアウト崩れが発生した場合は```dotnet clean```を試すこと

## フォルダ構成
```
Root
├─ExampleFiles      // ファイルアップロードサンプル用アップロードファイル
├─PdfReport         // PDFSharpを利用したPDF出力
│ ├─DataLists      // PDF出力用データリスト
│ ├─Interfaces     // PDF用インターフェイス
│ ├─Layouts        // PDF出力用レイアウト(C#ソースコードでのPDF定義)
│ └─assets         // フォントファイルを格納する
├─Tools
│ └─Create         // スケルトンコード生成ツール
│    └─Templates   // モード別のソースコード自動生成用テンプレート
├─WebApp
└─WebApp.sln
　 ├─Components    // ページやパーツ、ナビゲーションを格納
　 │ ├─Layout
　 │ ├─Pages
　 │ ├─Parts        // ページングなどのパーツを格納
　 │ └─Providers    // SessionStorageなどを管理するクラスを格納
　 ├─Components    // ページやパーツ、ナビゲーションを格納
　 ├─DI             // DI設定
　 ├─DBAccess       // DBアクセスクラス
　 ├─Extentions     // 拡張関数を格納
　 ├─Models         // Repositoryとページで利用するModelクラス
　 ├─Repositories   // 「SQL発行とModelクラスを返す」メソッド群
　 ├─Interfaces     // DI設定用インターフェイス
　 └─assets         // SQLiteファイルを格納する(デバッグ時に利用)
```

## ログ出力
NLogを利用している。  
* ```@inject ILogger<クラス名> Logger```  
  例:@inject ILogger<**Login**> Logger

* 通常ログ：Logger.LogInformation(**await GetLogMessage("開始")**);
* 例外ログ：Logger.LogError(await GetLogError(ex));

例：ログイン処理
```
/// <summary>
/// ログイン
/// </summary>
/// <param name="editContext"></param>
private async void SubmitLogin(EditContext editContext)
{
    try
    {
        Logger.LogInformation(await GetLogMessage("開始"));

        // エラーメッセージクリア
        errorMessages.Clear();

        // 未入力チェック
        var loginModel = editContext.Model as LoginModel;
        if (loginModel?.ID is null)
        {
            // エラーメッセージ
            errorMessages.Add("IDを入力してください。");
        }
        if (loginModel?.Password is null)
        {
            // エラーメッセージ
            errorMessages.Add("パスワードを入力してください。");
        }
        if (errorMessages.Any()) return;

        Logger.LogInformation(await GetLogMessage($"ログインチェック前 ログインID:{loginModel!.ID}"));
        if (!EqalsPassword(loginModel!))
        {
            errorMessages.Add("IDまたはパスワードが間違っています。");
        }
        if (errorMessages.Any()) return;

        // ログイン成功
        var userModel = userRepository.GetUser(loginModel!.ID!);
        await SetAsync(UserModel.SessionFullName, userModel!.Fullname);
        await SetAsync(UserModel.SessionLoggedIn, true);
        await SetAsync(UserModel.SessionAdminRole, userModel!.AdminRole);
        await SetAsync(UserModel.SessionUser, userModel.Serialize());

        Logger.LogInformation(await GetLogMessage($"ログイン成功"));

        // 画面遷移
        Navigation.NavigateTo("/");
        Navigation.Refresh(true);
    }
    catch(Exception ex)
    {
        Logger.LogError(ex, await GetLogError(ex));
        GotoErrorPage();
    }
    finally{
        Logger.LogInformation(await GetLogMessage("終了"));
    }
}
```
↓  
ログ出力結果
```
2024-10-30 19:24:59.1636||INFO|WebApp.Components.Pages.Login|開始 url=http://localhost:5034/login method=SubmitLogin 
2024-10-30 19:24:59.1636||INFO|WebApp.Components.Pages.Login|ログインチェック前 ログインID:aa url=http://localhost:5034/login method=SubmitLogin 
2024-10-30 19:24:59.1994||INFO|WebApp.Components.Pages.Login|aa Aさん ログイン成功 url=http://localhost:5034/login method=SubmitLogin 
2024-10-30 19:24:59.2046||INFO|WebApp.Components.Pages.Login|aa Aさん 終了 url=http://localhost:5034/ method=SubmitLogin 
```

## ソースコード生成ツール
モード別にスケルトンコード生成するツール。
### モード
* 一覧
* 編集
* アップロード
* テンプレート(必要最低限のスケルトンコード)
* スニペット「PDF」(実装例)
* スニペット「CSV」(実装例)

### 生成ファイル
カレントパスにOutputディレクトリに下記を生成する
* Components/Pages/{クラス名}.razor
* DI/DI{クラス名}.cs
* Models/{クラス名}Model.cs
* Models/Input{クラス名}Model.cs  
  → モード edit/upload/template 時に生成
* Repositories/{ClassName}Repository.cs
* Repositories/IRepository/I{ClassName}Repository.cs

### コマンド
```sh
#ルートパス上で実行
dotnet run --project Tools/Create/Create.csproj <RunMode>　<uri> [options]
```

<h4>○必須パラメータ</h4>  

|パラメータ名|設定値|概要|
|---|---|---|
|RunMode|list:一覧モード<br>edit:詳細<br>upload:アップロード<br>template:必要最低限のスケルトンコード<br>tips_pdf:スニペット「PDF」<br>tips_csv:スニペット「CSV」|スケルトン生成モード|
|uri|例）order-list|ページのURI<br>uriからクラス名を自動作成する<br>例）OderList|


<h4>○オプションパラメータ</h4>  

|パラメータ名|設定値|概要|
|---|---|---|
|--edit_key_type<br>-ekt|例）string|型情報(省略時:int)|
|--title|例）一覧|タイトル名(省略時:クラス名)|
|--repository<be>-rep|例）Shred|共通リポジトリ名※プレフィックスのみ(省略時:クラス名)|

### 実行例
  ```sh
   #一覧ページ生成　ページuri:order-list (クラス名:OrderList) キー型:int(初期値) タイトル名:クラス名(OrderList) リポジトリ名:クラス名(OrderList)
  dotnet run --project Tools/Create/Create.csproj list order-list 

  #詳細ページ生成　ページuri:order-edit (クラス名:OrderEdit) キー型:int(初期値) タイトル名:クラス名(OrderEdit) リポジトリ名:クラス名(OrderEdit)
  dotnet run --project Tools/Create/Create.csproj edit order-edit

  #アップロードページ生成　ページuri:order-upload (クラス名:OrderUpload) キー型:int(初期値) タイトル名:クラス名(OrderUpload) リポジトリ名:クラス名(OrderUpload)
  dotnet run --project Tools/Create/Create.csproj upload order-upload

  #テンプレートページ※生成　ページuri:ex-template (クラス名:ExTemplate) キー型:int(初期値) タイトル名:クラス名(ExTemplate) リポジトリ名:クラス名(ExTemplate)
  #※必要最低限のスケルトンコード
  dotnet run --project Tools/Create/Create.csproj template ex-template

  #スニペット「PDF」ページ生成　ページuri:ex-pdf (クラス名:ExPdf) キー型:int(初期値)  タイトル名:クラス名(ExPdf) リポジトリ名:クラス名(ExPdf)
  dotnet run --project Tools/Create/Create.csproj tips_pdf ex-pdf 

  #スニペット「CSV」ページ生成　ページuri:ex-csv (クラス名:ExCsv) キー型:int(初期値) タイトル名:クラス名(ExCsv) リポジトリ名:クラス名(ExCsv)
  dotnet run --project Tools/Create/Create.csproj tips_csv ex-csv
 ```

* オプション:型情報指定  
 ```sh
  #一覧ページ生成　ページuri:order-list (クラス名:OrderList) キー型:string タイトル名:クラス名(OrderList) リポジトリ名:クラス名(OrderList)
   dotnet run --project Tools/Create/Create.csproj list order-list --edit_key_type string 

  #詳細ページ生成　ページuri:order-edit (クラス名:OrderEdit) キー型:string タイトル名:クラス名(OrderEdit) リポジトリ名:クラス名(OrderEdit)
  dotnet run --project Tools/Create/Create.csproj edit order-edit --edit_key_type string 

  #アップロードページ生成　ページuri:order-upload (クラス名:OrderUpload) キー型:string タイトル名:クラス名(OrderUpload) リポジトリ名:クラス名(OrderUpload)
  dotnet run --project Tools/Create/Create.csproj upload order-upload --edit_key_type string 

  #テンプレートページ※生成　ページuri:ex-template (クラス名:ExTemplate) キー型:string タイトル名:クラス名(ExTemplate) リポジトリ名:クラス名(ExTemplate)
  #※必要最低限のスケルトンコード
  dotnet run --project Tools/Create/Create.csproj template ex-template --edit_key_type string 

  #スニペット「PDF」ページ生成　ページuri:ex-pdf (クラス名:ExPdf) キー型:string タイトル名:クラス名(ExPdf) リポジトリ名:クラス名(ExPdf)
  dotnet run --project Tools/Create/Create.csproj tips_pdf ex-pdf --edit_key_type string 

  #スニペット「CSV」ページ生成　ページuri:ex-csv (クラス名:ExCsv) キー型:string タイトル名:クラス名(ExCsv) リポジトリ名:クラス名(ExCsv)
  dotnet run --project Tools/Create/Create.csproj tips_csv ex-csv --edit_key_type string 
 ```

* オプション:型情報、タイトル指定  
 ```sh
  #一覧ページ生成　ページuri:order-list (クラス名:OrderList) キー型:string タイトル名:一覧 リポジトリ名:クラス名(OrderList)
   dotnet run --project Tools/Create/Create.csproj list order-list --edit_key_type string --title "一覧"

  #詳細ページ生成　ページuri:order-edit (クラス名:OrderEdit) キー型:string タイトル名:編集 リポジトリ名:クラス名(OrderEdit)
  dotnet run --project Tools/Create/Create.csproj edit order-edit --edit_key_type string --title "編集"

  #アップロードページ生成　ページuri:order-upload (クラス名:OrderUpload) キー型:string タイトル名:アップロード リポジトリ名:クラス名(OrderUpload)
  dotnet run --project Tools/Create/Create.csproj upload order-upload --edit_key_type string --title "アップロード"

  #テンプレートページ※生成　ページuri:ex-template (クラス名:ExTemplate) キー型:string タイトル名:テンプレート リポジトリ名:クラス名(ExTemplate)
  #※必要最低限のスケルトンコード
  dotnet run --project Tools/Create/Create.csproj template ex-template --edit_key_type string --title "テンプレート"

  #スニペット「PDF」ページ生成　ページuri:ex-pdf (クラス名:ExPdf) キー型:string タイトル名:スニペット「PDF」 リポジトリ名:クラス名(ExPdf)
  dotnet run --project Tools/Create/Create.csproj tips_pdf ex-pdf --edit_key_type string --title "スニペット「PDF」"

  #スニペット「CSV」ページ生成　ページuri:ex-csv (クラス名:ExCsv) キー型:string タイトル名:スニペット「CSV」 リポジトリ名:クラス名(ExCsv)
  dotnet run --project Tools/Create/Create.csproj tips_csv ex-csv --edit_key_type string --title "スニペット「CSV」"
 ```

* オプション:型情報、タイトル、リポジトリ指定  
 ```sh
  #一覧ページ生成　ページuri:order-list (クラス名:OrderList) キー型:string タイトル名:一覧 リポジトリ名:Shared
  dotnet run --project Tools/Create/Create.csproj list order-list --edit_key_type string --title "一覧" -rep Shared

  #詳細ページ生成　ページuri:order-edit (クラス名:OrderEdit) キー型:string タイトル名:編集 リポジトリ名:Shared
  dotnet run --project Tools/Create/Create.csproj edit order-edit --edit_key_type string --title "編集" -rep Shared

  #アップロードページ生成　ページuri:order-upload (クラス名:OrderUpload) キー型:string タイトル名:アップロード リポジトリ名:Shared
  dotnet run --project Tools/Create/Create.csproj upload order-upload --edit_key_type string --title "アップロード" -rep Shared

  #テンプレートページ※生成　ページuri:ex-template (クラス名:ExTemplate) キー型:string タイトル名:テンプレート リポジトリ名:Shared
  #※必要最低限のスケルトンコード
  dotnet run --project Tools/Create/Create.csproj template ex-template --edit_key_type string --title "テンプレート" -rep Shared

  #スニペット「PDF」ページ生成　ページuri:ex-pdf (クラス名:ExPdf) キー型:string タイトル名:スニペット「PDF」 リポジトリ名:Shared
  dotnet run --project Tools/Create/Create.csproj tips_pdf ex-pdf --edit_key_type string --title "スニペット「PDF」" -rep Shared

  #スニペット「CSV」ページ生成　ページuri:ex-csv (クラス名:ExCsv) キー型:string タイトル名:スニペット「CSV」 リポジトリ名:Shared
  dotnet run --project Tools/Create/Create.csproj tips_csv ex-csv --edit_key_type string --title "スニペット「CSV」" -rep Shared
 ```
