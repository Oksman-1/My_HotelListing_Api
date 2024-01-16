using AutoMapper;
using HotelListing.Configurations;
using HotelListing.Extensions;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSerilog();
builder.Services.ConfigureCors();
builder.Services.ConfigureDbContext(builder.Configuration);
builder.Services.AddAutoMapper(typeof(MapperInitializer));

Log.Logger = new LoggerConfiguration()
	.WriteTo.File(path: "C:\\Users\\Oks\\CODEBASE\\My_JS\\C#\\C# PROJECTS\\ISLANDman\\HotelListing\\HotelListing\\bin\\Debug\\net7.0\\logs\\log-.txt",
				  outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
				  rollingInterval: RollingInterval.Day,
				  restrictedToMinimumLevel:LogEventLevel.Information
	).CreateLogger();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");	

app.UseAuthorization();

app.MapControllers();


try
{
	Log.Information("Application is Starting");

	app.Run();
}
catch (Exception ex)
{

	Log.Fatal(ex, "Application failed to start!!");
}
finally
{
  Log.CloseAndFlush();	
}


