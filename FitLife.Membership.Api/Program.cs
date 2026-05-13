using System.Text.Json.Serialization;
using FitLife.Membership.Api.Repositories;
using FitLife.Membership.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Web;

var logger = NLog.LogManager.Setup().LoadConfigurationFromFile("NLog.config").GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();
    
    builder.Services
        .AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    
    builder.Services.AddScoped<IMemberService, MemberService>();
    builder.Services.AddScoped<IMemberRepository, MongoMemberRepository>();

    var jwksUrl = builder.Configuration["Jwt:JwksUrl"]!;

    var jwksJson = new HttpClient()
        .GetStringAsync(jwksUrl)
        .GetAwaiter()
        .GetResult();

    var jwks = new JsonWebKeySet(jwksJson);

    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.MapInboundClaims = false;
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