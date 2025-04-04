namespace WebApp.DI;
using WebApp.Repositories;
using WebApp.Repositories.Interfaces;

/// <summary>
/// DIクラス
/// </summary>
public partial class DIClass
{
    public void SetPostMasterRepository(IServiceCollection services)
    {
        services.AddTransient<IPostMasterRepository, PostMasterRepository>();
    }
}