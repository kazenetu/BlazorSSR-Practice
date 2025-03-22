namespace WebApp.Models;

/// <summary>
/// 郵便番号・住所 クラス
/// </summary>
/// <param name="PostCd">郵便番号</param>
/// <param name="Todohuken">都道府県名</param>
/// <param name="Shikuson">市町村名</param>
/// <param name="Machi">町域名</param>
/// <param name="TodohukenKana">都道府県名カナ</param>
/// <param name="ShikusonKana">市町村名カナ</param>
/// <param name="MachiKana">町域名カナ</param>
/// <param name="Address">住所(郵便番号検索時に使用)</param>
/// <param name="AddressKana">住所カナ(郵便番号検索時に使用)</param>
public record PostMasterModel(string PostCd, string Todohuken, string Shikuson, string Machi, string TodohukenKana = "", string ShikusonKana = "", string MachiKana = "", string Address = "", string AddressKana = "");
