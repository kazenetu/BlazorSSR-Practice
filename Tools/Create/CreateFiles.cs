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
        Other,
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
            "other" => ModeEnum.Other,
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
            case ModeEnum.Other:
                Other();
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
    /// 
    /// </summary>
    /// <param name="rootPath"></param>
    /// <param name="contentsPath"></param>
    /// <param name="temlateFileName"></param>
    /// <param name="outputFileName"></param>
    /// <returns></returns>
    private void CreateFile(string rootPath, string contentsPath, string temlateFileName, string outputFileName)
    {
        var templatePath = Path.Combine(rootPath, contentsPath, temlateFileName);
        var contents = string.Empty;
        contents = ReadTemplate(templatePath);
        contents = contents.Replace("$ClassName$", ClassName);
        contents = contents.Replace("$uri$", Uri);
        if (EditKeyType is not null)
            contents = contents.Replace("$EditKeyType$", EditKeyType);
        else
            contents = contents.Replace("$EditKeyType$", "int");
        CreateFile($"{RootPath}/{contentsPath}", outputFileName, contents);
    }

    /// <summary>
    /// その他生成
    /// </summary>
    private void Other()
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="templatePath"></param>
    /// <returns></returns>
    private string ReadTemplate(string templatePath)
    {
        return File.ReadAllText(templatePath);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <param name="fileName"></param>
    /// <param name="contents"></param>
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