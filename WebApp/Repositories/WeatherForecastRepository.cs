using System.Data;
using System.Text;
using Microsoft.Extensions.Options;
using WebApp.DBAccess;
using WebApp.Models;
using WebApp.Repositories.Interfaces;

namespace WebApp.Repositories;

public class WeatherForecastRepository : RepositoryBase, IWeatherForecastRepository
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public WeatherForecastRepository(IOptions<DatabaseConfigModel> config) : base(config)
    {
    }

    /// <summary>
    /// レコード数を返す
    /// </summary>
    /// <returns>対象レコード数</returns>
    public int GetTotalRecord()
    {
        var result = 0;

        // パラメータ初期化
        db!.ClearParam();

        var sql = new StringBuilder();

        sql.AppendLine("SELECT count(target_date) AS cnt FROM t_teather_forecast");

        var sqlResult = db.Fill(sql.ToString());
        foreach (DataRow row in sqlResult.Rows)
        {
            result = Parse<int>(row["cnt"]);
            break;
        }

        return result;
    }

    /// <summary>
    /// 対象レコード配列を返す
    /// </summary>
    /// <param name="startIndex">取得開始レコードインデックス</param>
    /// <param name="recordCount">取得レコード数</param>
    /// <returns>指定範囲のレコード配列</returns>
    public WeatherForecastModel[] GetList(int startIndex, int recordCount)
    {
        var result = new List<WeatherForecastModel>();

        // パラメータ初期化
        db!.ClearParam();

        var sql = new StringBuilder();

        sql.AppendLine("SELECT target_date, temperature_c, summary FROM t_teather_forecast");
        sql.AppendLine("ORDER BY target_date");
        sql.AppendLine(db!.GetLimitSQL(recordCount, startIndex));

        var sqlResult = db.Fill(sql.ToString());
        foreach (DataRow row in sqlResult.Rows)
        {
            // 天気予報情報
            var targetDate = Parse<string>(row["target_date"]);
            var temperatureC = Parse<int>(row["temperature_c"]);
            var summary = Parse<string>(row["summary"]);
            result.Add(new WeatherForecastModel(DateOnly.Parse(targetDate), temperatureC, summary));
        }
        return [.. result];
    }
}
