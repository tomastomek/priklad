using Zufanci.Client.Pages;
using Zufanci.Server.Components;
using Syncfusion.Blazor;
using Zufanci.Client.Helpers;
using Zufanci.Server;
using Microsoft.EntityFrameworkCore;
using Zufanci.Client.Repository.IRepository;
using Zufanci.Server.Repository;
using Zufanci.Server.Service;
using System.Globalization;
using Zufanci.Server.Helpers;
using Microsoft.AspNetCore.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("");

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSignalR();
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")).EnableSensitiveDataLogging(true));
builder.Services.AddSyncfusionBlazor();
builder.Services.AddScoped<IFileUpload, FileUpload>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepositoryServer>();
builder.Services.AddScoped<IImageRepository, ImageRepositoryServer>();
builder.Services.AddScoped<IProductPriceRepository, ProductPriceRepositoryServer>();
builder.Services.AddScoped<IProductRepository, ProductRepositoryServer>();
builder.Services.AddScoped<IShopRepository, ShopRepositoryServer>();
builder.Services.AddScoped<IUnitRepository, UnitRepositoryServer>();
builder.Services.AddScoped<IMonitoringService, MonitoringService>();
builder.Services.AddScoped<IMonitoringDetailsService, MonitoringDetailsService>();
builder.Services.AddScoped<IStringParserService, StringParserService>();
builder.Services.AddScoped<ImageService>();
builder.Services.AddSingleton(typeof(ISyncfusionStringLocalizer), typeof(SyncfusionLocalizer));
var emailSettings = builder.Configuration.GetSection("EmailSettings");
builder.Services.Configure<EmailSettings>(emailSettings);
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("cs-CZ");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("cs-CZ");
// if we use API we need to add this, otherwise it won't find endpoints and display 404
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Zufanci.Client._Imports).Assembly);

// Map SignalR hub
app.MapHub<MonitoringChangeHub>("/monitoringchangehub");

// if we use API we need to add this, otherwise it won't find endpoints and display 404
app.MapControllers();

app.Run();
