using System.Data;
using System.Runtime.CompilerServices;
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
    /// <param name="inputModel">検索条件</param>
    /// <returns>対象レコード数</returns>
    public int GetTotalRecord(InputWeatherForecastModel inputModel)
    {
        var result = 0;

        // パラメータ初期化
        db!.ClearParam();

        var sql = new StringBuilder();

        sql.AppendLine("SELECT count(target_date) AS cnt FROM t_teather_forecast");
        sql.Append(GetWhere(inputModel));


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
    /// <param name="inputModel">検索条件</param>
    /// <param name="startIndex">取得開始レコードインデックス</param>
    /// <param name="recordCount">取得レコード数</param>
    /// <returns>指定範囲のレコード配列</returns>
    public WeatherForecastModel[] GetList(InputWeatherForecastModel inputModel, int startIndex, int recordCount)
    {
        var result = new List<WeatherForecastModel>();

        // パラメータ初期化
        db!.ClearParam();

        var sql = new StringBuilder();

        sql.AppendLine("SELECT target_date, temperature_c, summary FROM t_teather_forecast");
        sql.Append(GetWhere(inputModel));
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

    /// <summary>
    /// 検索条件を取得
    /// </summary>
    /// <param name="inputModel"></param>
    /// <returns></returns>
    private string GetWhere(InputWeatherForecastModel inputModel)
    {
        if(db is null) return string.Empty;

        var where = new StringBuilder();
        if(inputModel.StartDate is not null)
        {
            if (where.Length > 0) where.Append(" AND ");
            where.AppendLine(" target_date >= @startDate");

            // Param設定
            db.AddParam("@startDate", inputModel.StartDate);
        }
        if(inputModel.EndDate is not null)
        {
            if (where.Length > 0) where.Append(" AND ");
            where.AppendLine(" target_date <= @endDate");

            // Param設定
            db.AddParam("@endDate", inputModel.EndDate);
        }
        if(inputModel.StartTemperatureC is not null)
        {
            if (where.Length > 0) where.Append(" AND ");
            where.AppendLine(" temperature_c >= @startTemperatureC");

            // Param設定
            db.AddParam("@startTemperatureC", inputModel.StartTemperatureC);
        }
        if(inputModel.EndTemperatureC is not null)
        {
            if (where.Length > 0) where.Append(" AND ");
            where.AppendLine(" temperature_c <= @endTemperatureC");

            // Param設定
            db.AddParam("@endTemperatureC", inputModel.EndTemperatureC);
        }
        if (where.Length > 0) return " WHERE " + where.ToString();

        return string.Empty;
    }
}
