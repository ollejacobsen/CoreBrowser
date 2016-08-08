using System.IO;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using CoreBrowser.Helpers;
using CoreBrowser.Services;
using Microsoft.Extensions.Options;

namespace CoreBrowser.Controllers
{
    public class CoreBrowser : Controller
    {
	    private readonly IFileSystemService _fileService;
	    private readonly CoreBrowserConfiguration _configuration;

	    public CoreBrowser(IFileSystemService fileService, IOptions<CoreBrowserConfiguration> configuration)
	    {
		    _fileService = fileService;
		    _configuration = configuration.Value;
	    }

	    public IActionResult Index(string url)
		{
			if (_fileService.IsFile(url))
				return ResponseWithFile(url);


		    ViewBag.GoogleUA = _configuration.GaTrackingUA;
			var model = _fileService.GetDirectory(url);
			return View("Index", model);
		}

	    private IActionResult ResponseWithFile(string url)
	    {
		    if (!_fileService.IsAvailable(url))
		    {
			    HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
			    return View("NotAvailable");
		    }

			var fileInfo = _fileService.GetFileFromFilesystem(url);
			byte[] fileContents;
			using (MemoryStream data = new MemoryStream())
			{
				using (Stream fileStream = fileInfo.OpenRead())
				{
					fileStream.CopyTo(data);
					fileContents = data.ToArray();
				}
			}

			return File(fileContents, MimeTypes.GetMimeType(fileInfo.Extension));
		}

	    public IActionResult Error()
	    {
		    return View("Error");
	    }
	}
}
