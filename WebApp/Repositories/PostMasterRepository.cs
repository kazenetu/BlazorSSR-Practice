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
        sql.AppendLine("SELECT post_cd, todohuken_kana, sikuson_kana, machi_kana, todohuken, sikuson, machi, address, address_kana FROM post_master ");
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
            var todohukenKana = Parse<string>(row["todohuken_kana"]);
            var shikusonKana = Parse<string>(row["sikuson_kana"]);
            var machiKana = Parse<string>(row["machi_kana"]);
            var todohuken = Parse<string>(row["todohuken"]);
            var shikuson = Parse<string>(row["sikuson"]);
            var machi = Parse<string>(row["machi"]);
            var dbAddress = Parse<string>(row["address"]);
            var dbAddressKana = Parse<string>(row["address_kana"]);

            return new PostMasterModel(postCd, todohuken, shikuson, machi, todohukenKana, shikusonKana, machiKana, dbAddress, dbAddressKana);
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
        sql.AppendLine("SELECT post_cd, todohuken_kana, sikuson_kana, machi_kana, todohuken, sikuson, machi, address, address_kana FROM post_master ");
        sql.AppendLine("where @address like todohuken||sikuson||machi||'%'");

        // パラメータ初期化
        db!.ClearParam();

        // Param設定
        db.AddParam("@address", address);

        // SQL発行
        var sqlResult = db.Fill(sql.ToString());

        var postMasterModels = new List<(int mismatchCount, PostMasterModel model)>();
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

            // 不一致文字数取得
            var tempAddress = $"{todohuken}{shikuson}{machi}";
            var mismatchCount = address.Replace(tempAddress, string.Empty).Length;
            if (mismatchCount == address.Length) mismatchCount = int.MaxValue;

            // リスト追加
            postMasterModels.Add((mismatchCount, new PostMasterModel(PostCd, todohuken, shikuson, machi, todohukenKana, shikusonKana, machiKana)));
        }

        // リストが存在する場合、最小文字数のModelを返す
        if (postMasterModels.Count != 0)
        {
            return postMasterModels.OrderBy(item => item.mismatchCount).First().model;

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
        sql.AppendLine("where @address like todohuken||sikuson||machi||'%'");

        // パラメータ初期化
        db!.ClearParam();

        // Param設定
        db.AddParam("@address", address);

        // SQL発行
        var sqlResult = db.Fill(sql.ToString());
        var postMasterModels = new List<(int mismatchCount, PostMasterModel model)>();
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

            // 不一致文字数取得
            var tempAddress = $"{todohuken}{shikuson}{machi}";
            var mismatchCount = address.Replace(tempAddress, string.Empty).Length;
            if (mismatchCount == address.Length) mismatchCount = int.MaxValue;

            // リスト追加
            postMasterModels.Add((mismatchCount, new PostMasterModel(PostCd, todohuken, shikuson, machi, todohukenKana, shikusonKana, machiKana)));
        }

        // リストが存在する場合、最小文字数のModelでフリガナを作成して返す
        if (postMasterModels.Count != 0)
        {
            var target = postMasterModels.OrderBy(item => item.mismatchCount).First().model;
            var addressKanji = $"{target.Todohuken}{target.Shikuson}{target.Machi}";
            var addressKana = $"{target.TodohukenKana}{target.ShikusonKana}{target.MachiKana}{address.Replace(addressKanji, string.Empty)}";

            return addressKana;
        }

        return string.Empty;
    }
}