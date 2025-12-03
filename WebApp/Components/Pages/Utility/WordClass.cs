namespace WebApp.Components.Pages.Utility;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using WebApp.Models.Parts;

/// <summary>
/// Word管理クラス
/// </summary>
public class WordClass : IDisposable
{
    /// <summary>
    /// コンストラクタ：新規作成
    /// </summary>
    public WordClass()
    {
        // ClosedXMLは非スレッドセーフのためロック取得
        cacheLock.EnterWriteLock();
    }

    /// <summary>
    /// 破棄
    /// </summary>
    public void Dispose()
    {
        // 排他ロック解放
        cacheLock.ExitWriteLock();
    }

    /// <summary>
    /// Wordファイルを返す
    /// </summary>
    /// <param name="templateFilePath">テンプレートファイルパス</param>
    /// <param name="replaceList">置換リスト</param>
    /// <param name="outputFileName">ファイル名</param>
    /// <returns>ファイルデータModel</returns>
    public DownLoadModel Create(string templateFilePath, List<Replacements> replaceList, string outputFileName)
    {
        // テンプレートファイルのバイト配列を取得
        TemplateBytes = File.ReadAllBytes(templateFilePath);

        byte[] srcDocBytes = [];
        var isFirst = true;
        foreach (var item in replaceList)
        {
            if (isFirst)
            {
                srcDocBytes = CreateDocument(templateFilePath, item);
                isFirst = false;
            }
            else
            {
                var destDocBytes = CreateDocument(templateFilePath, item);
                srcDocBytes = MergeDocuments(srcDocBytes, destDocBytes);
            }
        }
        return new DownLoadModel(outputFileName, srcDocBytes);
    }

    /// <summary>
    /// ソースドキュメントにドキュメントをマージする(改ページ)
    /// </summary>
    /// <param name="srcBytes">ソースドキュメント</param>
    /// <param name="combinedBytes">マージされるドキュメント</param>
    /// <returns>ドキュメントのバイト配列</returns>
    private byte[] MergeDocuments(byte[] srcBytes, byte[] combinedBytes)
    {
        using (var srcStream = new MemoryStream())
        {
            srcStream.Write(srcBytes, 0, (int)srcBytes.Length);

            using var srcDoc = WordprocessingDocument.Open(srcStream, true);

            using (var combinedStream = new MemoryStream())
            {
                combinedStream.Write(combinedBytes, 0, (int)combinedBytes.Length);

                using var destDoc = WordprocessingDocument.Open(combinedStream, true);

                // 結合先の本文を取得
                Body sourceBody = srcDoc!.MainDocumentPart!.Document.Body!;

                // 結合元の本文を取得
                Body combinedBody = destDoc!.MainDocumentPart!.Document.Body!;

                // 改行を追加
                Paragraph PageBreakParagraph = new Paragraph(new Run(new Break() { Type = BreakValues.Page }));
                sourceBody.Append(PageBreakParagraph);

                // 結合元の本文の子要素をすべて結合先にコピー
                foreach (var element in combinedBody.Elements().Where(e => !(e is SectionProperties)))
                {
                    // 要素を複製して追加 (元のドキュメントへの参照を避けるため)
                    sourceBody.Append(element.CloneNode(true));
                }
            }
            srcDoc.Save();

            srcDoc.MainDocumentPart.FeedData(srcStream);
            return srcStream.ToArray();
        }
    }

    /// <summary>
    /// ドキュメントを作成
    /// </summary>
    /// <param name="templateFilePath">テンプレートファイルパス</param>
    /// <param name="replacements">置換リスト</param>
    /// <returns>ドキュメントのバイト配列</returns>
    private byte[] CreateDocument(string templateFilePath, Replacements replacements)
    {
        using (var stream = new MemoryStream())
        {
            stream.Write(TemplateBytes, 0, (int)TemplateBytes.Length);
            using (var wordprocessingDocument = WordprocessingDocument.Open(stream, true))
            {
                // ドキュメント メインパーツを取得
                var mainDocumentPart = wordprocessingDocument.MainDocumentPart;

                // ドキュメント メインパーツが存在しない場合は例外エラーを返す
                if (mainDocumentPart is null)
                    throw new Exception($"添付ファイル[{templateFilePath}]が存在しません。");


                // ドキュメントルート要素を取得
                Document documentRoot = mainDocumentPart.Document;

                // Body要素を取得
                var bodyElements = documentRoot.Elements<Body>();
                var body = bodyElements?.FirstOrDefault();
                if (body is not null)
                {
                    // Body要素以下のテキストを置換え
                    ReplaceText(body.ChildElements, replacements);
                }
                wordprocessingDocument.Save();

                mainDocumentPart.FeedData(stream);
                return stream.ToArray();
            }
        }
    }

    /// <summary>
    /// 対象要素の子要素のテキスト置換え
    /// </summary>
    /// <param name="targets">対象要素</param>
    /// <param name="replaceList">置換リスト</param>
    private void ReplaceText(OpenXmlElementList targets, Replacements replaceList)
    {
        foreach (var target in targets)
        {
            if (target.HasChildren) ReplaceText(target.ChildElements, replaceList);
            else if (target.GetType() == typeof(Text))
            {
                var text = target.InnerText;
                foreach (var replacement in replaceList.Items)
                {
                    text = text.Replace(replacement.Keyword, replacement.ReplacementText);
                }
                ((Text)target).Text = text;
            }
        }
    }

    /// <summary>
    /// 排他ロックオブジェクト
    /// </summary>
    private static ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();

    /// <summary>
    /// テンプレートのバイト配列
    /// </summary>
    private byte[] TemplateBytes = [];
}

/// <summary>
/// 置換アイテム
/// </summary>
/// <param name="Keyword">キーワードテキスト</param>
/// <param name="ReplacementText">置換文字列</param>
public record ReplacementItem(string Keyword, string ReplacementText);

/// <summary>
/// 置換リスト
/// </summary>
/// <param name="Items">置換アイテム</param>
public record Replacements(List<ReplacementItem> Items);