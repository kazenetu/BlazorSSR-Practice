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

### 生成ファイル
カレントパスにOutputディレクトリに下記を生成する
* Components/Pages/{クラス名}.razor
* DI/DI{クラス名}.cs
* Models/{クラス名}Model.cs
* Models/Input{クラス名}Model.cs  
  → Editモード/Uploadモードのみ生成
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
|RunMode|list:一覧モード<br>edit:詳細<br>upload:アップロード|スケルトン生成モード|
|uri|例）order-list|ページのURI<br>uriからクラス名を自動作成する<br>例）OderList|


<h4>○オプションパラメータ</h4>  

|パラメータ名|設定値|概要|
|---|---|---|
|--edit_key_type<br>-ekt|例）string|型情報(省略時:int)<br>下記で使用<br>・詳細<br>・アップロード|

### 実行例
```sh
#一覧ページ生成　ページuri:order-list (クラス名:OrderList)
dotnet run --project Tools/Create/Create.csproj list order-list 

#詳細ページ生成　ページuri:order-edit (クラス名:OrderEdit)　キー型:string
dotnet run --project Tools/Create/Create.csproj edit order-edit --edit_key_type string

#アップロードページ生成　ページuri:order-upload (クラス名:OrderUpload)　キー型:string
dotnet run --project Tools/Create/Create.csproj upload order-upload --edit_key_type string
```