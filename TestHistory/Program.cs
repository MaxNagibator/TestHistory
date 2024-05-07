using NLog.Web;
using TestHistory.Business;
using TestHistory.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHostedService<TestResultParserService>();
builder.Services.AddSingleton<TestResultKeeper>();
builder.Services.AddSingleton<GitlabService>();

builder.Host.UseNLog(new NLogAspNetCoreOptions()
{
    LoggingConfigurationSectionName = "NLog"
});

var app = builder.Build();

Globals.Settings = new Settings
{
    DataPath = builder.Configuration["DataPath"],
    GitlabAccessToken = builder.Configuration["GitlabAccessToken"],
};
Globals.Settings.Init();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
