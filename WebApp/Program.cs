using WebApp.Components;
using WebApp.DBAccess;
using WebApp.Repositories;
using WebApp.Repositories.Interfaces;

namespace WebApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        //builder.Services.AddRazorComponents();
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        // DI設定
        builder.Services.AddTransient<IOrderRepository, OrderRepository>();

        // Configを専用Modelに設定
        var dbRoot = builder.Configuration.GetSection("DB");
        var dbSetting = builder.Configuration.GetSection("Setting");

        #if DEBUG
            var dbConnectionStrings = dbRoot.GetSection("ConnectionStrings");
            var dbTarget = dbRoot.GetSection("Target");
            if (dbTarget.Value is null)
            {
                // デバッグ時のみ未設定の場合はSQLiteを選択
                dbConnectionStrings.GetSection("sqlite").Value = "assets/Test.db";
                dbTarget.Value = "sqlite";
            }
        #endif

        builder.Services.Configure<DatabaseConfigModel>(dbRoot);
        builder.Services.Configure<SettingConfigModel>(dbSetting);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseStaticFiles();
        app.UseAntiforgery();

        //app.MapRazorComponents<App>();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}