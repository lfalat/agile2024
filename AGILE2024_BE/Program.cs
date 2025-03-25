using AGILE2024_BE.Data;
using AGILE2024_BE.Models.Identity;
using AGILE2024_BE.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

//if (args.Length == 1 && args[0].ToLower() == "seeddata")
//    SeedData(app);

//void SeedData(IHost app)
//{
//    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

//    using (var scope = scopedFactory.CreateScope())
//    {
//        var service = scope.ServiceProvider.GetService<Seed>();
//        service.SeedDataContext();
//    }
//}


namespace AGILE2024_BE.API
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var webAppBuilder = WebApplication.CreateBuilder(args);
            //webAppBuilder.WebHost.UseUrls("http://0.0.0.0:5001", "https://0.0.0.0:5002");
            AddServices(webAppBuilder);

            var webApp = webAppBuilder.Build();

            AppSetup(webApp);

            using (var scope = webApp.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await Seed.InitializeAsync(services);
            }


            webApp.Run();
        }

        private static void AddServices(WebApplicationBuilder webAppBuilder)
        {
            var webAppConfig = webAppBuilder.Configuration;
            webAppBuilder.Services.AddEndpointsApiExplorer();
            webAppBuilder.Services.AddSwaggerGen();
            webAppBuilder.Services.AddAntiforgery();

            webAppBuilder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            }
            );

            webAppBuilder.Services.AddSignalR();

            webAppBuilder.Services.AddDbContext<AgileDBContext>(options =>
            {
                var connectionString = webAppConfig.GetConnectionString("Azure_MySql");
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                //options.UseLazyLoadingProxies();
            });

            webAppBuilder.Services.AddIdentity<ExtendedIdentityUser, IdentityRole>(o =>
            {
                o.Password.RequiredLength = 13;
                o.Password.RequireNonAlphanumeric = true;
                o.Password.RequireUppercase = true;
                o.Password.RequireLowercase = true;
                o.Password.RequireDigit = true;
                o.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<AgileDBContext>()
                .AddDefaultTokenProviders();

            webAppBuilder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = webAppConfig["Jwt:Issuer"],
                    ValidAudience = webAppConfig["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(webAppConfig["Jwt:Key"]))
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            //webAppBuilder.Services.AddScoped<INotificationHandler, DummyNotificationHandler>();
            webAppBuilder.Services.AddScoped<INotificationHandler, GoalNotificationHandler>();
            webAppBuilder.Services.AddScoped<INotificationHandler, FeedbackNotificationHandler>();
            webAppBuilder.Services.AddScoped<INotificationHandler, ReviewNotificationHandler>();
            webAppBuilder.Services.AddHostedService<NotificationService>();

            webAppBuilder.Services.AddControllers();
        }

        private static void AppSetup(WebApplication webApp)
        {
            if (webApp.Environment.IsDevelopment())
            {
                webApp.UseSwagger();
                webApp.UseSwaggerUI();
            }

            webApp.UseHttpsRedirection();

            webApp.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            webApp.UseDefaultFiles();
            webApp.UseStaticFiles();
            webApp.UseRouting();

            webApp.UseAuthentication();
            webApp.UseAuthorization();

            webApp.MapFallbackToFile("index.html");
            webApp.MapControllers();

            webApp.MapHub<NotificationHub>("/notificationHub");
        }
    }
}
