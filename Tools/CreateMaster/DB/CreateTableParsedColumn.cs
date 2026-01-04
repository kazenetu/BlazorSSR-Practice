namespace DB;

/// <summary>
/// CREATETABLEパース済みカラム情報
/// </summary>
/// <param name="ColumnName">カラム名</param>
/// <param name="ColumnType">C#型</param>
/// <param name="Option">NotNullやdefaultなどのオプション</param>
public record CreateTableParsedColumn(string ColumnName, Type ColumnType, string Option)
{
    /// <summary>
    /// DBの型をC#型に変換
    /// </summary>
    /// <param name="dbType">DB型</param>
    /// <returns>C#型</returns>
    public static Type GetType(string dbType)
    {
        return dbType.ToUpper() switch
        {
            "INTEGER" => typeof(int),
            "INT" => typeof(int),
            "SMALLINT" => typeof(int),
            "BIGINT" => typeof(long),
            "FLOAT" => typeof(float),
            "REAL" => typeof(double),
            "DECIMAL" => typeof(decimal),
            "NUMERIC" => typeof(decimal),
            "DATE" => typeof(DateTime),
            "TIME" => typeof(TimeSpan),
            "BOOLEAN" => typeof(bool),
            _ => typeof(string)
        };
    }

}
