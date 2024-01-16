using HotelListing.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HotelListing.Extensions;

public static class ServiceExtensions
{
	public static void ConfigureCors(this IServiceCollection services) =>
		services.AddCors(o => 
		{
			o.AddPolicy("AllowAll", builder =>
			builder.AllowAnyOrigin()
			.AllowAnyMethod()
			.AllowAnyHeader());
		});

	public static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration) =>
		services.AddDbContext<DatabaseContext>(options =>
		{
			options.UseSqlServer(configuration.GetConnectionString("sqlConnection"));
		});


		
}
