using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBrowser.Extensions
{
    public static class IHtmlHelperExtensions
    {
		public static string RenderTitle(this IHtmlHelper html, string prefix, string suffix, string seperator = null)
		{
			if (string.IsNullOrWhiteSpace(prefix))
				return suffix;

			if (seperator != null)
				return $"{prefix}{seperator}{suffix}";
			
			return $"{prefix}{suffix}";
		}
    }
}
