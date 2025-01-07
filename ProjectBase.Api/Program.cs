using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjectBase.Api.Extensions;
using ProjectBase.Api.Middleware;
using ProjectBase.Business.Abstract;
using ProjectBase.Core.Abstract;
using ProjectBase.Data.Concrete;
using ProjectBase.Data.Context;
using ProjectBase.Entity.Database;
using System.Diagnostics;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var cacheOptions = builder.Configuration.GetSection("CacheOptions").Get<CacheOptions>();
builder.Services.AddCacheServices(cacheOptions);

#region JWT
var jwtSettings = builder.Configuration.GetSection("JWT");
var environmentSettings = jwtSettings.GetSection("opt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"]);

var issuer = environmentSettings["ValidIssuer"];
var audience = environmentSettings["ValidAudience"];
var validateIssuer = bool.Parse(environmentSettings["ValidateIssuer"]);
var validateAudience = bool.Parse(environmentSettings["ValidateAudience"]);
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = validateIssuer, 
        ValidIssuer = jwtSettings["ValidIssuer"], 
        ValidateAudience = validateAudience, 
        ValidAudience = jwtSettings["ValidAudience"], 
        ValidateLifetime = true, 
        ValidateIssuerSigningKey = true, 
        IssuerSigningKey = new SymmetricSecurityKey(key) 
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Debug.WriteLine($"Authentication failed: {context.Exception?.Message}");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            return Task.CompletedTask;
        },

        OnChallenge = context =>
        {
            Debug.WriteLine("Authorization challenge triggered.");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Debug.WriteLine("Token validated successfully.");
            return Task.CompletedTask;
        }
    };
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;

    options.LoginPath = PathString.Empty;
    options.AccessDeniedPath = PathString.Empty;
    options.Events.OnRedirectToLogin = context =>
    {
        Debug.WriteLine("Redirect to login prevented. Returning 401.");
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };

    options.Events.OnRedirectToAccessDenied = context =>
    {
        Debug.WriteLine("Redirect to access denied prevented. Returning 403.");
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
});
builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .Build();
});
#endregion

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));

builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
}).AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();


var assemblies = new[]
{
    typeof(ILocalizationService).Assembly,
    typeof(ILanguageRepository).Assembly, 
    Assembly.GetExecutingAssembly()
};

builder.Services.AddServicesFromAttributes(assemblies);

var app = builder.Build();

app.Use(async (context, next) =>
{
    var language = context.Request.Headers["Accept-Language"].ToString().Split('-').FirstOrDefault() ?? "en";
    if (!string.IsNullOrEmpty(language))
    {
        var localizationService = context.RequestServices.GetService<ILocalizationService>();
        localizationService.SetCulture(language);
    }
await next();
});

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseMiddleware<LoggerMiddleware>();
app.UseMiddleware<ExceptionHandlerMiddleware>();
app.Run();
