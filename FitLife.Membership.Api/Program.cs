using NLog;
using NLog.Web;

var logger = NLog.LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseHttpsRedirection();

    app.MapGet("/healthz", () => Results.Ok(new { status = "healthy" }));
    
    

    app.Run();
}
catch (Exception e)
{
    logger.Fatal(e, "MembershipService failed to start");
    throw;
}
finally
{
    LogManager.Shutdown();
}
