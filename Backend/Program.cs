using Backend.Data;
using Backend.Models;
using CloudinaryDotNet;
using Backend.Services.Implementations;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Backend
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            // CORS
            var allowedOrigins = builder.Configuration
                .GetSection("AllowedOrigins")
                .Get<string[]>() ?? Array.Empty<string>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Frontend", policy =>
                {
                    policy
                        .WithOrigins(allowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            // Database
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")
                ));

            // Application services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<IFarmerService, FarmerService>();

            // Geocoding service
            builder.Services.AddHttpClient<IGeocodingService, GeocodingService>(
                client =>
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("AgroMarket/1.0");
                });

            // Identity
            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // JWT
            var jwtKey = builder.Configuration["JwtSettings:Key"];

            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                throw new Exception("JWT key is missing from configuration.");
            }

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtKey)
                        )
                };
            });

            builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("CloudinarySettings"));

            var cloudinarySettings = builder.Configuration
                .GetSection("CloudinarySettings")
                .Get<CloudinarySettings>();

            if (cloudinarySettings == null ||
                string.IsNullOrWhiteSpace(cloudinarySettings.CloudName) ||
                string.IsNullOrWhiteSpace(cloudinarySettings.ApiKey) ||
                string.IsNullOrWhiteSpace(cloudinarySettings.ApiSecret))
            {
                throw new Exception("Cloudinary settings are missing.");
            }

            var account = new Account(
                cloudinarySettings.CloudName,
                cloudinarySettings.ApiKey,
                cloudinarySettings.ApiSecret
            );

            var cloudinary = new Cloudinary(account);

            builder.Services.AddSingleton(cloudinary);

            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Production error handling
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            // Seed roles
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await Backend.Data.Seed.RoleSeeder.SeedAsync(services);
            }

            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseCors("Frontend");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}