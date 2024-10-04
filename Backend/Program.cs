using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Backend.Data;

namespace Backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureServices((context, services) =>
                    {
                        var configuration = context.Configuration;

                        // Bind email settings from appsettings.json
                        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

                        // Register GmailService for dependency injection
                        services.AddSingleton<GmailService>();

                        // Register MongoDB context
                        services.AddSingleton<IMongoDbContext, MongoDbContext>();

                        // Register services
                        services.AddScoped<IUserService, UserService>();
                        services.AddScoped<IJwtService, JwtService>();
                        services.AddScoped<IPasswordHasher, PasswordHasher>(); // Register IPasswordHasher if needed
                        services.AddScoped<IOrderService, OrderService>();

                        // Configure JWT authentication
                        services.AddAuthentication(options =>
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
                                ValidateLifetime = true,
                                ValidateIssuerSigningKey = true,
                                ValidIssuer = configuration["Jwt:Issuer"],
                                ValidAudience = configuration["Jwt:Audience"],
                                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                            };
                        });

                        // Configure authorization policies
                        services.AddAuthorization(options =>
                        {
                            options.AddPolicy("Customer", policy => policy.RequireRole("Customer"));
                            options.AddPolicy("Administrator", policy => policy.RequireRole("Administrator"));
                            options.AddPolicy("CSR", policy => policy.RequireRole("CSR"));
                            options.AddPolicy("Vendor", policy => policy.RequireRole("Vendor"));
                        });

                        // Add CORS
                        services.AddCors(options =>
                        {
                            options.AddPolicy("AllowReactApp", builder =>
                            {
                                builder.WithOrigins("http://localhost:3000") // React app URL
                                       .AllowAnyHeader()
                                       .AllowAnyMethod()
                                       .AllowCredentials(); // Allow cookies and authorization headers
                            });
                        });

                        // Add MVC controllers
                        services.AddControllers();
                    });

                    webBuilder.Configure((context, app) =>
                    {
                        var env = context.HostingEnvironment;

                        if (env.IsDevelopment())
                        {
                            app.UseDeveloperExceptionPage();
                        }
                        else
                        {
                            app.UseExceptionHandler("/Home/Error"); // Handle errors in production
                            app.UseHsts();
                        }

                        app.UseHttpsRedirection();
                        app.UseStaticFiles();

                        app.UseRouting();

                        app.UseCors("AllowReactApp"); // Enable CORS
                        app.UseAuthentication(); // Enable authentication middleware
                        app.UseAuthorization();  // Enable authorization middleware

                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers(); // Map attribute routes
                        });
                    });
                });
    }
}
