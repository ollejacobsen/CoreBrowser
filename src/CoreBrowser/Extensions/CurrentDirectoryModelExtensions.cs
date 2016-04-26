using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.ViewFeatures;
using CoreBrowser.Models;

namespace CoreBrowser.Extensions
{
    public static class CurrentDirectoryModelExtensions
    {
	    public static string[] GetBreadcrumb(this CurrentDirectoryModel model)
	    {
		    var pathAsArray = model.Path.Split('/');


		    return pathAsArray;
	    }

		public static BreadcrumbItem[] GetBreadcrumbList(this CurrentDirectoryModel model)
		{
			var pathAsArray = !string.IsNullOrWhiteSpace(model.Path)
				? model.Path.Split('/') : new string[] {};

			var b = new List<BreadcrumbItem>(pathAsArray.Length + 1) { new BreadcrumbItem { Name = "Home", Path = "/", CssIcon = "fa fa-home" } };
			var lastPath = b.First().Path;
			foreach (var path in pathAsArray)
			{
				var item = new BreadcrumbItem { Name = path, Path = lastPath + path + "/"};
				b.Add(item);
				lastPath = item.Path;
			}

			b.Last().Active = true;

			return b.ToArray();
		}
	}

	public class BreadcrumbItem
	{
		public string Name { get; set; }
		public string Path { get; set; }
		public bool Active { get; set; }
		public string CssIcon { get; set; }

		public string ToHtml()
		{
			var html = new StringBuilder();
			html.Append(Active ? "<li class=\"active\">" : "<li>");
			html.Append(!string.IsNullOrWhiteSpace(CssIcon) ? $"<i class=\"{CssIcon}\" aria-hidden=\"true\"></i> " : "");
			html.Append(Active ? $"{Name}</li>" : $"<a href=\"{Path}\">{Name}</a></li>");

			return html.ToString();
		}
}
}
