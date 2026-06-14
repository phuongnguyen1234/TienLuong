using Core;
using Core.Interfaces.Database;
using Core.Interfaces.Validators; // Import our custom validator interface
using Core.Middlewares;
using Core.Validators; // Import our custom validator implementations
using Infrastructure;
using Infrastructure.Database;
using Infrastructure.Repository;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        // Giữ nguyên PascalCase của C# khi trả về JSON (không chuyển sang camelCase)
        options.SerializerSettings.ContractResolver = new DefaultContractResolver();
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 1. Khai báo Policy CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") // Cho phép ứng dụng Vue.js chạy trên localhost:5173
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Nạp cấu hình DatabaseOptions từ appsettings.json
builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection("ConnectionStrings"));

// Cấu hình Dapper Mapping
BaseRepository<object>.ConfigureDapperMapping();

// DI
builder.Services.AddScoped<IDbConnectionFactory, ConnectionFactory>();
builder.Services.AddServiceDI();
builder.Services.AddInfrastructureDI();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Middlewares
app.UseMiddleware<GlobalExceptionMiddleware>();

// 2. Kích hoạt Middleware CORS (Lưu ý: Phải đặt TRƯỚC UseAuthorization)
app.UseCors("AllowVueApp");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
