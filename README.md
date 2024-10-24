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
├─Models        // Repositoryとページで利用するModelクラス
├─Repositories  // 「SQL発行とModelクラスを返す」メソッド群
│  └─Interfaces // DI設定用インターフェイス
└─wwwroot
```

