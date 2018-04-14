using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using MarkdownSharp;
using CoreBrowser.Models;

namespace CoreBrowser.Services
{
	public interface IFileSystemService
	{
		CurrentDirectoryModel GetDirectory(string absoluteVirtualPath = "/");
		FileInfo GetFileFromFilesystem(string absoluteVirtualPath);
		SearchResultModel FindFiles(string pattern);
		bool IsFile(string relativePath);
		bool IsDirectory(string relativePath);
		bool IsAvailable(string relativePath);
		string GetAbsoluteVirtualPath(string fullFilesystemPath);
		string MapPath(string virtualPath);
	}

	public class FileSystemService : IFileSystemService
	{
		private readonly FileSystemConfiguration _conf;
		private Func<FileInfo, IEnumerable<string>, IEnumerable<string>, bool> FileFilter = (file, exludedNames, exludedExtensions)
				=> (
					(exludedExtensions.Contains(file.Extension, StringComparer.OrdinalIgnoreCase) ||
					exludedNames.Contains(file.Name, StringComparer.OrdinalIgnoreCase)) == false
				);

		public FileSystemService(FileSystemConfiguration configuration)
		{
			_conf = configuration;
		}

		public (DirectoryInfo Directory, DirectoryInfo[] SubDirs, FileInfo[] Files) GetDirectoryContent(string absoluteVirtualPath)
		{
			var activePath = MapPath(absoluteVirtualPath);
			var currentDirectory = new DirectoryInfo(activePath);

			if (!currentDirectory.Exists)
				throw new DirectoryNotFoundException();

			var subDirs = currentDirectory.GetDirectories().OrderBy(x => x.Name).ToArray();
			var files = currentDirectory.GetFiles()
				.Where(x => FileFilter(x, _conf.ExcludedFileNames, _conf.ExcludedFileExtension))
				.OrderBy(x => x.Name)
				.ToArray();

			return (currentDirectory, subDirs, files);
		}
		
		public CurrentDirectoryModel GetDirectory(string absoluteVirtualPath = null)
		{
			var contents = GetDirectoryContent(absoluteVirtualPath);

			var model = new CurrentDirectoryModel(contents.Directory, absoluteVirtualPath)
			{
				Parent = contents.Directory.FullName.Equals(_conf.Root.FullName) 
					? null : new DirectoryModel(contents.Directory.Parent, GetAbsoluteVirtualPath(contents.Directory.Parent.FullName)),
				DirectoryTree = null,
				HeaderContent = GetHeaderContent(contents.Directory, _conf.CurrentHeaderFile)
								?? string.Empty,
				Directories = contents.SubDirs != null
					? contents.SubDirs.Select(x => DirectoryModel.Map(x, GetAbsoluteVirtualPath(x.FullName))).ToList()
					: new List<DirectoryModel>(),
				Files = contents.Files != null
					? contents.Files.Select(x => FileModel.Map(x, GetAbsoluteVirtualPath(x.FullName))).ToList()
					: new List<FileModel>()
			};

			return model;
		}

		public FileInfo GetFileFromFilesystem(string absoluteVirtualPath)
		{
			if (string.IsNullOrWhiteSpace(absoluteVirtualPath))
				return null;

			if (!IsFile(absoluteVirtualPath) || !IsAvailable(absoluteVirtualPath))
				return null;

			var fullPath = Path.Combine(_conf.Root.FullName, absoluteVirtualPath);
			
			return new FileInfo(fullPath);
		}

		public bool IsFile(string relativePath)
		{
			var isFile = false;

			try
			{
				var fullPath = Path.Combine(_conf.Root.FullName, relativePath);
				var fileInfo = new FileInfo(fullPath);

				isFile = fileInfo.Exists && (fileInfo.Attributes & FileAttributes.Directory) == 0;
			}
			catch (Exception)
			{
				// Todo: Log
			}

			return isFile;
		}

		public bool IsDirectory(string relativePath)
		{
			var isDirectory = false;

			try
			{
				var fullPath = Path.Combine(_conf.Root.FullName, relativePath);
				var dirInfo = new DirectoryInfo(fullPath);

				isDirectory = dirInfo.Exists && (dirInfo.Attributes & FileAttributes.Directory) != 0;
			}
			catch (Exception)
			{
				//Todo: Log
			}

			return isDirectory;
		}

		public bool IsAvailable(string relativePath)
		{
			var fullPath = Path.Combine(_conf.Root.FullName, relativePath);

			try
			{
				if (Directory.Exists(fullPath))
					return true;

				var file = new FileInfo(fullPath);
				if (!file.Exists)
					return false;

				var fileIsExcluded = _conf.ExcludedFileNames.Contains(file.Name, StringComparer.OrdinalIgnoreCase)
										|| _conf.ExcludedFileExtension.Contains(file.Extension, StringComparer.OrdinalIgnoreCase);

				return !fileIsExcluded;
			}
			catch (Exception)
			{
				//LOG
			}
			return false;
		}

		public SearchResultModel FindFiles(string pattern)
		{
			// Get all files with "*" and then match with Where. This is because we want case insensitive searches even on Linux.
			var foundFiles = new DirectoryInfo(_conf.Root.FullName)
				.GetFiles("*", SearchOption.AllDirectories)
				.Where(x => x.Name.ToLowerInvariant().Contains(pattern))
				.Where(x => FileFilter(x, _conf.ExcludedFileNames, _conf.ExcludedFileExtension))
				.OrderBy(x => x.Name)
				.ToArray();

			var model = new SearchResultModel()
			{
				SearchTerm = pattern,
				Files = foundFiles.Select(x =>
					FileModel.Map(x, GetAbsoluteVirtualPath(x.FullName))).ToList()
			};

			return model;
		}

		private string GetHeaderContent(DirectoryInfo dir, string filename)
		{
			if (dir == null || string.IsNullOrWhiteSpace(filename))
				return null;

			var file = new FileInfo(Path.Combine(dir.FullName + Path.DirectorySeparatorChar + filename));
			if (!file.Exists)
				return null;

			switch (file.Extension.ToLower())
			{
				case ".md":
					return GetMarkdownFile(file);
				case ".txt":
				case ".html":
				case ".htm":
					return GetFileContent(file);
				default:
					return null;
			}
		}

		private string GetMarkdownFile(FileInfo file)
		{
			var content = GetFileContent(file);
			var markdown = new Markdown();
			var html = markdown.Transform(content);

			return html;
		}

		private string GetFileContent(FileInfo file)
		{
			var content = string.Empty;

			using (var r = file.OpenText())
			{
				try
				{
					content = r.ReadToEnd();
				}
				catch (Exception)
				{
					//swallow
				}	
			}

			return content;
		}

		public string GetAbsoluteVirtualPath(string fullFilesystemPath)
		{
			if (string.IsNullOrWhiteSpace(fullFilesystemPath))
				return "/";
			
			//Check if path in in beneth root
			if (!fullFilesystemPath.StartsWith(_conf.Root.FullName, StringComparison.OrdinalIgnoreCase))
				return "/";

			fullFilesystemPath = fullFilesystemPath.Replace(_conf.Root.FullName, "")
				.Replace(@"\\", @"\");
			var virtualPath = fullFilesystemPath.Replace(Path.DirectorySeparatorChar, '/');

			return virtualPath.StartsWith("/") 
				? virtualPath 
				: string.Concat("/", virtualPath);
		}

		public string MapPath(string virtualPath)
		{
			if (string.IsNullOrWhiteSpace(virtualPath) || virtualPath.Equals("/"))
				return _conf.Root.FullName;

			if (virtualPath.StartsWith("/"))
				virtualPath = virtualPath.Remove(0, 1);

			virtualPath = virtualPath.Replace('/', Path.DirectorySeparatorChar);
			return Path.Combine(_conf.Root.FullName, virtualPath);
		}

		internal string CreateSearchPattern(string pattern)
		{
			if (string.IsNullOrWhiteSpace(pattern))
				return pattern;

			var searchPattern = pattern;
			if (!pattern.StartsWith("*"))
				searchPattern = $"*{searchPattern}";
			if(!pattern.EndsWith("*"))
				searchPattern = $"{searchPattern}*";

			return searchPattern;
		}
	}
}
