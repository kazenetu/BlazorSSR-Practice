using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Options;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using WebApp.DBAccess;
using WebApp.Models;
using WebApp.Repositories.Interfaces;

namespace WebApp.Repositories;

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
            var totpSecrets = Parse<string>(row["totp_secrets"]);
            var fullName = Parse<string>(row["fullname"]);
            var adminRole = Parse<bool>(row["admin_role"]);
            var disabled = Parse<bool>(row["disabled"]);
            var version = Parse<int>(row["version"]);

            result = new UserModel(id, password, salt, totpSecrets, fullName, adminRole, disabled, version);
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

    #region 登録・更新

    /// <summary>
    /// ユーザーの保存
    /// </summary>
    /// <param name="target">登録対象</param>
    /// <param name="userId">ユーザーID</param>
    /// <param name="programId">プログラムID</param>
    /// <returns>成功/失敗</returns>
    /// <remarks>パスワードは平文を設定すること。Saltは未設定</remarks>
    public bool Save(UserModel target, string userId, string programId)
    {
        var result = false;
        try
        {
            // トランザクション開始
            BeginTransaction();

            // DB存在確認
            var dbResult = GetUser(target.ID);
            if (dbResult is not null)
            {
                result = Update(target, userId ?? string.Empty, programId);
            }
            else
            {
                result = Insert(target, userId ?? string.Empty, programId);
            }

            // 登録・更新結果でコミット・ロールバック
            if (result)
            {
                Commit();
            }
            else
            {
                Rollback();
            }

        }
        catch (Exception)
        {
            Rollback();
            throw;
        }
        return result;
    }

    /// <summary>
    /// 登録
    /// </summary>
    /// <param name="target">登録対象</param>
    /// <param name="userId">ユーザーID</param>
    /// <param name="programId">プログラムID</param>
    /// <returns>登録結果成功/失敗</returns>
    private bool Insert(UserModel target, string userId, string programId)
    {
        var sql = new StringBuilder();
        sql.AppendLine("INSERT ");
        sql.AppendLine("INTO m_user(unique_name,password,salt,fullname,admin_role,version) ");
        sql.AppendLine("VALUES (@unique_name,@password,@salt,@fullname,@admin_role, @version) ");

        // ソルトが未設定の場合はパスワード再生成
        var password = target.Password;
        var salt = target.Salt;
        if (string.IsNullOrEmpty(salt))
        {
            var passAndSalt = CreateEncryptedPassword(password);
            password = passAndSalt.encryptedPassword;
            salt = passAndSalt.salt;
        }

        // Param設定
        var date = DateTime.Now;
        db!.ClearParam();
        db.AddParam("@unique_name", target.ID);
        db.AddParam("@password", password);
        db.AddParam("@salt", salt);
        db.AddParam("@fullname", target.Fullname);
        db.AddParam("@admin_role", target.AdminRole);
        db.AddParam("@version", 1);

        // SQL発行
        if (db.ExecuteNonQuery(sql.ToString()) == 1)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="target">更新対象</param>
    /// <param name="userId">ユーザーID</param>
    /// <param name="programId">プログラムID</param>
    /// <returns>更新結果成功/失敗</returns>
    private bool Update(UserModel target, string userId, string programId)
    {
        var sql = new StringBuilder();
        sql.AppendLine("UPDATE m_user");
        sql.AppendLine("SET");
        sql.AppendLine("unique_name=@unique_name,");
        sql.AppendLine("password=@password,");
        sql.AppendLine("salt=@salt,");
        sql.AppendLine("fullname=@fullname,");
        sql.AppendLine("admin_role=@admin_role,");
        sql.AppendLine("version=version+1");
        sql.AppendLine("WHERE");
        sql.AppendLine("  unique_name = @unique_name");

        // ソルトが未設定の場合はパスワード再生成
        var password = target.Password;
        var salt = target.Salt;
        if (string.IsNullOrEmpty(salt))
        {
            var passAndSalt = CreateEncryptedPassword(password);
            password = passAndSalt.encryptedPassword;
            salt = passAndSalt.salt;
        }

        // Param設定
        var date = DateTime.Now;
        db!.ClearParam();
        db.AddParam("@unique_name", target.ID);
        db.AddParam("@password", password);
        db.AddParam("@salt", salt);
        db.AddParam("@fullname", target.Fullname);
        db.AddParam("@admin_role", target.AdminRole);
        db.AddParam("@version", target.Version);

        // SQL発行
        if (db.ExecuteNonQuery(sql.ToString()) == 1)
        {
            return true;
        }

        return false;
    }

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

    #endregion
}
