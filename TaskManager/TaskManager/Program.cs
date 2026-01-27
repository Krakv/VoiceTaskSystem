using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using TaskManager.Application.Common.DTOs.Responses;
using TaskManager.Application.Domain.Entities;
using TaskManager.Application.Services;
using TaskManager.Application.Services.Interfaces;
using TaskManager.Config;
using TaskManager.Infrastructure.Repository;
using TaskManager.Middleware;
using TaskManager.Pipeline;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

#region MediatR

builder.Services.AddMediatR(cf => cf.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

#endregion MediatR

#region Services

builder.Services.AddHttpClient();
builder.Services.AddScoped<ISpeechProcessingClient, SpeechProcessingClient>();
builder.Services.AddScoped<IIntentDispatcher, IntentDispatcher>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
builder.Services.AddScoped<ICurrentUser>(sp =>
    sp.GetRequiredService<ICurrentUserProvider>().GetCurrentUser());

#endregion Services

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ContentTypeValidationFilter>();
});

builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddOpenApi();

#region Config

builder.Services.Configure<TelegramBotConfig>(builder.Configuration.GetSection("TelegramBot"));
builder.Services.Configure<SpeechProcessingConfig>(builder.Configuration.GetSection("SpeechProcessingConfig"));

#endregion Config

# region Auth

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettings);
var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"] ?? throw new ArgumentException("JWT_SECRET не содержит значения"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

#endregion Auth

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

# region Logging

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog((ctx, services, cfg) =>
{
    cfg
    .Enrich.FromLogContext()
    .Enrich.WithCorrelationId()
    .WriteTo.Console();
});

# endregion Logging

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Scoped);

# region Swagger

// Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Voice Task API",
        Description = "API для работы с задачами через голосовые команды и ручное управление",
        Contact = new OpenApiContact { Name = "VTS", Email = "abliten@mail.ru" }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Description = "Введите JWT токен: Bearer {token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference{ Type = ReferenceType.SecurityScheme, Id = "Bearer"} },
            Array.Empty<string>()
        }
    });

    //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    //if (File.Exists(xmlPath))
    //    c.IncludeXmlComments(xmlPath);
});

#endregion Swagger

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    await dbContext.Database.MigrateAsync();
}

app.Use(async (context, next) =>
{
    Log.Information(
    "Incoming request {Method} {Path}",
    context.Request.Method,
    context.Request.Path
    );
    await next();
});

app.UseMiddleware<HttpErrorHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Voice Task API V1");
        c.RoutePrefix = string.Empty;
    });
}

await app.RunAsync();