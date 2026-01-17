using Microsoft.Extensions.Options;
using System.Data;
using System.Text;
using WebApp.DBAccess;
using WebApp.Extentions;
using WebApp.Models;
using WebApp.Repositories.Interfaces;

namespace WebApp.Repositories;

/// <summary>
/// 注文用リポジトリ
/// </summary>
public class OrderRepository : RepositoryBase, IOrderRepository
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public OrderRepository(IOptions<DatabaseConfigModel> config) : base(config)
    {
    }

    #region 取得

    /// <summary>
    /// 注文リストを取得
    /// </summary>
    /// <returns>注文リスト</returns>
    public List<OrderModel> GetOderList()
    {
        return Find(null);
    }

    /// <summary>
    /// 注文キーリストを取得
    /// </summary>
    /// <returns>注文リスト</returns>
    public List<OrderModel> GetOderKeyList()
    {
        var result = new List<OrderModel>();

        // パラメータ初期化
        db!.ClearParam();

        var sql = new StringBuilder();
        sql.AppendLine("SELECT  productName, unitPrice*qty AS Total FROM t_order");
        sql.AppendLine("ORDER BY productName");

        var sqlResult = db.Fill(sql.ToString());
        foreach (DataRow row in sqlResult.Rows)
        {
            // 注文キー情報
            var product = Parse<string>(row["productName"]);
            var totalPrice = Parse<decimal>(row["Total"]);
            result.Add(new OrderModel(0, product, 0, 0, totalPrice));
        }
        return result;
    }

    /// <summary>
    /// 注文情報を取得
    /// </summary>
    /// <param name="productName">製品名</param>
    /// <returns>注文情報</returns>
    public OrderModel GetOder(string productName)
    {
        var dbResult = Find(productName);
        if (dbResult.Count != 0)
        {
            return dbResult.First();
        }
        return new OrderModel(0, string.Empty, 0, 0, 0);
    }

    /// <summary>
    /// 注文リストを取得：メイン部分
    /// </summary>
    /// <param name="productName">商品名</param>
    /// <returns>注文リスト</returns>
    /// <remarks>メイン処理</remarks>
    private List<OrderModel> Find(string? productName)
    {
        var result = new List<OrderModel>();

        // パラメータ初期化
        db!.ClearParam();

        var sql = new StringBuilder();

        sql.AppendLine("select ROW_NUMBER() OVER(ORDER BY productName) AS NoId,* from t_order");

        // 商品名指定
        if (productName is not null)
        {
            sql.AppendLine("WHERE");
            sql.AppendLine("  productName = @product_name");

            // Param設定
            db.AddParam("@product_name", productName);
        }
        sql.AppendLine("ORDER BY productName");

        var sqlResult = db.Fill(sql.ToString());
        foreach (DataRow row in sqlResult.Rows)
        {
            // 注文
            var no = Parse<int>(row["NoId"]);
            var enabled = Parse<bool>(row["enabled"]);
            var product = Parse<string>(row["productName"]);
            var unitPrice = Parse<decimal>(row["unitPrice"]);
            var qty = Parse<decimal>(row["qty"]);
            var totalPrice = unitPrice * qty;
            var version = Parse<int>(row["version"]);
            result.Add(new OrderModel(no, product, unitPrice, qty, totalPrice, enabled, version));
        }
        return result;
    }

    #region ページング用
    /// <summary>
    /// レコード数を返す
    /// </summary>
    /// <param name="inputModel">検索条件</param>
    /// <returns>対象レコード数</returns>
    public int GetTotalRecord(InputOrderModel inputModel)
    {
        var result = 0;

        // パラメータ初期化
        db!.ClearParam();

        var sql = new StringBuilder();

        sql.AppendLine("SELECT count(productName) AS cnt FROM t_order");
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
    public OrderModel[] GetList(InputOrderModel inputModel, int startIndex, int recordCount)
    {
        var result = new List<OrderModel>();

        // パラメータ初期化
        db!.ClearParam();

        var sql = new StringBuilder();

        sql.AppendLine("SELECT productName, unitPrice, qty,unitPrice*qty AS totalPrice, enabled FROM t_order");
        sql.Append(GetWhere(inputModel));
        sql.AppendLine("ORDER BY productName");
        sql.AppendLine(db!.GetLimitSQL(recordCount, startIndex));

        var sqlResult = db.Fill(sql.ToString());
        var noIndex = 1;
        foreach (DataRow row in sqlResult.Rows)
        {
            // 注文
            var no = startIndex + noIndex;
            var product = Parse<string>(row["productName"]);
            var unitPrice = Parse<decimal>(row["unitPrice"]);
            var qty = Parse<decimal>(row["qty"]);
            var totalPrice = unitPrice * qty;
            var enabled = Parse<bool>(row["enabled"]);
            result.Add(new OrderModel(no, product, unitPrice, qty, totalPrice, enabled, 1));

            noIndex++;

        }
        return [.. result];
    }

    /// <summary>
    /// 検索条件を取得
    /// </summary>
    /// <param name="inputModel">検索条件</param>
    /// <returns>検索条件文字列</returns>
    private string GetWhere(InputOrderModel inputModel)
    {
        if(db is null) return string.Empty;

        var where = new StringBuilder();
        if(!string.IsNullOrEmpty(inputModel.ProductName))
        {
            if (where.Length > 0) where.Append(" AND ");
            where.AppendLine(" productName like @ProductName");

            // Param設定
            db.AddParam("@ProductName", $"%{inputModel.ProductName}%");
        }
        if(inputModel.StartUnitPrice is not null)
        {
            if (where.Length > 0) where.Append(" AND ");
            where.AppendLine(" unitPrice >= @StartUnitPrice");

            // Param設定
            db.AddParam("@StartUnitPrice", inputModel.StartUnitPrice);
        }
        if(inputModel.EndUnitPrice is not null)
        {
            if (where.Length > 0) where.Append(" AND ");
            where.AppendLine(" unitPrice <= @EndUnitPrice");

            // Param設定
            db.AddParam("@EndUnitPrice", inputModel.EndUnitPrice);
        }
        if(inputModel.StartTotalPrice is not null)
        {
            if (where.Length > 0) where.Append(" AND ");
            where.AppendLine(" cast(unitPrice*qty AS numeric) >= @StartTotalPrice");

            // Param設定
            db.AddParam("@StartTotalPrice", inputModel.StartTotalPrice);
        }
        if(inputModel.EndTotalPrice is not null)
        {
            if (where.Length > 0) where.Append(" AND ");
            where.AppendLine(" cast(unitPrice*qty AS numeric) <= @EndTotalPrice");

            // Param設定
            db.AddParam("@EndTotalPrice", inputModel.EndTotalPrice);
        }
        if (where.Length > 0) return " WHERE " + where.ToString();

        return string.Empty;
    }
    #endregion

    #endregion

    #region 登録・更新

    /// <summary>
    /// 注文情報の登録
    /// </summary>
    /// <param name="target">登録対象</param>
    /// <param name="userId">ユーザーID</param>
    /// <param name="programId">プログラムID</param>
    /// <returns>成功/失敗</returns>
    public bool Save(OrderModel target, string userId, string programId)
    {
        var result = false;
        try
        {
            // トランザクション開始
            BeginTransaction();

            // DB存在確認
            var dbResult = Find(target.ProductName);
            if (dbResult.Count != 0)
            {
                result = Update(target, userId ?? string.Empty, programId);
            }
            else
            {
                result = Insert(target, userId ?? string.Empty, programId);
            }

            // 登録・更新結果でコミット・ロールバック
            if (result)
            {
                Commit();
            }
            else
            {
                Rollback();
            }

        }
        catch (Exception)
        {
            Rollback();
            throw;
        }
        return result;
    }

    /// <summary>
    /// 登録
    /// </summary>
    /// <param name="target">登録対象</param>
    /// <param name="userId">ユーザーID</param>
    /// <param name="programId">プログラムID</param>
    /// <returns>登録結果成功/失敗</returns>
    private bool Insert(OrderModel target, string userId, string programId)
    {
        var sql = new StringBuilder();
        sql.AppendLine("INSERT ");
        sql.AppendLine("INTO t_order(productName, unitPrice, qty, createDate,createUserId, createProgramId, updateDate, updateUserId, updateProgramId, enabled, version) ");
        sql.AppendLine("VALUES (@productName, @unitPrice, @qty, @date, @user, @program, @date, @user, @program, true, 1) ");

        // Param設定
        var date = DateTime.UtcNow.ToJstTime();
        db!.ClearParam();
        db.AddParam("@productName", target.ProductName);
        db.AddParam("@unitPrice", target.UnitPrice);
        db.AddParam("@qty", target.Qty);
        db.AddParam("@date", date.ToString());
        db.AddParam("@user", userId);
        db.AddParam("@program", programId);

        // SQL発行
        if (db.ExecuteNonQuery(sql.ToString()) == 1)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="target">更新対象</param>
    /// <param name="userId">ユーザーID</param>
    /// <param name="programId">プログラムID</param>
    /// <returns>更新結果成功/失敗</returns>
    private bool Update(OrderModel target, string userId, string programId)
    {
        var sql = new StringBuilder();
        sql.AppendLine("UPDATE t_order");
        sql.AppendLine("SET");
        sql.AppendLine("productName=@productName,");
        sql.AppendLine("unitPrice=@unitPrice,");
        sql.AppendLine("qty=@qty,");
        sql.AppendLine("updateDate=@date,");
        sql.AppendLine("updateUserId=@user,");
        sql.AppendLine("updateProgramId=@program,");
        sql.AppendLine("enabled=@enabled,");
        sql.AppendLine("version=version+1");
        sql.AppendLine("WHERE");
        sql.AppendLine("  productName = @productName");

        // Param設定
        var date = DateTime.UtcNow.ToJstTime();
        db!.ClearParam();
        db.AddParam("@productName", target.ProductName);
        db.AddParam("@unitPrice", target.UnitPrice);
        db.AddParam("@qty", target.Qty);
        db.AddParam("@date", date.ToString());
        db.AddParam("@user", userId);
        db.AddParam("@program", programId);
        db.AddParam("@enabled", target.Enabled);

        // SQL発行
        if (db.ExecuteNonQuery(sql.ToString()) == 1)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 注文情報リストの登録(バルクインサート)
    /// </summary>
    /// <param name="targets">登録対象リスト</param>
    /// <param name="userId">ユーザーID</param>
    /// <param name="programId">プログラムID</param>
    public bool Save(IReadOnlyList<OrderModel> targets, string userId, string programId)
    {
        var result = false;
        try
        {
            // トランザクション開始
            BeginTransaction();

            // SQL作成
            var sql = new StringBuilder();
            sql.AppendLine("INSERT INTO t_order(productName, unitPrice, qty, createDate,createUserId, createProgramId, updateDate, updateUserId, updateProgramId, enabled, version) VALUES ");
            sql.AppendLine("(@productName, @unitPrice, @qty, @date, @user, @program, @date, @user, @program, true, 1) ");
            var sqlString = sql.ToString();

            var date = DateTime.UtcNow.ToJstTime();

            // 登録処理
            var insertCount = 0;
            foreach (var target in targets)
            {
                // Param設定
                db!.ClearParam();
                db.AddParam("@productName", target.ProductName);
                db.AddParam("@unitPrice", target.UnitPrice);
                db.AddParam("@qty", target.Qty);
                db.AddParam("@date", date.ToString());
                db.AddParam("@user", userId);
                db.AddParam("@program", programId);

                if (db.ExecuteNonQuery(sql.ToString()) != 1)
                {
                    break;
                }
                insertCount++;
            }

            // 登録結果でコミット・ロールバック
            if (targets.Count == insertCount)
            {
                Commit();
                result = true;
            }
            else
            {
                Rollback();
            }

        }
        catch (Exception)
        {
            Rollback();
            throw;
        }
        return result;
    }
    #endregion
}
