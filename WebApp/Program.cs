using Microsoft.AspNetCore.StaticFiles;
using NLog.Extensions.Logging;
using NLog.Web;
using WebApp.Components;
using WebApp.DBAccess;
using WebApp.DI;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace WebApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // フォントリゾルバーのグローバル登録
        PdfReport.Utilites.Utility.SetJapaneseFontResolver();

        // ログ
        builder.Logging.ClearProviders();
        builder.Logging.SetMinimumLevel(LogLevel.Trace);
        builder.Logging.AddNLog(new NLogProviderOptions {
                CaptureMessageTemplates = true,
                CaptureMessageProperties = true
            });
        builder.Host.UseNLog();        

        // Add services to the container.
        //builder.Services.AddRazorComponents();
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        // DI設定
        DIClass.SetDI(builder.Services);
        
        // Configを専用Modelに設定
        var dbRoot = builder.Configuration.GetSection("DB");
        var setting = builder.Configuration.GetSection("Settings");

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
        builder.Services.Configure<SettingConfigModel>(setting);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/error");
        }

        app.UseStaticFiles();
        app.UseAntiforgery();

        // pdf.jsのftlが取得できるように設定
        var provider = new FileExtensionContentTypeProvider();
        provider.Mappings[".ftl"] = "text/plain";
        provider.Mappings[".dat.gz"] = "text/plain";
        app.UseStaticFiles(new StaticFileOptions
        {
            ContentTypeProvider = provider
        });

        //app.MapRazorComponents<App>();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}