using System.Text;

using DNTYD.Core.Options;
using DNTYD.Core.Services;
using DNTYD.Core.Services.Identity;
using DNTYD.Core.Services.Tracking;
using DNTYD.Infrastructure.Database;
using DNTYD.Infrastructure.Services;
using DNTYD.Infrastructure.Services.Identity;
using DNTYD.Infrastructure.Services.Tracking;
using DNTYD.WebAPI.HostedServices;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace DNTYD.WebAPI;

public static class Program {
	public static void Main(string[] args) {
		var builder = WebApplication.CreateBuilder(args);
		Program.ConfigureServices(builder);
		var app = builder.Build();
		Program.ConfigurePipeline(app);
		app.Run();
	}

	private static void ConfigureServices(WebApplicationBuilder builder) {
		// Add services to the container.
		builder.Services.AddControllers();
		
		Program.ConfigureOptions(builder, out JwtIssuingOptions jwtIssuingOptions);
		Program.ConfigureScopedServices(builder);
		builder.Services.AddHostedService<ReverseGeocodingBackgroundService>();
		Program.ConfigureEntityFramework(builder);
		Program.ConfigureIdentity(builder);
		Program.ConfigureJwtAuthentication(builder, jwtIssuingOptions);
		
		// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1", new OpenApiInfo { 
				Title = "DNTYM API", 
				Version = "v1" 
			});
			c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
				In = ParameterLocation.Header, 
				Description = "Please insert JWT with Bearer into field",
				Name = "Authorization",
				Type = SecuritySchemeType.ApiKey 
			});
			c.AddSecurityRequirement(new OpenApiSecurityRequirement {
				{ 
					new OpenApiSecurityScheme 
					{ 
						Reference = new OpenApiReference 
						{ 
							Type = ReferenceType.SecurityScheme,
							Id = "Bearer" 
						} 
					},
					new string[] { } 
				} 
			});
		});
	}

	private static void ConfigurePipeline(WebApplication app) {
		// Configure the HTTP request pipeline.
		if (app.Environment.IsDevelopment()) {
			app.UseSwagger();
			app.UseSwaggerUI();
		}

		//app.UseHttpsRedirection();

		app.UseAuthentication();
		app.UseAuthorization();

		app.MapControllers();
	}

	#region Services

	private static void ConfigureOptions(WebApplicationBuilder builder, out JwtIssuingOptions jwtIssuingOptions) {
		jwtIssuingOptions = new JwtIssuingOptions();
		builder.Configuration.Bind(JwtIssuingOptions.Key, jwtIssuingOptions);
		builder.Services.Configure<JwtIssuingOptions>(builder.Configuration.GetSection(JwtIssuingOptions.Key));
	}

	private static void ConfigureScopedServices(WebApplicationBuilder builder) {
		builder.Services.AddScoped<IJwtIssuingService, JwtIssuingService>();
		builder.Services.AddScoped<ILoginService<IdentityUser>, LoginService>();
		builder.Services.AddScoped<ISignUpService, SignUpService>();
		builder.Services.AddScoped<ITrackingService<string>, TrackingService>();
	}
	
	private static void ConfigureEntityFramework(WebApplicationBuilder builder) {
		builder.Services.AddEntityFrameworkNpgsql().AddDbContext<ApplicationDbContext>(
			options => options.UseNpgsql(
				builder.Configuration.GetConnectionString("postgres"),
				optionsBuilder => {
					optionsBuilder.MigrationsAssembly(typeof(Program).Assembly.GetName().Name);
				}
			)
		);
	}

	private static void ConfigureIdentity(WebApplicationBuilder builder) {
		builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => {
			options.User.RequireUniqueEmail = true;
			options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._+"; // removed @ so username cannot be an email
		}).AddEntityFrameworkStores<ApplicationDbContext>();
	}

	private static void ConfigureJwtAuthentication(WebApplicationBuilder builder, JwtIssuingOptions jwtIssuingOptions) {
		builder.Services.AddAuthentication(
			options => {
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}
		).AddJwtBearer(
			options => {
				options.SaveToken = true;
				options.TokenValidationParameters = new TokenValidationParameters {
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtIssuingOptions.Secret)),
					ValidateIssuer = false,
					ValidateAudience = false,
					ValidateLifetime = true
				};
			}
		);
	}

	#endregion
}