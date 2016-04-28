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
				case "archive":
				    return "fa-file-archive-o";
				case "code":
				    return "fa-file-code-o";
				case "video":
				    return "fa-file-video-o";
				case "audio":
				    return "fa-file-audio-o";
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
					return "powerpoint";
				case ".zip":
				case ".rar":
				case ".7zip":
				case ".tar":
				case ".gz":
				case ".lzma":
					return "archive";
				case ".js":
				case ".jsx":
				case ".json":
				case ".htm":
				case ".html":
				case ".cshtml":
				case ".cs":
					return "code";
				case ".mpeg":
				case ".avi":
				case ".mpg":
				case ".mkv":
					return "video";
				case ".mp3":
				case ".aac":
				case ".wave":
					return "audio";
				default:
					return "file";
			}
		}
    }
}
