using BlazorDownloadFile;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MS.wyyyy;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMasaBlazor();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://api.jirigalang.com") });
builder.Services.AddHttpClient<BlogService>();
builder.Services.AddHttpClient<WyySongList>();
builder.Services.AddBlazorDownloadFile(ServiceLifetime.Scoped);
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


//app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();

app.MapFallbackToPage("/_Host");

app.Run();