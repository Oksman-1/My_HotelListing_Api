using AspNetCoreRateLimit;
using AutoMapper;
using HotelListing.Configurations;
using HotelListing.Extensions;
using HotelListing.Repository.GenericRepository;
using HotelListing.Repository.RepositoryContracts;
using HotelListing.Services;
using Serilog;
using Serilog.Core;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(config =>
{
	config.CacheProfiles.Add("120SecondsDuration", new Microsoft.AspNetCore.Mvc.CacheProfile
	{
		Duration = 120
	});
}).AddNewtonsoftJson(op =>
   op.SerializerSettings.ReferenceLoopHandling = 
      Newtonsoft.Json.ReferenceLoopHandling.Ignore);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSerilog();
builder.Services.ConfigureCors();
builder.Services.ConfigureDbContext(builder.Configuration);
builder.Services.AddAutoMapper(typeof(MapperInitializer));
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddAuthentication();	
builder.Services.ConfigureIdentity();
builder.Services.ConfigureJWT(builder.Configuration);
builder.Services.AddScoped<IAuthManager, AuthManager>();
builder.Services.ConfigureVersioning();
builder.Services.AddResponseCaching();
builder.Services.ConfigureHttpCacheHeaders();
builder.Services.AddMemoryCache();
builder.Services.ConfigureRateLimiting();
builder.Services.AddHttpContextAccessor();	



//Configuring Serilog
Log.Logger = new LoggerConfiguration()
	.WriteTo.File(path: "C:\\Users\\Oks\\CODEBASE\\My_JS\\C#\\C# PROJECTS\\ISLANDman\\My_HotelListing\\My_HotelListing\\bin\\Debug\\net6.0\\Logs\\log-.txt",
				  outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
				  rollingInterval: RollingInterval.Day,
				  restrictedToMinimumLevel:LogEventLevel.Information
	).CreateLogger();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json","HotelListing v1"));

app.ConfigureExceptionHandler();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseResponseCaching();
app.UseHttpCacheHeaders();
app.UseIpRateLimiting();	
app.UseRouting();	
app.UseAuthentication();
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


