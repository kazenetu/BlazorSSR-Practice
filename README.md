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
├─WebApp
└─WebApp.sln
　 ├─Components    // ページやパーツ、ナビゲーションを格納
　 │ ├─Layout
　 │ ├─Pages
　 │ ├─Parts        // ページングなどのパーツを格納
　 │ └─Providers    // SessionStorageなどを管理するクラスを格納
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
