namespace ResetPassword;

using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;
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

        // コンソールに更新用SQL作成
        Console.WriteLine("--Run This SQL");
        Console.WriteLine(CreateUpdateSQL(argManager.GetRequiredArg(0)!, argManager.GetRequiredArg(1)!));
        Console.WriteLine();
    }

    /// <summary>
    /// 更新用SQL取得
    /// </summary>
    /// <param name="id">PrimaryId</param>
    /// <param name="resetPassword">平文パスワード</param>
    /// <param name="programId">プログラムID</param>
    /// <returns>更新用SQL文</returns>
    private static string CreateUpdateSQL(string id, string resetPassword)
    {
        // 暗号化済パスワードとソルトを取得
        var passAndSalt = CreateEncryptedPassword(resetPassword);
        var encryptedPassword = passAndSalt.encryptedPassword;
        var salt = passAndSalt.salt;

        // 更新用SQL作成
        var sql = new StringBuilder();
        sql.Append("UPDATE m_user ");
        sql.Append("SET ");
        sql.Append($"password='{encryptedPassword}',");
        sql.Append($"salt='{salt}' ");
        sql.Append("WHERE ");
        sql.Append($"unique_name = '{id}'");

        //SQLを返す
        return sql.ToString();
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
        var encryptedPassword = Convert.ToBase64String(Rfc2898DeriveBytes.Pbkdf2(password, salt, 10000, HashAlgorithmName.SHA1, 256 / 8));

        // 暗号化済パスワードとソルトを返す
        return (encryptedPassword, saltBase64);
    }
}
