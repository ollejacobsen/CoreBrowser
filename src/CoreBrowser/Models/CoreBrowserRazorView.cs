using System;
using Microsoft.Extensions.Options;

namespace CoreBrowser.Models
{
	public interface ICoreBrowserRazorView
	{
		string TitleSuffix { get; }
		string GoogleUA { get; }

		string RenderTitle(string prefix, string separator = " - ");
	}

	public class CoreBrowserRazorView : ICoreBrowserRazorView
	{
		private CoreBrowserConfiguration _configuration { get; set; }

		public string TitleSuffix => _configuration.TitleSuffix;

		public string GoogleUA => _configuration.GaTrackingUA;

		public CoreBrowserRazorView(IOptions<CoreBrowserConfiguration> configuration)
		{
			_configuration = configuration.Value;
		}

		public string RenderTitle(string prefix, string separator = " - ")
		{
			if (string.IsNullOrWhiteSpace(prefix))
				return TitleSuffix;

			if (separator != null && !string.IsNullOrWhiteSpace(TitleSuffix))
				return $"{prefix}{separator}{TitleSuffix}";

			return $"{prefix}{TitleSuffix}";
		}
	}
}
