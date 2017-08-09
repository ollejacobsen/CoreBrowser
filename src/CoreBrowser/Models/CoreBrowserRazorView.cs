using System;
using Microsoft.Extensions.Options;

namespace CoreBrowser.Models
{
	public interface ICoreBrowserRazorView
	{
		string TitleSuffix { get; }
		string GoogleUA { get; }

		string RenderTitle(string prefix, string separator = " - ");
		string ReplaceTokens(string value, string replaceWith = " ");
	}

	public class CoreBrowserRazorView : ICoreBrowserRazorView
	{
		private CoreBrowserConfiguration _configuration { get; set; }

		public string TitleSuffix => _configuration.TitleSuffix;

		public string GoogleUA => _configuration.GaTrackingUA;

		private string charactersWhitespaceTokens => _configuration.CharactersWhitespaceTokens;

		private string[] _charactersWhitespaceTokens;
		private string[] CharactersWhitespaceTokens
		{
			get
			{
				if (_charactersWhitespaceTokens == null)
				{
					if (string.IsNullOrWhiteSpace(_configuration.CharactersWhitespaceTokens))
					{
						_charactersWhitespaceTokens = new string[0];
					}
					else
					{
						_charactersWhitespaceTokens = _configuration.CharactersWhitespaceTokens
							.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
					}
				}

				return _charactersWhitespaceTokens;
			}
		}

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

		public string ReplaceTokens(string value, string replaceWith = " ")
		{
			if (CharactersWhitespaceTokens.Length > 0)
			{
				foreach (var token in CharactersWhitespaceTokens)
				{
					value = value.Replace(token, replaceWith);
				}
			}

			return value;
		}
	}
}
