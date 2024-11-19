using System.Text.Json;

namespace WebApp.Extentions;

/// <summary>
/// JSON変換拡張関数
/// </summary>
public static class JsonExtention
{
    /// <summary>
    /// JSONシリアライズ
    /// </summary>
    /// <param name="src">シリアライズ対象のクラスインスタンス</param>
    /// <returns>JSON文字列</returns>
    static public string Serialize<T>(this T src) where T : class
    {
        return JsonSerializer.Serialize(src);
    }

    /// <summary>
    /// JSONデシアライズ
    /// </summary>
    /// <param name="src">JSON文字列</param>
    /// <typeparam name="T">デシアライズ用クラス</typeparam>
    /// <returns>デシアライズ用クラスのインスタンス</returns>
    static public T? Deserialize<T>(this string src) where T : class
    {
        return JsonSerializer.Deserialize<T>(src);
    }
}