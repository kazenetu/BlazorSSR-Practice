namespace Create;
using ConvertCStoTS.Common;

class Program
{
    static void Main(string[] args)
    {
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
            Console.WriteLine("<Mode> list/edit/upload/template/tips_pdf/tips_csv");
            Console.WriteLine("<Uri> @page uri ex)order-list");
            Console.WriteLine("");
            Console.WriteLine("options:");
            Console.WriteLine("-ekt --edit_key_type <KeyType> KeyType (NotSet:int) ex int or string or other... ");
            Console.WriteLine("--title <PageTitle> Display Title (NotSet:ClassName)");
            Console.WriteLine("-rep --repository <RepositoryPrefix> RepositoryPrefix\"Repository\" (NotSet:\"ClassName\"Repository)");
            Console.WriteLine("-h, --help  view this page");
            Console.WriteLine();
            return;
        }

        // ファイル生成
        var outputPath = Environment.CurrentDirectory+"/Output";
        var mode = argManager.GetRequiredArg(0);
        var uri = argManager.GetRequiredArg(1);
        var editKeyType = argManager.GetOptionArg(new List<string>() { "--edit_key_type", "-ekt" });
        var editTitle = argManager.GetOptionArg(new List<string>() { "--title" });
        var editReposiotry = argManager.GetOptionArg(new List<string>() { "-rep", "--repository" });
        var createFiles = new CreateFils(outputPath, mode!, uri!, editKeyType, editTitle, editReposiotry);
        createFiles.Create();
    }
}
