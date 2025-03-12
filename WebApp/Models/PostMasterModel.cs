namespace WebApp.Models;

/// <summary>
/// 郵便番号・住所 クラス
/// </summary>
/// <param name="PostCd">郵便番号</param>
/// <param name="Todohuken">都道府県名</param>
/// <param name="Shikuson">市町村名</param>
/// <param name="machi">町名</param>
/// <param name="TodohukenKana">都道府県名カナ</param>
/// <param name="ShikusonKana">市町村名カナ</param>
/// <param name="machiKana">町名カナ</param>
public record PostMasterModel(string PostCd, string Todohuken, string Shikuson, string machi, string TodohukenKana = "", string ShikusonKana = "", string machiKana = "");