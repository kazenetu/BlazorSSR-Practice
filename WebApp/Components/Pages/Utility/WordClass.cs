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
    /// テンプレートファイルをベースにテキスト置換済ファイルデータを返す
    /// </summary>
    /// <param name="templateFilePath">テンプレートファイルパス</param>
    /// <param name="replaceList">置換リスト</param>
    /// <param name="outputFileName">ファイル名</param>
    /// <returns>ファイルデータ</returns>
    /// <exception cref="Exception">テンプレートファイルが存在しない場合</exception>
    public DownLoadModel ReplaceText(string templateFilePath, List<(string targetText, string replacementText)> replaceList, string outputFileName)
    {
        var byteArray = File.ReadAllBytes(templateFilePath);
        using (var stream = new MemoryStream())
        {
            stream.Write(byteArray, 0, (int)byteArray.Length);
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
                    LoopParagraph(body.ChildElements, replaceList);
                }
                wordprocessingDocument.Save();

                mainDocumentPart.FeedData(stream);
                return new DownLoadModel(outputFileName, stream.ToArray());
            }
        }
    }

    /// <summary>
    /// 対象要素の子要素のテキスト置換え
    /// </summary>
    /// <param name="targets">対象要素</param>
    /// <param name="replaceList">置換リスト</param>
    private void LoopParagraph(OpenXmlElementList targets, List<(string targetText, string replacementText)> replaceList)
    {
        foreach (var target in targets)
        {
            if (target.HasChildren) LoopParagraph(target.ChildElements, replaceList);
            else if (target.GetType() == typeof(Text))
            {
                var text = target.InnerText;
                foreach (var replacement in replaceList)
                {
                    text = text.Replace(replacement.targetText, replacement.replacementText);
                }
                ((Text)target).Text = text;

            }
        }
    }

    /// <summary>
    /// 排他ロックオブジェクト
    /// </summary>
    private static ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();
}