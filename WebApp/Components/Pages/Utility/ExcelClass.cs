namespace WebApp.Components.Pages.Utility;

using ClosedXML.Excel;

/// <summary>
/// Excel管理クラス
/// </summary>
public class ExcelClass: IDisposable
{
    /// <summary>
    /// コンストラクタ：新規作成
    /// </summary>
    public ExcelClass()
    {
        // ClosedXMLは非スレッドセーフのためロック取得
        cacheLock.EnterWriteLock();

        Workbook = new XLWorkbook();
    }

    /// <summary>
    /// コンストラクタ：テンプレート読み込み
    /// </summary>
    /// <param name="templateFilePath">読み込みテンプレートのパス</param>
    public ExcelClass(string templateFilePath)
    {
        // ClosedXMLは非スレッドセーフのためロック取得
        cacheLock.EnterWriteLock();

        Workbook = new XLWorkbook(templateFilePath);
    }

    /// <summary>
    /// 破棄
    /// </summary>
    public void Dispose()
    {
        Workbook?.Dispose();

        // 排他ロック解放
        cacheLock.ExitWriteLock();
    }

    /// <summary>
    /// ワークシート取得
    /// </summary>
    /// <param name="sheetName">ワークシート名</param>
    /// <returns>存在する場合は対象シート、しない場合は新規作成</returns>
    /// <remarks>利用ページで編集すること</remarks>
    public IXLWorksheet GetWorksheet(string sheetName)
    {
        if (Workbook is null) 
            throw new Exception("Workbook is null");

        // 存在する場合は対象シートを返す
        IXLWorksheet? targetWorksheet = null;
        if (Workbook.TryGetWorksheet(sheetName, out targetWorksheet))
        {
            return targetWorksheet;
        }

        // 存在しない場合はシートを追加して返す
        return Workbook.AddWorksheet(sheetName);
    }

    /// <summary>
    /// ワークブックを返す
    /// </summary>
    /// <returns>ワークブック</returns>
    public XLWorkbook GetWorkbook()
    {
        if (Workbook is null) 
            throw new Exception("Workbook is null");

        return Workbook;
    }

    /// <summary>
    /// ワークブック
    /// </summary>
    private XLWorkbook? Workbook = null;

    /// <summary>
    /// 排他ロックオブジェクト
    /// </summary>
    private static ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();
}