//using BepKhoiBackend.BusinessObject.Interfaces;
using BepKhoiBackend.BusinessObject.Services.LoginService;
using BepKhoiBackend.DataAccess.Abstract.MenuAbstract;
using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.DataAccess.Repository.LoginRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using BepKhoiBackend.API.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using BepKhoiBackend.BusinessObject.Mappings;
using BepKhoiBackend.BusinessObject.Services.InvoiceService;
using BepKhoiBackend.BusinessObject.Services;
using BepKhoiBackend.API.Hubs;


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);




// Scpace to call function
LoggingConfig.ConfigureLogging();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddAutoMapper(typeof(MenuProfile).Assembly);
builder.Services.AddAutoMapper(typeof(UnitProfile).Assembly);
builder.Services.AddAutoMapper(typeof(ProductCategoryProfile).Assembly);
builder.Services.AddAutoMapper(typeof(OrderMappingProfile));

// Config of logger
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Host.UseSerilog();

//// Add JWT Authentication (custom config)
//builder.Services.AddJwtAuthentication(builder.Configuration);

// Config Authentication Jwt
JwtConfig.ConfigureJwtAuthentication(builder.Services, builder.Configuration);
JwtConfig.ConfigureSwagger(builder.Services);
// Add Application Services (custom config DI)
builder.Services.AddApplicationServices(builder.Configuration);

//session
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddHttpContextAccessor();


//pdf print
builder.Services.AddScoped<PrintInvoicePdfService>();
builder.Services.AddScoped<PrintOrderPdfService>();
//VnPay
builder.Services.AddScoped<VnPayService>();


builder.Services.AddAuthorization();

builder.Services.AddCorsPolicy(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Quan trọng cho SignalR
    });
});

builder.Services.AddSignalR();

var app = builder.Build();

app.MapHub<SignalrHub>("/SignalrHub"); // Đăng ký đường dẫn của Hub

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseMiddleware<ExceptionMiddleware>(); // Use to solve problemss
app.UseSession();

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
