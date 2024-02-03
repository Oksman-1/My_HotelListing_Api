using AspNetCoreRateLimit;
using HotelListing.Data;
using HotelListing.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;


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

	public static void ConfigureIdentity(this IServiceCollection services)
	{
		var builder = services.AddIdentityCore<ApiUser>(o => o.User.RequireUniqueEmail = true);

		builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), services);
		builder.AddEntityFrameworkStores<DatabaseContext>().AddDefaultTokenProviders(); 
	}

	public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
	{
		var jwtSettings = configuration.GetSection("Jwt");
		var key = Environment.GetEnvironmentVariable("KEY");

		services.AddAuthentication(o =>
		{
			o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

		})
			.AddJwtBearer(o =>
			{
				o.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = jwtSettings.GetSection("Issuer").Value,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
					ValidAudience = jwtSettings.GetSection("validAudience").Value
				};
			});
	}


	public static void ConfigureExceptionHandler(this IApplicationBuilder app)
	{
		app.UseExceptionHandler(error =>
		{
			error.Run(async context =>
			{
                 context.Response.StatusCode = StatusCodes.Status500InternalServerError;
				 context.Response.ContentType = "application/json";	

				var contextFeature = context.Features.Get<IExceptionHandlerFeature>();  
				if(contextFeature != null) 
				{
					Log.Error($"Something Went Wrong in the {contextFeature.Error}");

					await context.Response.WriteAsync(new Error
					{
						StatusCode = context.Response.StatusCode,
						Message = "Internal Server Error. Please Try Again Later.",
					}.ToString());
				}
			});
		});
	}


	public static void ConfigureVersioning(this IServiceCollection services)
	{
		services.AddApiVersioning(opt =>
		{
			opt.ReportApiVersions = true;
			opt.AssumeDefaultVersionWhenUnspecified = true;
			opt.DefaultApiVersion = new ApiVersion(1, 0);
			opt.ApiVersionReader = new HeaderApiVersionReader("api-version");
		});
	}

	public static void ConfigureHttpCacheHeaders(this IServiceCollection services)
	{
		services.AddResponseCaching();
		services.AddHttpCacheHeaders((expirationOpt) =>
		{
			expirationOpt.MaxAge = 65;
			expirationOpt.CacheLocation = Marvin.Cache.Headers.CacheLocation.Private;
		},
		(validationOpt) =>
		{
			validationOpt.MustRevalidate = true;	
		});
	}

	public static void ConfigureRateLimiting(this IServiceCollection services)
	{
		var rateLimitRules = new List<RateLimitRule>
		{
			new RateLimitRule
			{
				Endpoint = "*",
				Limit = 1,
				Period = "5s"
			}

		};
		services.Configure<IpRateLimitOptions>(opt =>
		{
			opt.GeneralRules = rateLimitRules;
		});
		services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
		services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
		services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
		services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
	}

}
