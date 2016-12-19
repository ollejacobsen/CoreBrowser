using System;
using System.Collections.Generic;
using System.IO;

namespace CoreBrowser.Services
{
	public interface IFileSystemConfiguration
	{
		IFileSystemConfiguration AddExcludedFileExtensions(string extensions);
		IFileSystemConfiguration AddExcludedFileExtensions(params string[] extensions);
		IFileSystemConfiguration AddExcludedFileNames(string filenames);
		IFileSystemConfiguration AddExcludedFileNames(params string[] filenames);
		IFileSystemConfiguration SetDirectoryHeaderFileName(string name);

		FileSystemConfiguration Build();
	}

	public class FileSystemConfiguration : IFileSystemConfiguration
	{
		public DirectoryInfo Root { get; private set; }

		public string CurrentHeaderFile = "_headerContent.md";

		private List<string> _excludedFileExtension = new List<string>();
		public List<string> ExcludedFileExtension { get { return _excludedFileExtension; } }

		private List<string> _excludedFileNames = new List<string>();
		public List<string> ExcludedFileNames
		{
			get
			{
				if (!string.IsNullOrWhiteSpace(CurrentHeaderFile) && !_excludedFileNames.Contains(CurrentHeaderFile))
					_excludedFileNames.Add(CurrentHeaderFile);

				return _excludedFileNames;
			}
		}

		public FileSystemConfiguration(string rootDirectoryPath)
		{
			if (!Directory.Exists(rootDirectoryPath))
				throw new DirectoryNotFoundException(rootDirectoryPath);

			Root = new DirectoryInfo(rootDirectoryPath);
		}

		public IFileSystemConfiguration AddExcludedFileExtensions(string extensions)
		{
			if (!string.IsNullOrWhiteSpace(extensions))
			{
				var extensionsArray = extensions.ToLowerInvariant()
						.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries);

				_excludedFileExtension.AddRange(extensionsArray);
			}

			return this;
		}

		public IFileSystemConfiguration AddExcludedFileExtensions(params string[] extensions)
		{
			if (extensions.Length > 0)
				_excludedFileExtension.AddRange(extensions);

			return this;
		}

		public IFileSystemConfiguration AddExcludedFileNames(string filenames)
		{
			if (!string.IsNullOrWhiteSpace(filenames))
			{
				var filenamesArray = filenames.ToLowerInvariant()
						.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries);

				_excludedFileNames.AddRange(filenamesArray);
			}

			return this;
		}

		public IFileSystemConfiguration AddExcludedFileNames(params string[] filenames)
		{
			if (filenames.Length > 0)
				_excludedFileExtension.AddRange(filenames);

			return this;
		}

		public IFileSystemConfiguration SetDirectoryHeaderFileName(string name)
		{
			this.CurrentHeaderFile = name;

			return this;
		}

		public FileSystemConfiguration Build()
		{
			return this;
		}
	}
}