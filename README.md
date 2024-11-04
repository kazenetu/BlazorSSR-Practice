# BlazorSSR-Practice
Blazor静的サーバーレンダリング (SSR) の実装確認

## 実行環境
* .NET8 SDK

## 実行方法
```dotnet run --project WebApp/WebApp.csproj```

## 注意
* レイアウト崩れが発生した場合は```dotnet clean```を試すこと

## フォルダ構成
```
Root
├─assets        // SQLiteファイルやフォントファイルなどを格納する
├─Components    // ページやナビゲーションを格納
│  ├─Layout
│  ├─Pages
│  └─Providers  // SessionStorageなどを管理するクラスを格納
├─DBAccess      // DBアクセスクラス
├─Extentions    // 拡張関数を格納
├─Models        // Repositoryとページで利用するModelクラス
├─Repositories  // 「SQL発行とModelクラスを返す」メソッド群
│  └─Interfaces // DI設定用インターフェイス
└─wwwroot
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
