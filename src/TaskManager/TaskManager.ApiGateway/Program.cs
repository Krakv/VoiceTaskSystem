using FluentValidation;
using Hangfire;
using Hangfire.PostgreSql;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Prometheus;
using Serilog;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using TaskManager.ApiGateway.Middleware;
using TaskManager.Auth.Application.Features.Auth.Login;
using TaskManager.Auth.Application.Interfaces;
using TaskManager.Auth.Application.Services;
using TaskManager.Auth.Config;
using TaskManager.Auth.Infrastructure;
using TaskManager.Calendar.Application.Features.CalendarEvent.CreateCalendarEvent;
using TaskManager.Calendar.Application.Interfaces;
using TaskManager.Calendar.Application.Services;
using TaskManager.Calendar.Infrastructure;
using TaskManager.Calendar.Infrastructure.Interfaces;
using TaskManager.Notifications.Application.Features.NotificationFeature.CreateNotification;
using TaskManager.Notifications.Application.Services;
using TaskManager.Notifications.Application.Services.Factories;
using TaskManager.Notifications.Application.Services.Interfaces;
using TaskManager.Notifications.Config;
using TaskManager.Notifications.Pipeline;
using TaskManager.Repository.Context;
using TaskManager.RulesEngine.Application.Features.RuleFeature.CreateRule;
using TaskManager.RulesEngine.Application.Interfaces;
using TaskManager.RulesEngine.Application.Services;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Interfaces;
using TaskManager.Shared.Pipeline;
using TaskManager.TaskManagement.Application.Features.CommandRequestFeature.CreateVoiceTask;
using TaskManager.TaskManagement.Application.Features.TaskFeature.CreateTask;
using TaskManager.TaskManagement.Application.Services;
using TaskManager.TaskManagement.Application.Services.Interfaces;
using TaskManager.TaskManagement.Application.Services.VoiceProcessing;
using TaskManager.TaskManagement.Config;
using TaskManager.TaskManagement.Pipeline;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

#region Config

builder.Services.Configure<TelegramBotConfig>(builder.Configuration.GetSection("TelegramBot"));
builder.Services.Configure<SpeechProcessingConfig>(builder.Configuration.GetSection("SpeechProcessingConfig"));
builder.Services.Configure<YandexOAuthConfig>(builder.Configuration.GetSection("YandexOAuth"));
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));
builder.Services.Configure<FrontendOptions>(builder.Configuration.GetSection("Frontend"));

builder.Services.Configure<JsonSerializerOptions>(options =>
{
    options.PropertyNameCaseInsensitive = true;
});

#endregion Config

#region MediatR

builder.Services.AddMediatR(cf => cf.RegisterServicesFromAssemblies(
    typeof(Program).Assembly, 
    typeof(CreateTaskCommand).Assembly, 
    typeof(LoginCommand).Assembly,
    typeof(CreateRuleCommand).Assembly,
    typeof(CreateCalendarEventCommand).Assembly,
    typeof(CreateNotificationCommand).Assembly
    ));

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TaskAccessBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(NotificationAccessBehavior<,>));
builder.Services.AddValidatorsFromAssemblies(
    [
        typeof(Program).Assembly, 
        typeof(CreateTaskCommandValidator).Assembly,
        typeof(CreateNotificationCommandValidator).Assembly,
        typeof(CreateCalendarEventCommandValidator).Assembly,
        typeof(CreateVoiceTaskValidator).Assembly
    ]
    );

#endregion MediatR

#region Services

builder.Services.AddSingleton<ITelegramBotClient>(sp =>
{
    var config = sp.GetRequiredService<IOptions<TelegramBotConfig>>().Value;
    return new TelegramBotClient(config.AuthToken);
});
builder.Services.AddHostedService<TelegramBotService>();
builder.Services.AddSingleton<TelegramBotAdapter>();
builder.Services.AddSingleton<IBotService>(sp =>
    new RetryBotService(
        sp.GetRequiredService<TelegramBotAdapter>(),
        sp.GetRequiredService<ILogger<RetryBotService>>()
    ));

builder.Services.AddHttpClient();
builder.Services.AddScoped<ISpeechProcessingClient, SpeechProcessingClient>();

if (builder.Environment.IsDevelopment())
    builder.Services.AddSingleton<EmailServiceFactory, FakeEmailServiceFactory>();
else
    builder.Services.AddSingleton<EmailServiceFactory, SmtpEmailServiceFactory>();
builder.Services.AddScoped<INotificationSender, EmailNotificationSender>();
builder.Services.AddScoped<INotificationSender, TelegramNotificationSender>();
builder.Services.AddScoped<INotificationsProcessingService, NotificationsProcessingService>();
builder.Services.AddSingleton<IEmailService>(sp => sp.GetRequiredService<EmailServiceFactory>().CreateEmailService());

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
builder.Services.AddScoped<ITelegramContextAccessor, TelegramContextAccessor>();
builder.Services.AddScoped<ICurrentUser>(sp => sp.GetRequiredService<ICurrentUserProvider>().GetCurrentUser());

builder.Services.AddSingleton<IRuleValidator, RuleValidator>();
builder.Services.AddSingleton<IRuleSchemaValidator, RuleSchemaValidator>();
builder.Services.AddSingleton<IRuleDomainValidator, RuleDomainValidator>();
builder.Services.AddSingleton<IRulePolicyValidator, RulePolicyValidator>();
builder.Services.AddScoped<IRuleApplier, RuleApplier>();
builder.Services.AddScoped<IRuleEvaluator, RuleEvaluator>();
builder.Services.AddScoped<IRuleActionExecutor, RuleActionExecutor>();
builder.Services.AddScoped<IRuleRepository, RuleRepository>();
builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
builder.Services.AddHostedService<VoiceProcessingWorker>();
builder.Services.AddScoped<VoiceProcessingHandler>();

builder.Services.AddDataProtection();

builder.Services.AddSingleton<IStateService, OAuthStateService>();
builder.Services.AddScoped<YandexOAuthClient>();
builder.Services.AddScoped<ExternalCalendarAccountService>();
builder.Services.AddScoped<ICalDavClient, CalDavClient>();
builder.Services.AddScoped<ICalendarIcsGenerator, CalendarIcsGenerator>();
builder.Services.AddScoped<ICalendarSyncService, YandexCalendarSyncService>();

#endregion Services

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ContentTypeValidationFilter>();
    options.Filters.Add<AutoValidationFilter>();
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
}); 
builder.Services.AddScoped<AutoValidationFilter>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

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

builder.Services.AddHangfire(config =>
{
    config.UsePostgreSqlStorage(options =>
    {
        options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"));
    });
});

builder.Services.AddHangfireServer();

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

# region Logging

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

builder.Host.UseSerilog((ctx, services, cfg) =>
{
    cfg.ReadFrom.Configuration(ctx.Configuration);
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

    var sw = Stopwatch.StartNew();
    await next(context);
    sw.Stop();

    Log.Information(
        "Request {Method} {Path} completed in {ElapsedMs}ms", 
        context.Request.Method, 
        context.Request.Path, 
        sw.ElapsedMilliseconds
        );
});

app.UseMiddleware<HttpErrorHandlingMiddleware>();


app.UseHttpsRedirection();
app.UseRouting();

app.UseHttpMetrics();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapMetrics();

app.UseHangfireDashboard();

RecurringJob.AddOrUpdate<INotificationsProcessingService>("notification-processing-service",
    x => x.ProcessNotificationsAsync(),
    "*/10 * * * * *");

var swaggerPrefix = builder.Configuration["SwaggerPrefix"] ?? "";
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint($"{swaggerPrefix}/swagger/v1/swagger.json", "Voice Task API V1");
    c.RoutePrefix = string.Empty;
});

await app.RunAsync();