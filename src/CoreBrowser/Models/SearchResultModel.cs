using System.Collections.Generic;
using System.Linq;

namespace CoreBrowser.Models
{
	public class SearchResultModel
	{
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

		public string SearchTerm { get; set; }
	}
}
