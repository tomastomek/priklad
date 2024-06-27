using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Syncfusion.Blazor;
using System.Globalization;
using Zufanci.Client.Helpers;
using Zufanci.Client.Repository.IRepository;
using Zufanci.Client.Repository;
using Microsoft.AspNetCore.Components;

Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("");

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddSyncfusionBlazor();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IHttpService, HttpService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepositoryClient>();
builder.Services.AddScoped<IImageRepository, ImageRepositoryClient>();
builder.Services.AddScoped<IProductPriceRepository, ProductPriceRepositoryClient>();
builder.Services.AddScoped<IProductRepository, ProductRepositoryClient>();
builder.Services.AddScoped<IShopRepository, ShopRepositoryClient>();
builder.Services.AddScoped<IUnitRepository, UnitRepositoryClient>();
builder.Services.AddSingleton(typeof(ISyncfusionStringLocalizer), typeof(SyncfusionLocalizer));
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("cs-CZ");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("cs-CZ");

await builder.Build().RunAsync();
