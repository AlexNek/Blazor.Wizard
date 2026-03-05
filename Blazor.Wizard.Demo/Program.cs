using System.Diagnostics;

using Blazor.Wizard.Demo;
using Blazor.Wizard.Demo.Services.Animation;
using Blazor.Wizard.Demo.Services.Toaster;

using Serilog;
using Serilog.Debugging;

SelfLog.Enable(msg => Trace.WriteLine($"SERILOG DEBUG: {msg}"));

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("serilog.json", optional: false, reloadOnChange: true);

// Configure Serilog from serilog.json
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Host.UseSerilog();

Log.Information("***Application starting...***");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddScoped<IToasterService, ToasterService>();
builder.Services.AddScoped<IWizardAnimationService, WizardAnimationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

Log.Information("Application started successfully");
app.Run();
