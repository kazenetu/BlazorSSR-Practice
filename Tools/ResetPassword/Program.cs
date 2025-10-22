namespace ResetPassword;

using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

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

    /// <summary>
    /// パスワード作成
    /// </summary>
    /// <param name="password">パスワード</param>
    /// <returns>暗号化済パスワードとソルトのタプル</returns>
    private static (string encryptedPassword, string salt) CreateEncryptedPassword(string password)
    {
        // 128ビットのソルトを生成する
        byte[] salt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        var saltBase64 = Convert.ToBase64String(salt);

        // 256ビットのサブキーを導出
        string encryptedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));

        // 暗号化済パスワードとソルトを返す
        return (encryptedPassword, saltBase64);
    }
}
