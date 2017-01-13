using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ApiAuctionShop.Models;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNet.WebSockets.Server;
using System.Net.WebSockets;
using Microsoft.AspNet.Http;
using System.Threading;
using System.Text;
using Projekt.Controllers;
using ApiAuctionShop.Database;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNet.Authentication.Cookies;
using Microsoft.AspNet.Localization;
using System.Globalization;
using Microsoft.AspNet.Mvc.Razor;
using Microsoft.Extensions.OptionsModel;

namespace ApiAuctionShop
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {

            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);


            if (env.IsDevelopment())
            {
                builder.AddUserSecrets();
                builder.AddApplicationInsightsSettings(developerMode: true);
            }


            var waitHandle = new AutoResetEvent(false);
            ThreadPool.RegisterWaitForSingleObject(
                waitHandle,
                // Method to execute
                (state, timeout) =>
                {
                            
                    Console.WriteLine(DateTime.Now + " START: Updating auction states.");
                    SqlConnection sqlConnection1 = new SqlConnection(Configuration["Data:DefaultConnection:ConnectionString"]);
                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader; 

                    cmd.CommandText = "UPDATE[master].[dbo].[Auctions] SET state = 'active' WHERE startDate <= GETDATE() and endDate >= GETDATE() and state != 'inactive' and state != 'ended'; UPDATE[master].[dbo].[Auctions] SET state = 'waiting' WHERE startDate > GETDATE() and state != 'inactive' and state != 'ended'; UPDATE[master].[dbo].[Auctions] SET state = 'ended', winnerID = c.Id FROM [master].[dbo].[Auctions] a RIGHT JOIN [master].[dbo].[Bid] b on a.ID = b.auctionId LEFT JOIN [master].[dbo].[AspNetUsers] c on b.bidAuthor = c.Email WHERE endDate < GETDATE() and(state != 'inactive' or state IS null) and state != 'ended' and bidAuthor in(SELECT TOP 1 bidAuthor FROM [master].[dbo].[Bid] b where b.auctionId = a.ID order by b.bid DESC); UPDATE[master].[dbo].[Auctions] SET state = 'ended' WHERE endDate < GETDATE() and(state = 'active' or state = 'waiting') ";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = sqlConnection1;
                    sqlConnection1.Open();
                    reader = cmd.ExecuteReader();
                    sqlConnection1.Close();
                    Console.WriteLine(DateTime.Now + " END: Updating auction states complete.");
                },              
                // optional state object to pass to the method
                null,
                // Execute the method after 1 minute
                TimeSpan.FromMinutes(1),
                // Set this to false to execute it repeatedly every 5 seconds
                false
            );

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]));

            services.AddIdentity<Signup, IdentityRole>()
                 .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            services.AddJsonLocalization(options => options.ResourcesPath = "Resources");

            services.AddMvc()
                .AddViewLocalization();
            services.AddMvc().AddJsonOptions(a =>
            {
                a.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                a.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            }
            );
            
           
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            /////
            
            var requestLocalizationOptions = new RequestLocalizationOptions
            {
                SupportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("pl-PL"),
                    new CultureInfo("en-GB"),
                },
                SupportedUICultures = new List<CultureInfo>
                {
                    new CultureInfo("pl-PL"),
                    new CultureInfo("en-GB"),
                }
            };

            app.UseRequestLocalization(requestLocalizationOptions,
                             new RequestCulture(new CultureInfo("pl-PL")));

            ///////
           

            app.UseApplicationInsightsRequestTelemetry();
            app.UseDeveloperExceptionPage();
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/pl-PL/Home/Error");

                try
                {
                    using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                        .CreateScope())
                    {
                        serviceScope.ServiceProvider.GetService<ApplicationDbContext>()
                             .Database.Migrate();
                    }
                }
                catch { }
            }

            app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles();

            app.UseIdentity();

            app.UseWebSockets();

            app.Use(WebSocketHandlerServer.ChatHandler);

            //Jak obsluzyc MonitorController :C

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{language=pl-PL}/{controller=Home}/{action=Index}/{*id}");

                routes.MapRoute(
                    name: "s",
                    template: "{controller=Home}/{action=Index}/{*id}");
            });
            
        }

        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}