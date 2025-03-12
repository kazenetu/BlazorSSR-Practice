using Microsoft.Extensions.Options;
using System.Data;
using System.Text;
using WebApp.DBAccess;
using WebApp.Models;
using WebApp.Repositories.Interfaces;

namespace WebApp.Repositories;

/// <summary>
/// 郵便番号・住所用リポジトリ
/// </summary>
public class PostMasterRepository : RepositoryBase, IPostMasterRepository
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public PostMasterRepository(IOptions<DatabaseConfigModel> config) : base(config)
    {
    }

    /// <summary>
    /// 郵便番号検索
    /// </summary>
    /// <param name="postCd">郵便番号</param>
    /// <returns>郵便番号・住所 クラス</returns>
    /// <remarks>住所を設定</remarks>
    public PostMasterModel SearchPostCd(string postCd)
    {
        var sql = new StringBuilder();
        sql.AppendLine("SELECT post_cd, todohuken_kana, sikuson_kana, machi_kana, todohuken, sikuson, machi FROM post_master ");
        sql.AppendLine("WHERE post_cd = @post_cd");

        // パラメータ初期化
        db!.ClearParam();

        // Param設定
        db.AddParam("@post_cd", postCd);

        // SQL発行
        var sqlResult = db.Fill(sql.ToString());
        foreach (DataRow row in sqlResult.Rows)
        {
            // 郵便番号・住所 クラス インスタンス作成
            var PostCd = postCd;
            var todohukenKana = Parse<string>(row["todohuken_kana"]);
            var shikusonKana = Parse<string>(row["sikuson_kana"]);
            var machiKana = Parse<string>(row["machi_kana"]);
            var todohuken = Parse<string>(row["todohuken"]);
            var shikuson = Parse<string>(row["sikuson"]);
            var machi = Parse<string>(row["machi"]);

            return new PostMasterModel(postCd, todohuken, shikuson, machi, todohukenKana, shikusonKana, machiKana);
        }
        return new PostMasterModel(string.Empty, string.Empty, string.Empty, string.Empty);
    }

    /// <summary>
    /// 住所検索
    /// </summary>
    /// <param name="address">住所文字列</param>
    /// <returns>郵便番号・住所 クラス</returns>
    /// <remarks>郵便番号を設定</remarks>
    public PostMasterModel SearchAddress(string address)
    {
        var sql = new StringBuilder();
        sql.AppendLine("SELECT post_cd, todohuken_kana, sikuson_kana, machi_kana, todohuken, sikuson, machi FROM post_master ");
        sql.AppendLine("where @address like todohuken||sikuson||machi||'%' ;");

        // パラメータ初期化
        db!.ClearParam();

        // Param設定
        db.AddParam("@address", address);

        // SQL発行
        var sqlResult = db.Fill(sql.ToString());
        foreach (DataRow row in sqlResult.Rows)
        {
            // 郵便番号・住所 クラス インスタンス作成
            var PostCd = Parse<string>(row["post_cd"]);
            var todohukenKana = Parse<string>(row["todohuken_kana"]);
            var shikusonKana = Parse<string>(row["sikuson_kana"]);
            var machiKana = Parse<string>(row["machi_kana"]);
            var todohuken = Parse<string>(row["todohuken"]);
            var shikuson = Parse<string>(row["sikuson"]);
            var machi = Parse<string>(row["machi"]);

            return new PostMasterModel(PostCd, todohuken, shikuson, machi, todohukenKana, shikusonKana, machiKana);
        }
        return new PostMasterModel(string.Empty, string.Empty, string.Empty, string.Empty);
    }

    /// <summary>
    /// 住所カナ取得
    /// </summary>
    /// <param name="address">住所文字列</param>
    /// <returns>住所カナ文字列</returns>
    public string GetAddressKana(string address)
    {
        var sql = new StringBuilder();
        sql.AppendLine("SELECT post_cd, todohuken_kana, sikuson_kana, machi_kana, todohuken, sikuson, machi FROM post_master ");
        sql.AppendLine("where @address like todohuken||sikuson||machi||'%' ;");

        // パラメータ初期化
        db!.ClearParam();

        // Param設定
        db.AddParam("@address", address);

        // SQL発行
        var sqlResult = db.Fill(sql.ToString());
        foreach (DataRow row in sqlResult.Rows)
        {
            var todohukenKana = Parse<string>(row["todohuken_kana"]);
            var shikusonKana = Parse<string>(row["sikuson_kana"]);
            var machiKana = Parse<string>(row["machi_kana"]);
            var todohuken = Parse<string>(row["todohuken"]);
            var shikuson = Parse<string>(row["sikuson"]);
            var machi = Parse<string>(row["machi"]);

            var addressKanji = $"{todohuken}{shikuson}{machi}";
            var addressKana = $"{todohukenKana}{shikusonKana}{machiKana}{address.Replace(addressKanji,string.Empty)}";

            return addressKana;
        }
        return string.Empty;
   }
}