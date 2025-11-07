namespace WebApp.DI;
using WebApp.Repositories;
using WebApp.Repositories.Interfaces;

/// <summary>
/// DIクラス
/// </summary>
public partial class DIClass
{
    public void SetUserRepository(IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
    }
}