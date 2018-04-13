using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CoreBrowser.Services;
using CoreBrowser.Models;

namespace CoreBrowser
{
	public class Startup
	{
		public IConfigurationRoot Configuration { get; }
		public IHostingEnvironment HostingEnvironment { get; }

		private FileSystemConfiguration _fileSystemConfiguration;

		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
				.AddEnvironmentVariables();

			Configuration = builder.Build();
			HostingEnvironment = env;
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			var conf = Configuration.GetSection("CoreBrowser").Get<CoreBrowserConfiguration>();
			var filesRootFolder = conf.FilesRootFolder;

			if (filesRootFolder.StartsWith(ApplicationConstants.WWWROOT_PLACEHOLDER))
			{
				var removedPlaceholder = filesRootFolder.Remove(0, ApplicationConstants.WWWROOT_PLACEHOLDER.Length);
				filesRootFolder = $"{HostingEnvironment.WebRootPath}{removedPlaceholder}";
			}

			_fileSystemConfiguration = new FileSystemConfiguration(filesRootFolder)
				.AddExcludedFileNames(conf.ExcludedFileNames)
				.AddExcludedFileExtensions(conf.ExcludedFileExtensions)
				.SetDirectoryHeaderFileName(conf.DirectoryHeaderFileName)
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

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "error",
					template: "corebrowser/error",
					defaults: new { controller = "CoreBrowser", action = "Error" });

				routes.MapRoute(
					name: "search",
					template: "search",
					defaults: new { controller = "CoreBrowser", action = "Search" });

				routes.MapRoute(
					name: "default",
					template: "{*url}",
					defaults: new { controller = "CoreBrowser", action = "Index" });
			});
		}
	}
}
