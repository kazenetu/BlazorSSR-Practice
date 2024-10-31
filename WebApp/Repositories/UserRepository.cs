using Microsoft.Extensions.Options;
using System.Text;
using System.Data;
using WebApp.DBAccess;
using WebApp.Repositories.Interfaces;
using WebApp.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace WebApp.Repositories
{
  /// <summary>
  /// ユーザー用リポジトリ
  /// </summary>
  public class UserRepository : RepositoryBase, IUserRepository
  {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public UserRepository(IOptions<DatabaseConfigModel> config) : base(config)
    {
    }

    #region 取得

    /// <summary>
    /// ユーザーを取得
    /// </summary>
    /// <param name="userID">ユーザーID</param>
    /// <returns>ユーザー(存在しない場合はnull)</returns>
    public UserModel? GetUser(string userID)
    {
      UserModel? result = null;

      // パラメータ初期化
      db!.ClearParam();

      // パラメータ設定
      db.AddParam("@unique_name", userID);

      // SQL作成
      var sql = new StringBuilder();
      sql.AppendLine("SELECT");
      sql.AppendLine(" * ");
      sql.AppendLine("FROM");
      sql.AppendLine("  m_user");
      sql.AppendLine("WHERE");
      sql.AppendLine("  unique_name = @unique_name");

      var sqlResult = db.Fill(sql.ToString());
      foreach (DataRow row in sqlResult.Rows)
      {
        var id = Parse<string>(row["unique_name"]);
        var password = Parse<string>(row["password"]);
        var salt = Parse<string>(row["salt"]);
        var fullName = Parse<string>(row["fullname"]);
        var adminRole = Parse<bool>(row["admin_role"]);
        var disabled = Parse<bool>(row["admin_role"]);
        var version = Parse<int>(row["version"]);

        result = new UserModel(id, password, salt, fullName, adminRole, disabled, version);
        break;
      }

      return result;
    }

    /// <summary>
    /// パスワードチェック
    /// </summary>
    /// <param name="userID">ユーザーID</param>
    /// <param name="password">パスワード</param>
    /// <returns>パスワード一致か否か</returns>
    public bool EqalsPassword(string userID, string password)
    {
        var target = GetUser(userID);
        if (target is null || target.Disabled) return false;

        var salt = Convert.FromBase64String(target.Salt);

        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));

        return hashed == target.Password;
    }

    #endregion

    /// <summary>
    /// パスワード作成
    /// </summary>
    /// <param name="password">パスワード</param>
    /// <returns>暗号化済パスワードとソルトのタプル</returns>
    public (string encryptedPassword, string salt) CreateEncryptedPassword(string password)
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
}
