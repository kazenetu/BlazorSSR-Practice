using Microsoft.Extensions.Options;
using System.Data;
using System.Text;
using WebApp.DBAccess;
using WebApp.Models;
using WebApp.Repositories.Interfaces;

namespace WebApp.Repositories
{
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
                result.Add(new OrderModel(0, product, 0, 0, totalPrice, 0));
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
                var product = Parse<string>(row["productName"]);
                var unitPrice = Parse<decimal>(row["unitPrice"]);
                var qty = Parse<decimal>(row["qty"]);
                var totalPrice = unitPrice * qty;
                var version = Parse<int>(row["version"]);
                result.Add(new OrderModel(no, product, unitPrice, qty, totalPrice, version));
            }
            return result;
        }

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
            sql.AppendLine("INTO t_order(productName, unitPrice, qty, createDate,createUserId, createProgramId, updateDate, updateUserId, updateProgramId, version) ");
            sql.AppendLine("VALUES (@productName, @unitPrice, @qty, @date, @user, @program, @date, @user, @program, 1) ");

            // Param設定
            var date = DateTime.Now;
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
            sql.AppendLine("version=version+1");
            sql.AppendLine("WHERE");
            sql.AppendLine("  productName = @productName");

            // Param設定
            var date = DateTime.Now;
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

        #endregion
    }
}
