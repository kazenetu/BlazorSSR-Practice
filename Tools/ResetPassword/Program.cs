namespace ResetPassword;

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
            Console.WriteLine("create update sql");
            Console.WriteLine("how to use: ResetPassword <id> <resetPassword>");
            Console.WriteLine("");
            Console.WriteLine("<id> PrimaryKey");
            Console.WriteLine("<resetPassword> reset password");
            Console.WriteLine();
            return;
        }

        // TODO 後続処理
    }
}
