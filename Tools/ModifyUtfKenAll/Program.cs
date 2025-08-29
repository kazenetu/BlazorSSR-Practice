namespace ModifyUtfKenAll;
using ConvertCStoTS.Common;
using System.Text;
using System.Text.RegularExpressions;

class Program
{
    static void Main(string[] args)
    {
        // パラメータ取得
        var argManager = new ArgManagers(args);

        // ヘルプモードの確認
        var isShowHelp = false;
        if (argManager.GetRequiredArgCount() <= 0)
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
            Console.WriteLine("how to use: ModifyUtfKenAll <FilePath> [options]");
            Console.WriteLine("");
            Console.WriteLine("<FilePath> input utf_ken_all.csv full path");
            Console.WriteLine("");
            Console.WriteLine("options:");
            Console.WriteLine("-h, --help  view this page");
            Console.WriteLine();
            return;
        }

        // CSV読込み
        var postDataModels = new List<PostDataModel>();
        using (StreamReader sr = new StreamReader(argManager.GetRequiredArg(0)!))
        {
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                postDataModels.AddRange(GetPostDataModel(line));
            }
        }

        var outputPath = Environment.CurrentDirectory + "/Output";
        var contents = new StringBuilder();
        foreach (var item in postDataModels.DistinctBy(p => p.PostCd+p.TodohukenKana+p.ShikusonKana+p.MachiKana))
        {
            contents.Append($"{item.PostCd},\"{item.TodohukenKana}\",\"{item.ShikusonKana}\",\"{item.MachiKana}\",");
            contents.Append($"\"{item.Todohuken}\",\"{item.Shikuson}\",\"{item.Machi}\",");
            contents.AppendLine($"\"{item.Address}\",\"{item.AddressKana}\"");
        }
        CreateFile(outputPath, "PostData.csv", contents.ToString());
    }

    static List<PostDataModel> GetPostDataModel(string src)
    {
        var result = new List<PostDataModel>();

        string[] lines = src.Split(",");
        var postCd = lines[2].Replace("\"", string.Empty);
        var todohukenKana = lines[3].Replace("\"", string.Empty);
        var shikusonKana = lines[4].Replace("\"", string.Empty);
        var machiKana = lines[5].Replace("\"", string.Empty);
        var todohuken = lines[6].Replace("\"", string.Empty);
        var shikuson = lines[7].Replace("\"", string.Empty);
        var machi = lines[8].Replace("\"", string.Empty);

        // 削除：「以下に掲載がない」
        if (machi == "以下に掲載がない場合")
        {
            machi = string.Empty;
            machiKana = string.Empty;
        }

        // 郵便番号検索時に使用する住所取得
        var address = $"{todohuken}{shikuson}{machi}";
        var addressKana = $"{todohukenKana}{shikusonKana}{machiKana}";

        // 括弧開始キーワード
        var parenthesesKeyword = "（";

        // 括弧が存在する場合はチェック処理を行う
        if (machi.Contains(parenthesesKeyword))
        {
            // 括弧以前のキーワード
            var beforeParentheses = machi.Substring(0, machi.IndexOf(parenthesesKeyword));
            var beforeParenthesesKana = machiKana;
            if (machiKana.Contains(parenthesesKeyword))
                beforeParenthesesKana = machiKana.Substring(0, machiKana.IndexOf(parenthesesKeyword));

            // 括弧内のコレクションを取得
            var indexKanji = machi.IndexOf(parenthesesKeyword) + 1;
            var remarksKanji = machi.Substring(indexKanji, machi.Length - indexKanji - 1).Split("、");
            var indexKana = machiKana.IndexOf(parenthesesKeyword) + 1;
            var remarksKana = machiKana.Substring(indexKana, machiKana.Length - indexKana - 1).Split("、");

            // 漢字コレクションで処理を実行
            for (var index = 0; index < remarksKanji.Count(); index++)
            {
                var remark = remarksKanji[index];
                var isClear = false;

                // 数値、記号、その他を除外
                remark = ExclusionNumberAndKeywords(remark);

                // カタカナの設定
                var remarkKana = machiKana;
                if (machiKana.Contains(parenthesesKeyword))
                    remarkKana = remarksKana[index];

                // 「」内を除外する
                if (remark.Contains("「"))
                {
                    var tempKagiNone = string.Empty;
                    var remarkKagiStartIndex = remark.IndexOf("「");
                    tempKagiNone = $"{remark.Substring(0, remarkKagiStartIndex)}";
                    remark = tempKagiNone.Replace("」", string.Empty);

                    tempKagiNone = string.Empty;
                    remarkKagiStartIndex = remarkKana.IndexOf("＜");
                    tempKagiNone = $"{remarkKana.Substring(0, remarkKagiStartIndex)}";
                    remarkKana = tempKagiNone.Replace("＞", string.Empty); ;
                }

                // 特別処理：番地を含んだ地域を設定
                if (remark.Contains("番地") && remark.Replace("番地", string.Empty).Length > 0)
                {
                    remark = remark.Replace("番地", string.Empty);
                    remarkKana = ExclusionNumberAndKeywords(remarkKana).Replace("バンチ", string.Empty);

                    // 最後に「・」の場合は削除
                    if (remark.LastIndexOf("・") == remark.Length - 1)
                    {
                        remark = remark.Substring(0, remark.Length - 1);
                        remarkKana = remarkKana.Substring(0, remarkKana.Length - 1);
                    }
                }

                // 特別処理：及びが入っている場合は地名を抽出
                if (remark.Contains("及び"))
                {
                    isClear = false;

                    var andKeywordKanjiIndex = remark.IndexOf("及び");
                    remark = remark.Substring(andKeywordKanjiIndex + 2).Replace("番地", string.Empty);

                    var tempKana = ExclusionNumberAndKeywords(remarkKana);
                    var andKeywordKanaIndex = tempKana.IndexOf("オヨビ");
                    remarkKana = tempKana.Substring(andKeywordKanaIndex + 2).Replace("バンチ", string.Empty);
                }

                // 特別処理：岩手県の地割表記を削除
                if (!remark.Contains("地割"))
                {
                    isClear = false;
                    remark = remark.Replace("第", string.Empty).Replace("地割", string.Empty);
                    remarkKana = ExclusionNumberAndKeywords(remarkKana).Replace("ダイ", string.Empty).Replace("チワリ", string.Empty);
                }

                // その他はクリア
                if (!isClear && remark.Contains("その他")) isClear = true;

                // 番地はクリア
                if (!isClear && remark == "番地") isClear = true;

                // 無番地はクリア
                if (!isClear && remark.Contains("無番地")) isClear = true;

                // 大字はクリア
                if (!isClear && remark == "大字") isClear = true;

                // 丁目を含む場合はクリア
                if (!isClear && remark.Contains("丁目")) isClear = true;

                // 番地「数字-数字」「数字」はクリア
                if (string.IsNullOrEmpty(remark)) isClear = true;

                // 除外結果「の以内」はクリア
                if (remark == "の以内") isClear = true;

                // 除外結果「の」はクリア
                if (remark == "の" || remark == "の・") isClear = true;

                // の次にを含む場合はクリア
                if (!isClear && remark.Contains("の次に")) isClear = true;

                // 特別処理：北海道帯広市の「西９９～９９線」はクリア
                if (!isClear && Regex.IsMatch(remarksKanji[index], @"西([\uff10-\uff19])*(\u301c)*([\uff10-\uff19])*線")) isClear = true;

                // 特別処理：ビルの階層はクリア
                if (!isClear && Regex.IsMatch(remarksKanji[index], @"([\uff10-\uff19])+階")) isClear = true;
                if (!isClear && remarksKanji[index] == "地階・階層不明") isClear = true;

                // 町域名のクリア
                if (isClear)
                {
                    remark = string.Empty;
                    remarkKana = string.Empty;

                    //Console.WriteLine($"--------isClear={remark}");
                }

                result.Add(new PostDataModel(postCd, todohuken, shikuson, beforeParentheses + remark, todohukenKana, shikusonKana, beforeParenthesesKana + remarkKana, address, addressKana));

                if (isClear) continue;
                if (postCd != "0285233") continue;
                Console.Write($"  ...{remark} ");
                Console.WriteLine($"kana={remarkKana}");
            }
        }
        else
        {
            result.Add(new PostDataModel(postCd, todohuken, shikuson, machi, todohukenKana, shikusonKana, machiKana, address, addressKana));
        }

        return result;

        string ExclusionNumberAndKeywords(string target)
        {
            return target
                .Replace("０", string.Empty)
                .Replace("１", string.Empty)
                .Replace("２", string.Empty)
                .Replace("３", string.Empty)
                .Replace("４", string.Empty)
                .Replace("５", string.Empty)
                .Replace("６", string.Empty)
                .Replace("７", string.Empty)
                .Replace("８", string.Empty)
                .Replace("９", string.Empty)
                .Replace("−", string.Empty)
                .Replace("を除く", string.Empty)
                .Replace("以降", string.Empty)
                .Replace("以上", string.Empty)
                .Replace("以下", string.Empty)
                .Replace("〜", string.Empty);
        }
    }


    /// <summary>
    /// ファイル書き出し
    /// </summary>
    /// <param name="path">書き出しパス</param>
    /// <param name="fileName">書き出しファイル名</param>
    /// <param name="contents">書き出し内容(ソースコード)</param>
    static void CreateFile(string path, string fileName, string contents)
    {
        // ディレクトリ作成
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        // ファイル作成
        File.WriteAllText(Path.Combine(path, fileName), contents);
    }
}

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
public record PostDataModel(string PostCd, string Todohuken, string Shikuson, string Machi, string TodohukenKana, string ShikusonKana, string MachiKana, string Address, string AddressKana);

