using System;
using System.IO;

namespace CoreBrowser.Models
{
	public class FileModel
	{
		public string Name { get; private set; }

		public string NameWithoutExtension => Name.Remove(Name.LastIndexOf(this.Extension), Extension.Length);

		public string Extension { get; private set; }
		public FileSize Size { get; private set; }

		public DateTime LastWriteTime { get; private set; }

		public string Path { get; private set; }

		public FileModel(string name, string extension, long size, string path, DateTime lastWriteTime)
		{
			this.Name = name;
			this.Extension = extension;
			this.Size = new FileSize(size);
			this.LastWriteTime = lastWriteTime;
			this.Path = path;
		}

		public static FileModel Map(FileInfo file, string path)
		{
			var model = new FileModel(
				name: file.Name,
				extension: file.Extension,
				size: file.Length,
				path: path,
				lastWriteTime: file.LastWriteTime);

			return model;
		}
	}
}
