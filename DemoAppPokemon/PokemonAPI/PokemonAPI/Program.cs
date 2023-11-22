using Microsoft.EntityFrameworkCore;
using PokemonAPI.Data;
using PokemonAPI.Extensions;
using PokemonAPI.Models;
using PokemonAPI.Repository;
using PTT_DDS_API.Extensions;
using PTT_DDS_API.Settings;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddCors(options => options.AddPolicy(name: "PokenmonOrigins",
    policy =>
    {
        policy.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader();
    }));
//string dbconnection = @"server=" + SqlServer.HostName + ";database=" + SqlServer.DbName + ";;User Id=" + user + ";Password=" + pass + ";"; 
//string secureDbConnection = @"server=localhost;database=DdsDb;User Id=sa;Password=1234;Trusted_Connection=True;TrustServerCertificate=True;";
string secureDbConnection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(secureDbConnection));
// Cors
//var HostSettings = builder.Configuration.GetSection("HostSettings").Get<HostSettings>();
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("CorsPolicy", builder => builder
//          .WithOrigins(HostSettings.FEUrl, HostSettings.BEUrl)
//          .WithMethods("GET", "POST", "PUT", "DELETE", "PATCH")
//          .AllowAnyHeader());
//});




// Add Service
builder.Services.AddScoped<JwtHandler>();
builder.Services.AddTransient<DapperContext>(sp => new DapperContext(secureDbConnection));
builder.Services.AddRepository();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHealthChecks();

// Add services to the container. 
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
var app = builder.Build();
app.UseCors("PokenmonOrigins");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();



Log.Information("Starting web host");
//Log.Information("Env:" + env.EnvironmentName);
//Log.Information("Cors" + "||FE:" + HostSettings.FEUrl + "||BE:" + HostSettings.BEUrl);
app.Run();
