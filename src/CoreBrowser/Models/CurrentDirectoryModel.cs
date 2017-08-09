using System.Collections.Generic;
using System.IO;
using System.Linq;
using CoreBrowser.Extensions;

namespace CoreBrowser.Models
{
	public class CurrentDirectoryModel : DirectoryModel
	{
		public IReadOnlyList<DirectoryModel> Directories { get; set; }
		public IReadOnlyList<FileModel> Files { get; set; }

		public int NrOfFiles => Files.Count;

		private FileSize _totalSizeOfAllFiles;
		public FileSize TotalSizeOfAllFiles
		{
			get
			{
				return _totalSizeOfAllFiles
				  ?? (_totalSizeOfAllFiles = new FileSize(Files.Sum(x => x.Size.Length)));
			}
		}

		public string HeaderContent { get; set; }

		public DirectoryModel Parent { get; set; }

		public IReadOnlyList<DirectoryModel> DirectoryTree { get; set; }

		public CurrentDirectoryModel(DirectoryInfo directory, string path)
			: base(directory, path)
		{

		}
	}
}
