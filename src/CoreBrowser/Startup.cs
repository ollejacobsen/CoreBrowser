﻿using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CoreBrowser.Services;

namespace CoreBrowser
{
    public class Startup
    {
		private IHostingEnvironment _hostingEnv;

        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            Configuration = builder.Build();

			_hostingEnv = env;
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
			//Custom service
	        var conf = new FileSystemConfiguration(_hostingEnv.WebRootPath, "files")
				.AddExcludedFileNames("web.config").Build();
			services.AddInstance<IFileSystemService>(new FileSystemService(conf));
			
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/CoreBrowser/Error");
            }

            app.UseIISPlatformHandler();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
	            routes.MapRoute(
		            name: "error",
					template: "corebrowser/error",
					defaults: new {controller = "CoreBrowser", action = "Error"});

				routes.MapRoute(
					name: "default",
					template: "{*url}",
					defaults: new { controller = "CoreBrowser", action = "Index" });
			});

        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
