namespace Create;
using System.IO;

/// <summary>
/// ファイル作成クラス
/// </summary>
public class CreateFils
{
    private enum ModeEnum
    {
        None,
        List,
        Edit,
        Upload,
        Template,
        TipsPdf,
        TipsCsv,
    }

    private string RootPath;
    private ModeEnum Mode;
    private string Uri;
    private string? EditKeyType;
    private string ClassName;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="rootPath">ファイル生成のルートパス</param>
    /// <param name="mode">モード</param>
    /// <param name="uri">ページのuri</param>
    /// <param name="editKeyType">編集ページの主キー型</param>
    public CreateFils(string rootPath, string mode, string uri, string? editKeyType)
    {
        RootPath = rootPath;
        Uri = uri;
        EditKeyType = editKeyType;
        Mode = mode switch
        {
            "list" => ModeEnum.List,
            "edit" => ModeEnum.Edit,
            "upload" => ModeEnum.Upload,
            "template" => ModeEnum.Template,
            "tips_pdf" => ModeEnum.TipsPdf,
            "tips_csv" => ModeEnum.TipsCsv,
            _ => ModeEnum.None
        };

        var words = uri.Split("-");
        ClassName = string.Empty;
        foreach (var word in words)
        {
            ClassName += word.Substring(0, 1).ToUpper();
            ClassName += word.Substring(1).ToLower();
        }
    }

    /// <summary>
    /// ファイル生成
    /// </summary>
    public void Create()
    {
        switch (Mode)
        {
            case ModeEnum.List:
                List();
                break;
            case ModeEnum.Edit:
                Edit();
                break;
            case ModeEnum.Upload:
                Upload();
                break;
            case ModeEnum.Template:
                Template();
                break;
            case ModeEnum.TipsPdf:
                TipsPdf();
                break;
            case ModeEnum.TipsCsv:
                TipsCsv();
                break;

        }
    }

    /// <summary>
    /// リスト生成
    /// </summary>
    private void List()
    {
        var rootPath = $"{System.AppDomain.CurrentDomain.BaseDirectory}Templates/list";

        // ページ
        CreateFile(rootPath, "Components/Pages", "Page.txt", $"{ClassName}.razor");

        // モデル
        CreateFile(rootPath, "Models", "Model.txt", $"{ClassName}Model.cs");
        CreateFile(rootPath, "Models", "InputModel.txt", $"Input{ClassName}Model.cs");

        // リポジトリ
        CreateFile(rootPath, "Repositories", "Repository.txt", $"{ClassName}Repository.cs");
        CreateFile(rootPath, "Repositories/Interfaces", "IRepository.txt", $"I{ClassName}Repository.cs");

        // DI
        CreateFile(rootPath, "DI", "DI.txt", $"DI{ClassName}.cs");
    }

    /// <summary>
    /// 編集生成
    /// </summary>
    private void Edit()
    {
        var rootPath = $"{System.AppDomain.CurrentDomain.BaseDirectory}Templates/edit";

        // ページ
        CreateFile(rootPath, "Components/Pages", "Page.txt", $"{ClassName}.razor");

        // モデル
        CreateFile(rootPath, "Models", "Model.txt", $"{ClassName}Model.cs");

        // リポジトリ
        CreateFile(rootPath, "Repositories", "Repository.txt", $"{ClassName}Repository.cs");
        CreateFile(rootPath, "Repositories/Interfaces", "IRepository.txt", $"I{ClassName}Repository.cs");

        // DI
        CreateFile(rootPath, "DI", "DI.txt", $"DI{ClassName}.cs");
    }

    /// <summary>
    /// アップロード生成
    /// </summary>
    private void Upload()
    {
        var rootPath = $"{System.AppDomain.CurrentDomain.BaseDirectory}Templates/upload";

        // ページ
        CreateFile(rootPath, "Components/Pages", "Page.txt", $"{ClassName}.razor");

        // モデル
        CreateFile(rootPath, "Models", "Model.txt", $"{ClassName}Model.cs");
        CreateFile(rootPath, "Models", "InputModel.txt", $"Input{ClassName}Model.cs");

        // リポジトリ
        CreateFile(rootPath, "Repositories", "Repository.txt", $"{ClassName}Repository.cs");
        CreateFile(rootPath, "Repositories/Interfaces", "IRepository.txt", $"I{ClassName}Repository.cs");

        // DI
        CreateFile(rootPath, "DI", "DI.txt", $"DI{ClassName}.cs");
    }

    /// <summary>
    /// ページなしテンプレート生成
    /// </summary>
    private void Template()
    {
        var rootPath = $"{System.AppDomain.CurrentDomain.BaseDirectory}Templates/template";

        // ページ
        CreateFile(rootPath, "Components/Pages", "Page.txt", $"{ClassName}.razor");

        // モデル
        CreateFile(rootPath, "Models", "Model.txt", $"{ClassName}Model.cs");
        CreateFile(rootPath, "Models", "InputModel.txt", $"Input{ClassName}Model.cs");

        // リポジトリ
        CreateFile(rootPath, "Repositories", "Repository.txt", $"{ClassName}Repository.cs");
        CreateFile(rootPath, "Repositories/Interfaces", "IRepository.txt", $"I{ClassName}Repository.cs");

        // DI
        CreateFile(rootPath, "DI", "DI.txt", $"DI{ClassName}.cs");
    }

    /// <summary>
    /// TipsPDF生成
    /// </summary>
    private void TipsPdf()
    {
        var rootPath = $"{System.AppDomain.CurrentDomain.BaseDirectory}Templates/tips_pdf";

        // ページ
        CreateFile(rootPath, "Components/Pages", "Page.txt", $"{ClassName}.razor");

        // モデル
        CreateFile(rootPath, "Models", "Model.txt", $"{ClassName}Model.cs");
        CreateFile(rootPath, "Models", "InputModel.txt", $"Input{ClassName}Model.cs");

        // リポジトリ
        CreateFile(rootPath, "Repositories", "Repository.txt", $"{ClassName}Repository.cs");
        CreateFile(rootPath, "Repositories/Interfaces", "IRepository.txt", $"I{ClassName}Repository.cs");

        // DI
        CreateFile(rootPath, "DI", "DI.txt", $"DI{ClassName}.cs");
    }

    /// <summary>
    /// TipsCSV生成
    /// </summary>
    private void TipsCsv()
    {
        var rootPath = $"{System.AppDomain.CurrentDomain.BaseDirectory}Templates/tips_csv";

        // ページ
        CreateFile(rootPath, "Components/Pages", "Page.txt", $"{ClassName}.razor");

        // モデル
        CreateFile(rootPath, "Models", "Model.txt", $"{ClassName}Model.cs");
        CreateFile(rootPath, "Models", "InputModel.txt", $"Input{ClassName}Model.cs");

        // リポジトリ
        CreateFile(rootPath, "Repositories", "Repository.txt", $"{ClassName}Repository.cs");
        CreateFile(rootPath, "Repositories/Interfaces", "IRepository.txt", $"I{ClassName}Repository.cs");

        // DI
        CreateFile(rootPath, "DI", "DI.txt", $"DI{ClassName}.cs");
    }

    /// <summary>
    /// テンプレートファイルをベースにスケルトンコードを自動生成
    /// </summary>
    /// <param name="rootPath">テンプレートファイルの起点パス</param>
    /// <param name="contentsPath">読込と書き込みのパス</param>
    /// <param name="temlateFileName">テンプレートファイル名</param>
    /// <param name="outputFileName">書き込みファイル名</param>
    /// <returns></returns>
    private void CreateFile(string rootPath, string contentsPath, string temlateFileName, string outputFileName)
    {
        // テンプレートファイル読込
        var templatePath = Path.Combine(rootPath, contentsPath, temlateFileName);
        var contents = string.Empty;
        contents = ReadTemplate(templatePath);

        // 実際のクラス名など置き換え
        contents = contents.Replace("$ClassName$", ClassName);
        contents = contents.Replace("$uri$", Uri);

        var defaultEditKeyType = "default";
        if (EditKeyType is not null)
        {
            contents = contents.Replace("$EditKeyType$", EditKeyType);
            if (EditKeyType.ToLower() == "string")
                defaultEditKeyType = "string.Empty";
        }
        else
            contents = contents.Replace("$EditKeyType$", "int");
        contents = contents.Replace("$DefaultEditKeyType$", defaultEditKeyType);
        
        // インスタンスフィールド「RootPath」起点でファイル書き出し
        CreateFile($"{RootPath}/{contentsPath}", outputFileName, contents);
    }

    /// <summary>
    /// テンプレートファイルの読込
    /// </summary>
    /// <param name="templatePath">テンプレートファイルのパス</param>
    /// <returns>読込結果の文字列</returns>
    private string ReadTemplate(string templatePath)
    {
        return File.ReadAllText(templatePath);
    }

    /// <summary>
    /// ファイル書き出し
    /// </summary>
    /// <param name="path">書き出しパス</param>
    /// <param name="fileName">書き出しファイル名</param>
    /// <param name="contents">書き出し内容(ソースコード)</param>
    private void CreateFile(string path, string fileName, string contents)
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