using System;
using System.Collections.Generic;
using System.IO;

namespace CoreBrowser.Services
{
	public interface IFileSystemConfiguration
	{
		IFileSystemConfiguration AddExcludedFileExtensions(string[] extensions);
		IFileSystemConfiguration AddExcludedFileNames(string[] filenames);
		IFileSystemConfiguration SetDirectoryHeaderFileName(string name);

		FileSystemConfiguration Build();
	}

	public class FileSystemConfiguration : IFileSystemConfiguration
	{
		public DirectoryInfo Root { get; private set; }

		public string CurrentHeaderFile = "_headerContent.md";
		public List<string> ExcludedFileExtension { get; } = new List<string>();

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

		public IFileSystemConfiguration AddExcludedFileExtensions(string[] extensions)
		{
			if (extensions != null && extensions.Length > 0)
			{
				ExcludedFileExtension.AddRange(extensions);
			}

			return this;
		}

		public IFileSystemConfiguration AddExcludedFileNames(string[] filenames)
		{
			if (filenames != null && filenames.Length > 0)
			{
				ExcludedFileExtension.AddRange(filenames);
			}

			return this;
		}

		public IFileSystemConfiguration SetDirectoryHeaderFileName(string name)
		{
			if(name != null && !string.IsNullOrWhiteSpace(name))
			{
				this.CurrentHeaderFile = name;
			}

			return this;
		}

		public FileSystemConfiguration Build()
		{
			return this;
		}
	}
}