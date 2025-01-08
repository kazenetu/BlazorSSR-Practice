namespace Create;
using ConvertCStoTS.Common;

class Program
{
    static void Main(string[] args)
    {
        var webAppPath = GetWebAppPath().Replace("\\", "/");
        Console.WriteLine($"{webAppPath}");

        // パラメータ取得
        var argManager = new ArgManagers(args);

        // ヘルプモードの確認
        var isShowHelp = false;
        if (argManager.GetRequiredArgCount() <= 1)
        {
            // パラメータが不正の場合はヘルプモード
            isShowHelp = true;
        }
        if (argManager.ExistsOptionArg(new List<string>() { "--help", "-h" }))
        {
            // ヘルプオプションはヘルプモード
            isShowHelp = true;
        }

        // ヘルプ画面を表示
        if (isShowHelp)
        {
            Console.WriteLine();
            Console.WriteLine("how to use: Create <Mode> <Uri> [options]");
            Console.WriteLine("");
            Console.WriteLine("<Mode> list or edit or other");
            Console.WriteLine("<Uri> @page uri ex)order-list");
            Console.WriteLine("");
            Console.WriteLine("options:");
            Console.WriteLine("-ekt --edit_key_type <KeyType> EditMode KeyType int or string or other...");
            Console.WriteLine("-h, --help  view this page");
            Console.WriteLine();
            return;
        }

        webAppPath = Environment.CurrentDirectory+"/Output";

        // ファイル生成
        var mode = argManager.GetRequiredArg(0);
        var uri = argManager.GetRequiredArg(1);
        var editKeyType = argManager.GetOptionArg(new List<string>() { "--edit_key_type", "-ekt" });
        var createFiles = new CreateFils(webAppPath, mode!, uri!, editKeyType);
        createFiles.Create();
    }

    /// <summary>
    /// WebAppのパスを取得する
    /// </summary>
    /// <returns>WebAppのパス(見つからない場合はstring.Empty)</returns>
    private static string GetWebAppPath()
    {
        var result = string.Empty;

        // カレントパスがルートパス
        result = Path.Combine(Environment.CurrentDirectory, "WebApp");
        if (Directory.Exists(result))
        {
            return result;
        }

        // カレントパスが本ツールのルートパス
        result = Path.Combine(Environment.CurrentDirectory, "../../WebApp");
        if (Directory.Exists(result))
        {
            return result;
        }

        // 見つからない
        return string.Empty;
    }
}
