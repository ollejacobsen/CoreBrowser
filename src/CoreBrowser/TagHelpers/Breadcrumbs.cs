using System;
using System.Text;
using Microsoft.AspNet.Razor.TagHelpers;

namespace CoreBrowser.TagHelpers
{
	public class Breadcrumbs : TagHelper
	{
		public string Path { get; set; }
		public string Root { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			var pathAsArray = !string.IsNullOrWhiteSpace(Path)
				? Path.Split(new string[] { "/" } , StringSplitOptions.RemoveEmptyEntries) : new string[] { };

			var lastPath = "/";
			var builder = new StringBuilder();
			if (!string.IsNullOrWhiteSpace(Root))
			{
				var homeElement = pathAsArray.Length == 0 
					? $"<li class=\"active\"><i class=\"fa fa-home\" aria-hidden=\"true\"></i> {Root}</li>"
					: $"<li><i class=\"fa fa-home\" aria-hidden=\"true\"></i> <a href=\"{lastPath}\">{Root}</a></li>";

				builder.AppendLine(homeElement);
			}

			for (int i = 0; i < pathAsArray.Length; i++)
			{
				var portion = pathAsArray[i];
				var currentPath = lastPath + portion + "/";

				var element = i == pathAsArray.Length - 1 
					? $"<li class=\"active\">{portion}</li>" 
					: $"<li><a href=\"{currentPath}\">{portion}</a></li>";
				
				builder.AppendLine(element);
				lastPath = currentPath;
			}

			output.TagName = "ol";
			if (output.Attributes["class"] == null)
				output.Attributes["class"] += "breadcrumb";
			else
				output.Attributes["class"].Value += " breadcrumb";
			
			output.Content.SetHtmlContent(builder.ToString());

			base.Process(context, output);
		}
	}
}
