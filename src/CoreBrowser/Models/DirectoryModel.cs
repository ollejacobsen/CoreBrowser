using System;
using System.IO;

namespace CoreBrowser.Models
{
	public class DirectoryModel
	{
		public string Name { get; private set; }

		public DateTime LastWriteTime { get; private set; }

		public string Path { get; private set; }

		public DirectoryModel(DirectoryInfo directory, string path)
		{
			this.Name = directory.Name;
			this.LastWriteTime = directory.LastWriteTime;
			this.Path = path;
		}

		public static DirectoryModel Map(DirectoryInfo dir, string path)
		{
			var model = new DirectoryModel(dir, path);

			return model;
		}
	}
}
