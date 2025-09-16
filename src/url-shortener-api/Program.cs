using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using Url.Shortener.Api.Middleware;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;
using Url.Shortener.Api.Data;

const string applicationName = "Url Shortener API";

Activity.DefaultIdFormat = ActivityIdFormat.W3C;

InitializeBootUpLogger();

try
{
    Log.Logger.Information("Main - Init {ApplicationName}", applicationName);
    Log.Logger.Information("Executing in user context: {User}", Environment.UserName);

    var builder = WebApplication.CreateBuilder(new WebApplicationOptions
    {
        Args = args
    });

    // Add service defaults & Aspire client integrations.
    _ = builder.AddServiceDefaults();

    // Add Seq endpoint pointing to the Aspire/config reference.
    builder.AddSeqEndpoint("SeqServer", static settings =>
    {
        settings.DisableHealthChecks = true;
    });

    // Tell the host to use serilog, required to properly connect the logger to serilog.
    _ = builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

    var assembly = Assembly.GetExecutingAssembly();
    var assemblyName = assembly.GetName();
    var version = assemblyName.Version;
    var buildDate = new FileInfo(Assembly.GetExecutingAssembly().Location).LastWriteTime;

    _ = builder.Services.AddSerilog((services, loggerConfig) =>
            loggerConfig
                .ReadFrom.Configuration(builder.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Version", version)
                .Enrich.WithProperty("BuildDate", buildDate.ToString("f", CultureInfo.InvariantCulture))
        );

    // Add the DB Config
    builder.AddNpgsqlDbContext<UrlShortenerDbContext>(connectionName: "urlshortenerdb");
    
    _ = builder.Services
        .AddFastEndpoints()
        .SwaggerDocument();

    // Add Logging to all HTTP requests.
    _ = builder.Services.AddTransient<LoggingDelegatingHandler>();

    await using var app = builder.Build();

    app.Logger.LogInformation("App created...");

    _ = app
        .UseSerilogRequestLogging()
        .UseFastEndpoints(c =>
        {
            c.Errors.UseProblemDetails();
        });

    if (app.Environment.IsDevelopment())
    {
        _ = app.UseOpenApi(c => c.Path = "/openapi/{documentName}.json");
        _ = app.MapScalarApiReference(options =>
        {
            _ = options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.RestSharp);
        });
    }
    else
    {
        _ = app.UseHttpsRedirection();
    }

    await app.RunAsync();

}
catch (Exception ex)
{
    // Catch errors in project setup that result in an unusable system.
    Log.Logger.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.Logger.Information("Main - Shutdown {ApplicationName}", applicationName);

    // Ensure to flush and stop internal timers/threads before application-exit
    // (Avoid segmentation fault on Linux).
    Log.CloseAndFlush();
}


static void InitializeBootUpLogger() =>
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .WriteTo.Console(new Serilog.Formatting.Compact.RenderedCompactJsonFormatter())
        .CreateLogger();