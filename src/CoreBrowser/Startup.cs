using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CoreBrowser.Services;
using CoreBrowser.Models;
using Microsoft.Extensions.FileProviders;

namespace CoreBrowser
{
    public class Startup
    {
		public IConfigurationRoot Configuration { get; }
		private IHostingEnvironment _hostingEnv;

		private FileSystemConfiguration _fileSystemConfiguration;

		public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

			Configuration = builder.Build();
			_hostingEnv = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
			var filesRootFolder = Configuration["CoreBrowser:RootFolderInWWWRoot"];
			if(filesRootFolder.StartsWith(ApplicationConstants.WWWROOT_PLACEHOLDER)) {
				var removedPlaceholder = filesRootFolder.Remove(0, ApplicationConstants.WWWROOT_PLACEHOLDER.Length);
				filesRootFolder = $"{_hostingEnv.WebRootPath}{removedPlaceholder}";
			}

			_fileSystemConfiguration = new FileSystemConfiguration(filesRootFolder)
				.AddExcludedFileNames("web.config")
				.Build();

			services.AddTransient<IFileSystemService>(x => new FileSystemService(_fileSystemConfiguration));
			services.AddTransient<IConfiguration>(x => Configuration);
			services.AddTransient<ICoreBrowserRazorView, CoreBrowserRazorView>();

			// Add framework services.
			services.AddMvc();

			services.Configure<CoreBrowserConfiguration>(Configuration.GetSection("CoreBrowser"));
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

			app.UseStaticFiles();

			app.UseFileServer(new FileServerOptions()
			{
				FileProvider = new PhysicalFileProvider(_fileSystemConfiguration.Root.FullName)
			});

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "error",
					template: "corebrowser/error",
					defaults: new { controller = "CoreBrowser", action = "Error" });

				routes.MapRoute(
					name: "default",
					template: "{*url}",
					defaults: new { controller = "CoreBrowser", action = "Index" });
			});
		}
    }
}
