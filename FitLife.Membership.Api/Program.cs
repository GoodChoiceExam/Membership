using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Web;

var logger = LogManager.Setup().LoadConfigurationFromFile("NLog.config").GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddControllers();
    
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "fitlife-identity",
                ValidateAudience = true,
                ValidAudience = builder.Configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeyResolver = (_, _, kid, _) =>
                {
                    using var client = new HttpClient();

                    var json = client.GetStringAsync(
                        builder.Configuration["Jwt:Authority"] + "/.well-known/jwks.json").Result;

                    var jwks = new JsonWebKeySet(json);

                    return jwks.GetSigningKeys();
                }
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
    
    app.MapGet("/auth-test", () => Results.Ok(new { message = "JWT virker" }))
        .RequireAuthorization();
    
    app.MapControllers();

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
