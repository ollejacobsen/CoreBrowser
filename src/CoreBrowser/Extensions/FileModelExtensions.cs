using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreBrowser.Models;

namespace CoreBrowser.Extensions
{
    public static class FileModelExtensions
    {
	    public static string GetFontAwsomeIcon(this FileModel model)
	    {
		    switch (model.GetKnownType())
		    {
				case "image":
					return "fa-picture-o";
				case "text":
				    return "fa-file-text-o";
				case "pdf":
					return "fa-file-pdf-o";
				case "word":
					return "fa-file-word-o";
				case "excel":
					return "fa-excel-o";
				case "powerpoint":
					return "fa-file-powerpoint-o";
				default:
				    return "fa-file-o";
		    }
	    }

	    public static string GetKnownType(this FileModel model)
	    {
			switch (model.Extension.ToLower())
			{
				case ".jpg":
				case ".jpeg":
				case ".png":
				case ".gif":
					return "image";
				case ".txt":
					return "text";
				case ".pdf":
					return "pdf";
				case ".doc":
				case ".docx":
					return "word";
				case ".xls":
				case ".xlsx":
					return "excel";
				case ".ppt":
				case ".pptx":
					return "powerpoint-";
				default:
					return "file";
			}
		}
    }
}
