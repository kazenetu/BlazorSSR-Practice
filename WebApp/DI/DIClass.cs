namespace WebApp.DI;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// DIクラス
/// </summary>
public partial class DIClass
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    private DIClass()
    {
    }

    /// <summary>
    /// DI設定
    /// </summary>
    /// <param name="services">DI設定用サービス</param>
    public static void SetDI(IServiceCollection services)
    {
        var di = new DIClass();
        var typeInfo = di.GetType();
        foreach (var item in typeInfo.GetMethods())
        {
            if(item.IsStatic || item.ReturnType != typeof(void))
                continue;

            Console.WriteLine($"{item.Name} {item.MemberType} {item.ReturnType} {item.ReturnParameter}");
            item.Invoke(di, [services]);
        }
    }
}