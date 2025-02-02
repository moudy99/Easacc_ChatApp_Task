using Application.Helpers;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWork;
using Application.Mapping;
using Application.Services;
using Core.Model;
using Infrastructure;
using Infrastructure.Repositories;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Presentation.HUBs;

namespace Presentation
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            ConfigurationManager configuration = builder.Configuration;

            builder.Services.AddControllersWithViews();
            builder.Services.AddSignalR();

            builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);


            builder.Services.AddDbContext<ApplicationDbContext>(options =>

               options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                   b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))

           );

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();


            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddTransient<UserManager<ApplicationUser>>();
            builder.Services.AddTransient<SignInManager<ApplicationUser>>();

            builder.Services.AddScoped<IChatRepository, ChatRepository>();
            builder.Services.AddScoped<IChatService, ChatService>();

            builder.Services.AddScoped<IMessageRepository, MessageRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();



            //CORS 
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowOrigin",
                    builder => builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .SetIsOriginAllowed(alow => true));

            });

            var app = builder.Build();



            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var dbContext = services.GetRequiredService<ApplicationDbContext>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            try
            {
                await RoleInitializer.SeedRolesAsync(roleManager);
                await AdminInitializer.SeedAdminUserAsync(userManager);
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "Error occurred during database migration or seeding");
            }



            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors("AllowOrigin");

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseStaticFiles();


            app.MapHub<ChatHub>("/chatHub");
            app.MapControllerRoute(
                name: "default",
            pattern: "{controller=Account}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
