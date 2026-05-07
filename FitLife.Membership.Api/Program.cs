using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
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

    var jwksUrl = "http://localhost:5244/.well-known/jwks.json";

    var jwksJson = new HttpClient()
        .GetStringAsync(jwksUrl)
        .GetAwaiter()
        .GetResult();

    var jwks = new JsonWebKeySet(jwksJson);

    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "fitlife-identity",

                ValidateAudience = true,
                ValidAudience = "fitlife",

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = jwks.Keys
            };
        });

    builder.Services.AddAuthorization();

    var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

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