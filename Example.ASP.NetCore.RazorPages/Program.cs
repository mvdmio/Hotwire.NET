using Example.ASP.NetCore.RazorPages.Pages.TurboBroadcasts;
using mvdmio.Hotwire.NET.ASP.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTurboStreamsBroadcasting();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddHostedService<BroadcastBackgroundTask>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
   app.UseExceptionHandler("/Error");

   // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
   app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseTurboStreamsBroadcasting();
app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();