namespace Create;

using System.Data;
using System.IO;
using System.Text;

/// <summary>
/// ファイル作成クラス
/// </summary>
public class CreateFils
{
    /// <summary>
    /// ファイル生成のルートパス
    /// </summary>
    private string RootPath;

    List<DataColumn> Columns = [];

    /// <summary>
    /// オプション：リポジトリ名
    /// </summary>
    private string? EditReposiotry;

    /// <summary>
    /// テーブル名
    /// </summary>
    private string TableName;

    /// <summary>
    /// クラス名
    /// </summary>
    /// <remarks>ページのuriから自動生成</remarks> 
    private string ClassName;

    /// <summary>
    /// ページの@pageのプレフィックスurl
    /// </summary>
    private string UrlPrefix;

    /// <summary>
    /// 編集ページの@pageを[UrlPrefix]-editとするか否か(falsの場合は[UrlPrefix]_edit)
    /// </summary>
    private bool UrlUseHyphen;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="rootPath">ファイル生成のルートパス</param>
    /// <param name="columns">DB対象テーブルのカラムコレクション</param>
    /// <param name="tableName">DB対象テーブル名</param>
    /// <param name="urlPrefix">ページの@pageにプレフィックスurl(nullの場合はクラス名の小文字)</param>
    /// <param name="urlUseHyphen">編集ページの@pageを[urlPrefix]-editとするか否か(falsの場合は[urlPrefix]_edit)</param>
    public CreateFils(string rootPath, DataColumnCollection columns, string tableName, string? urlPrefix = null, bool urlUseHyphen = true)
    {
        RootPath = rootPath;
        TableName = tableName;
        UrlUseHyphen = urlUseHyphen;

        foreach (DataColumn col in columns)
        {
            Columns.Add(col);
        }

        var words = tableName.Split("_");
        ClassName = string.Empty;
        foreach (var word in words)
        {
            ClassName += word.Substring(0, 1).ToUpper();
            ClassName += word.Substring(1).ToLower();
        }

        if (string.IsNullOrEmpty(urlPrefix))
            UrlPrefix = ClassName.ToLower();
        else
            UrlPrefix = urlPrefix;

        // リポジトリ名はクラス名を設定
        EditReposiotry = ClassName;
    }

    /// <summary>
    /// ファイル生成
    /// </summary>
    public void Create()
    {
        var rootPath = $"{System.AppDomain.CurrentDomain.BaseDirectory}Templates";
        CreateModel(rootPath);
        CreateRepository(rootPath);
        CreateDI(rootPath);
        CreatePageList(rootPath);
        CreatePageEdit(rootPath);
    }

    /// <summary>
    /// ファイル出力：Model
    /// </summary>
    /// <param name="rootPath">テンプレートパス</param>
    private void CreateModel(string rootPath)
    {
        // プロパティ定義
        var modelProp = Columns.Select(col => $"    public {ToCShrpType(col.DataType)}{Nullable(col)} {ToCsharpName(col.ColumnName)} {{ set; get; }}{SetDefaultValue(col)}");
        CreateFile(rootPath, "Models", "Model.txt", $"{ClassName}Model.cs", new Dictionary<string, string>
        {
            {"$Properties$", string.Join(Environment.NewLine, modelProp)}
        });

        string SetDefaultValue(DataColumn target)
        {
            if (target.DefaultValue == DBNull.Value) return string.Empty;
            return $" = {target.DefaultValue.ToString()?.ToLower()};";
        }

        string Nullable(DataColumn target)
        {
            if (target.DataType == typeof(bool)) return string.Empty;
            return "?";
        }
    }

    /// <summary>
    /// ファイル出力：IRepository、Repository
    /// </summary>
    /// <param name="rootPath">テンプレートパス</param>
    private void CreateRepository(string rootPath)
    {
        // インターフェイス
        var keys = Columns.Where(col => col.Unique).Select(col => $"{col.DataType.Name}{(col.AllowDBNull ? "?" : "")} {ToCsharpName(col.ColumnName, true)}");
        CreateFile(rootPath, "Repositories/Interfaces", "IRepository.txt", $"I{ClassName}Repository.cs", new Dictionary<string, string>
        {
            {"$Keys$", string.Join(",", keys)}
        });

        // リポジトリ
        var newItems = Columns.Where(col => col.DefaultValue == DBNull.Value).Select(col => $"            {ToCsharpName(col.ColumnName)} = {GetDefault(col.DataType)},");

        var keyParams = Columns.Where(col => col.Unique).Select(col => $"{ToCsharpName(col.ColumnName, true)}");
        var selectOrder = Columns.Where(col => col.Unique).Select(col => $"{col.ColumnName}");
        var findWhere = Columns.Where(col => col.Unique).Select(col => $"{col.ColumnName} = @{col.ColumnName}");
        var findWhereParams = Columns.Where(col => col.Unique).Select(col => $"        db.AddParam(\"@{col.ColumnName}\", {ToCsharpName(col.ColumnName, true)}!);");
        var setDBParams = Columns.Select(col => $"                {ToCsharpName(col.ColumnName)} = Parse<{ToCShrpType(col.DataType)}>(row[\"{col.ColumnName}\"]),");
        var saveTargetKeys = Columns.Where(col => col.Unique).Select(col => $"target.{ToCsharpName(col.ColumnName)}!");
        var insertParams = Columns.Select(col => $"{col.ColumnName}");
        var insertValues = Columns.Select(col => $"@{col.ColumnName}");

        var updateValues = Columns.Select(col => $"{col.ColumnName} = @{col.ColumnName}");
        
        var dbParams = Columns.Select(col => $"        db.AddParam(\"@{col.ColumnName}\", target.{ToCsharpName(col.ColumnName)}!);");
        CreateFile(rootPath, "Repositories", "Repository.txt", $"{ClassName}Repository.cs", new Dictionary<string, string>
        {
            {"$NewItems$", string.Join(Environment.NewLine, newItems)},

            {"$tabeleName$", TableName},
            {"$Keys$", string.Join(",", keys)},
            {"$KeyParams$", string.Join(",", keyParams)},
            {"$SelectOrder$", string.Join(",", selectOrder)},
            {"$FindWhere$", string.Join(" AND ", findWhere)},
            {"$FindWhereParams$", string.Join(Environment.NewLine, findWhereParams)},
            {"$SetDBParams$", string.Join(Environment.NewLine, setDBParams)},
            {"$SaveTargetKeys$", string.Join(",", saveTargetKeys)},
            {"$InsertParams$", string.Join(",", insertParams)},
            {"$InsertValues$", string.Join(",", insertValues)},

            {"$UpdateValues$", string.Join(",", updateValues)},

            {"$DBParams$", string.Join(Environment.NewLine, dbParams)},
        });

        string GetDefault(Type target)
        {
            var defaultValue = "\"\"";

            if (target == typeof(bool))
            {
                defaultValue = "false";
            }
            if (target == typeof(int))
            {
                defaultValue = "0";
            }
            if (target == typeof(ulong))
            {
                defaultValue = "0";
            }
            if (target == typeof(decimal))
            {
                defaultValue = "0m";
            }
            if (target == typeof(float))
            {
                defaultValue = "0f";
            }
            if (target == typeof(double))
            {
                defaultValue = "0f";
            }

            return defaultValue;
        }
    }

    /// <summary>
    /// ファイル出力：DI
    /// </summary>
    /// <param name="rootPath">テンプレートパス</param>
    private void CreateDI(string rootPath)
    {
        // プロパティ定義
        CreateFile(rootPath, "DI", "DI.txt", $"{ClassName}DI.cs", new Dictionary<string, string>
        {
        });
    }

    /// <summary>
    /// ファイル出力：一覧ページ
    /// </summary>
    /// <param name="rootPath">テンプレートパス</param>
    private void CreatePageList(string rootPath)
    {
        var resultHeader = new StringBuilder();
        var resultBody = new StringBuilder();
        var editKeys = new List<string>();
        var editKeyType = string.Empty;
        foreach (DataColumn col in Columns)
        {
            // 結果：ヘッダ
            resultHeader.AppendLine($"                    <th>{ToCsharpName(col.ColumnName)}</th>");

            // 結果：値
            resultBody.AppendLine($"                            <td>@pageRecord.{ToCsharpName(col.ColumnName)}</td>");

            // キー
            if (col.Unique)
            {
                editKeyType = col.DataType.Name;
                editKeys.Add($"pageRecord.{ToCsharpName(col.ColumnName)}");
            }
        }

        CreateFile(rootPath,  "Components/Pages", "PageList.txt", $"{ClassName}List.razor", new Dictionary<string, string>
        {
            {"$uri$", UrlPrefix},
            {"$ResultHeader$", resultHeader.ToString()},
            {"$ResultBody$", resultBody.ToString()},
            {"$EditKeys$", string.Join(",", editKeys)},
            {"$EditKeyType$", editKeyType},
        });
    }

    /// <summary>
    /// ファイル出力：新規登録・編集
    /// </summary>
    /// <param name="rootPath">テンプレートパス</param>
    private void CreatePageEdit(string rootPath)
    {
        var editInput = new StringBuilder();
        var inputCheck = new StringBuilder();
        var editKeys = new List<string>();
        var editKeyType = string.Empty;
        foreach (DataColumn col in Columns)
        {
            // 結果：入力項目
            editInput.AppendLine($"            <div class=\"row row-margin\">");
            editInput.AppendLine($"                <div class=\"col\">");
            editInput.AppendLine($"                    <label class=\"col-md-4\" for=\"{ToCsharpName(col.ColumnName)}\">{ToCsharpName(col.ColumnName)}</label>");
            editInput.AppendLine($"                    <{GetInputComponent(col)} class=\"col-md-5\" id=\"{ToCsharpName(col.ColumnName)}\" @bind-Value=\"Model!.{ToCsharpName(col.ColumnName)}\" />");
            editInput.AppendLine($"                </div>");
            editInput.AppendLine($"            </div>");

            // 入力チェック
            inputCheck.Append(InputCheck(col));

            // キー
            if (col.Unique)
            {
                editKeys.Add($"Model.{ToCsharpName(col.ColumnName)}");
                editKeyType = col.DataType.Name;
            }
        }

        var editUrl = UrlPrefix;
        if (UrlUseHyphen)
            editUrl += "-";
        else
            editUrl += "_";
        editUrl += "edit";

        CreateFile(rootPath,  "Components/Pages", "PageEdit.txt", $"{ClassName}Edit.razor", new Dictionary<string, string>
        {
            {"$uri$", editUrl},
            {"$EditInput$", editInput.ToString()},
            {"$InputCheck$", inputCheck.ToString()},
            {"$ModelKey$", string.Join(",", editKeys)},
            {"$EditKeys$", string.Join(",", editKeys)},
            {"$EditKeyType$", editKeyType},
        });

        string GetInputComponent(DataColumn target)
        {
            Type[] numericTypes = [typeof(int), typeof(long), typeof(decimal)];
            if (numericTypes.Contains(target.DataType))
                return "InputNumber";
            else if (target.DataType == typeof(bool))
                return "InputCheckbox";
            return "InputText";
        }

        string InputCheck(DataColumn target)
        {
            // boolは入力チェックなし
            if (target.DataType == typeof(bool)) return string.Empty;

            var result = new StringBuilder();
            if (target.Unique)
            {
                result.AppendLine($"            if (model?.{ToCsharpName(target.ColumnName)} is null)");
                result.AppendLine("            {");
                result.AppendLine($"                errorMessages.AppendLine(\"{ToCsharpName(target.ColumnName)}を入力してください。\");");
                result.AppendLine("            }");
                return result.ToString();
            }

            // 任意の場合はチェックなし
            if (target.AllowDBNull) return string.Empty;

            // 必須入力チェック
            Type[] numericTypes = [typeof(int), typeof(decimal), typeof(long), typeof(float), typeof(double)];
            var isNumeric = numericTypes.Contains(target.DataType);
            if (isNumeric)
            {
                result.Append($"            if (model?.{ToCsharpName(target.ColumnName)} < ");
                if (target.DefaultValue != DBNull.Value)
                    result.Append(target.DefaultValue.ToString()?.ToLower());
                else
                    result.Append("0");
                result.AppendLine($")");
            }
            else if(target.DataType == typeof(bool))
            {
                result.Append($"            if (model?.{ToCsharpName(target.ColumnName)} == ");
                if (target.DefaultValue != DBNull.Value)
                    result.Append(target.DefaultValue.ToString()?.ToLower());
                else
                    inputCheck.Append("false");
                result.AppendLine($")");
                
            }
            else
            {
                result.AppendLine($"            if (string.IsNullOrEmpty(model?.{ToCsharpName(target.ColumnName)}))");
            }
            result.AppendLine("            {");
            result.AppendLine($"                errorMessages.AppendLine(\"{ToCsharpName(target.ColumnName)}を入力してください。\");");
            result.AppendLine("            }");

            return result.ToString();
        }
    }

    /// <summary>
    /// DBカラム名をC#コード体系(クラス名やプロパティ名)に変換
    /// </summary>
    /// <param name="dbName">DBカラム名</param>
    /// <param name="firstLower">1文字目を小文字にする</param>
    /// <returns>C#コード体系(クラス名やプロパティ名)</returns>
    private string ToCsharpName(string dbName, bool firstLower = false)
    {
        var words = dbName.Split("_");
        var result = string.Empty;
        foreach (var word in words)
        {
            result += word.Substring(0, 1).ToUpper();
            result += word.Substring(1).ToLower();
        }

        if (firstLower)
            result = result.Substring(0, 1).ToLower() + result[1..];

        return result;
    }

    /// <summary>
    /// DataColumnの型をC#の型に変換する
    /// </summary>
    /// <param name="targetColumnType">DataColumnの型</param>
    /// <returns>C#の型</returns>
    private string ToCShrpType(Type targetColumnType)
    {
        if (targetColumnType == typeof(string))
            return "string";
        if (targetColumnType == typeof(bool))
            return "bool";
        if (targetColumnType == typeof(int))
            return "int";
        if (targetColumnType == typeof(decimal))
            return "decimal";
        if (targetColumnType == typeof(long))
            return "long";
        if (targetColumnType == typeof(float))
            return "float";
        if (targetColumnType == typeof(double))
            return "double";
        return targetColumnType.Name;
    }

    /// <summary>
    /// テンプレートファイルをベースにスケルトンコードを自動生成
    /// </summary>
    /// <param name="rootPath">テンプレートファイルの起点パス</param>
    /// <param name="contentsPath">読込と書き込みのパス</param>
    /// <param name="temlateFileName">テンプレートファイル名</param>
    /// <param name="outputFileName">書き込みファイル名</param>
    /// <returns></returns>
    private void CreateFile(string rootPath, string contentsPath, string temlateFileName, string outputFileName, Dictionary<string, string> addReplaceItems)
    {
        // テンプレートファイル読込
        var templatePath = Path.Combine(rootPath, contentsPath, temlateFileName);
        var contents = string.Empty;
        contents = ReadTemplate(templatePath);

        // 実際のクラス名など置き換え
        contents = contents.Replace("$ClassName$", ClassName);
        contents = contents.Replace("$ReposiotryName$", EditReposiotry);

        // タイトル(省略時はクラス名)
        contents = contents.Replace("$Title$", ClassName);

        // 追加置換
        if (addReplaceItems.Any())
        {
            foreach (var item in addReplaceItems)
            {
                contents = contents.Replace(item.Key, item.Value);
            }
        }

        // インスタンスフィールド「RootPath」起点でファイル書き出し
        OutputFile($"{RootPath}/{contentsPath}", outputFileName, contents);
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
    private void OutputFile(string path, string fileName, string contents)
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