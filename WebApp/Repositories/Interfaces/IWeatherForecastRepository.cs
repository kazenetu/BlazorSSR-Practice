using WebApp.Models;

namespace WebApp.Repositories.Interfaces;

public interface IWeatherForecastRepository : IRepositoryBase
{
    /// <summary>
    /// レコード数を返す
    /// </summary>
    /// <param name="inputModel">検索条件</param>
    /// <returns>対象レコード数</returns>
    int GetTotalRecord(InputWeatherForecastModel inputModel);

    /// <summary>
    /// 対象レコード配列を返す
    /// </summary>
    /// <param name="inputModel">検索条件</param>
    /// <param name="startIndex">取得開始レコードインデックス</param>
    /// <param name="recordCount">取得レコード数</param>
    /// <returns>指定範囲のレコード配列</returns>
    WeatherForecastModel[] GetList(InputWeatherForecastModel inputModel, int startIndex, int recordCount);
}