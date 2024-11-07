using Microsoft.EntityFrameworkCore;
using Myshop.Web.Data;
using Myshop.Web.IRepository;
using Myshop.Web.Repository;
using Myshop.Web.UnitOfWorks;
using Microsoft.AspNetCore.Identity;
using Myshop.Web.Models;
using Myshop.Web.Service;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;

namespace Myshop.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            // Configure DbContext with a connection string from configuration
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<AppUser, IdentityRole>(options => options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMicroseconds(4))
                .AddDefaultTokenProviders()
                .AddDefaultUI()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddAutoMapper(typeof(Program));

            // Register UnitOfWork and Repositories
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfwork>();
            builder.Services.AddScoped<IProduct, ProductRepository>();
            builder.Services.AddScoped<ICategory, CategoryRepository>();
            builder.Services.AddScoped<IEmailSender, EmailSender>();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession();

            // Configure Stripe settings
            var stripeSettings = builder.Configuration.GetSection("StripeSettings").Get<StripeSettings>();
            StripeConfiguration.ApiKey = stripeSettings.SecretKey; // Set the Stripe Secret Key

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseSession();

            // Redirect root requests to a specific area route
            app.Use(async (context, next) =>
            {
                // Check if the request path is the root
                if (context.Request.Path == "/")
                {
                    // Redirect to the desired area, controller, and action
                    context.Response.Redirect("/Customer/Customer/Index");
                    return; // Skip to the next middleware
                }
                await next();
            });

            // Map area routes first to prioritize them
            app.MapControllerRoute(
                name: "areaRoute",
                pattern: "{area:exists}/{controller=Customer}/{action=Index}/{id?}");

            // Default route
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            // Run the application
            app.Run();
        }
    }
}
