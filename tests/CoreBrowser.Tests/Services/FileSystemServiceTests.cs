using CoreBrowser.Services;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace CoreBrowser.Tests.Services
{
	public class FileSystemServiceTests
	{
		private readonly IFileSystemService _fileService;
		private readonly FileSystemConfiguration _configuration;
		private readonly string[] _excludedExtensions = new [] { ".hidden", ".secret" };
		private readonly string[] _excludedFilenames = new[] { "web.config" };
		private string _rootDir
		{
			get
			{
				var root = Path.Combine(AppContext.BaseDirectory, "../../../_files");

				// Try to find the "_files" folder when runnning Live Unit Tests
				if (!Directory.Exists(root))
				{
					var baseDir = new DirectoryInfo(AppContext.BaseDirectory);
					while(baseDir.Name != ".vs")
					{
						baseDir = baseDir.Parent;
					}

					root = Path.Combine(baseDir.FullName, "../tests/CoreBrowser.Tests/_files");
				}

				if (!Directory.Exists(root))
				{
					throw new DirectoryNotFoundException(root);
				}

				return root;
			}
		}

		public FileSystemServiceTests()
		{
			var conf = new FileSystemConfiguration(_rootDir)
				.AddExcludedFileExtensions(_excludedExtensions)
				.AddExcludedFileNames(_excludedFilenames)
				.Build();

			_configuration = conf;

			_fileService = new FileSystemService(_configuration);
		}

		[Fact]
		public void GetDirectory_GivenNull_ReturnRoot()
		{
			var contents = _fileService.GetDirectory();

			Assert.Equal("/", contents.Path);
		}

		[Fact]
		public void GetDirectory_GivenSubdirectory_ReturnRelativePath()
		{
			var contents = _fileService.GetDirectory("/level-one");

			Assert.Equal("/level-one", contents.Path);
		}

		[Fact]
		public void GetDirectory_GivenSecondLevelSubdirectory_ReturnRelativePath()
		{
			var contents = _fileService.GetDirectory("/level-one/level-two");

			Assert.Equal("/level-one/level-two", contents.Path);
		}

		[Fact]
		public void GetDirectory_GivenRoot_ReturnNoSecretExtensions()
		{
			var contents = _fileService.GetDirectory();

			Assert.DoesNotContain(contents.Files, x =>
				_excludedExtensions.Contains(string.Concat(".", x.Extension)));
		}

		[Fact]
		public void GetDirectory_GivenRoot_ReturnNoHiddenFiles()
		{
			var contents = _fileService.GetDirectory();
			Assert.DoesNotContain(contents.Files, x => x.Name == "web.config");
			Assert.DoesNotContain(contents.Files, x => x.Name == "_headerContent.md");
		}

		[Fact]
		public void GetDirectory_GetKilobyteFile_Return()
		{
			var contents = _fileService.GetDirectory();
			var file = contents.Files.Single(x => x.Name == "kilobyte.fake");

			Assert.Equal("1KB", file.Size.ToPrettySize());
		}

		[Fact]
		public void GetDirectory_GetMegabyteFile_Return()
		{
			var contents = _fileService.GetDirectory();
			var file = contents.Files.Single(x => x.Name == "megabyte.fake");

			Assert.Equal("1MB", file.Size.ToPrettySize());
		}

		[Fact]
		public void GetDirectory_HeaderContent_ReturnsMarkdownFileAsHtml()
		{
			var contents = _fileService.GetDirectory();
			var header = contents.HeaderContent;

			Assert.Equal("<h1>Title</h1>", header);
		}

		[Fact]
		public void GetDirectory_GivenTxtHeaderContent_ReturnsTextFileContent()
		{
			var conf = new FileSystemConfiguration(_rootDir)
				.SetDirectoryHeaderFileName("_headerContent.txt")
				.Build();

			var fileService = new FileSystemService(conf);

			var contents = fileService.GetDirectory();
			var header = contents.HeaderContent;

			Assert.Equal("Title", header);
		}

		[Theory]
		[InlineData("kilobyte.fake")]
		[InlineData("level-one/level-two/fake-image.jpeg")]
		public void IsFile_ReturnsTrue(string path)
		{
			Assert.True(_fileService.IsFile(path));
		}

		[Theory]
		[InlineData("not.exists")]
		[InlineData("level-one")]
		public void IsFile_ReturnsFalse(string path)
		{
			Assert.False(_fileService.IsFile(path));
		}

		[Theory]
		[InlineData("level-one")]
		[InlineData("level-one/level-two")]
		public void IsDirectory_ReturnsTrue(string path)
		{
			Assert.True(_fileService.IsDirectory(path));
		}

		[Theory]
		[InlineData("kilobyte.fake")]
		[InlineData("nonexisting")]
		[InlineData("nonexisting.file")]
		public void IsDirectory_ReturnsFalse(string path)
		{
			Assert.False(_fileService.IsDirectory(path));
		}

		[Theory]
		[InlineData("kilobyte.fake")]
		[InlineData("level-one")]
		[InlineData("level-one/level-two")]
		public void IsAvailable_ReturnsTrue(string path)
		{
			Assert.True(_fileService.IsAvailable(path));
		}

		[Theory]
		[InlineData("web.config")]
		[InlineData("secret-extension.hidden")]
		[InlineData("_headerContent.md")]
		[InlineData("nonexisting")]
		public void IsAvailable__ReturnsFalse(string path)
		{
			Assert.False(_fileService.IsAvailable(path));
		}

		[Theory]
		[InlineData("", "/")]
		[InlineData("level-one", "/level-one")]
		[InlineData("..", "/..")]
		public void GetAbsoluteVirtualPath(string path, string expectedVirtualPath)
		{
			var fullFilesystemPath = Path.Combine(_configuration.Root.FullName, path);

			Assert.Equal(expectedVirtualPath, _fileService.GetAbsoluteVirtualPath(fullFilesystemPath));
		}

		[Theory]
		[InlineData("fake-image-unique.jpg")]
		[InlineData("fake-image-lvl-2.png")]
		public void FindFiles_should_find_on_exact_match(string pattern)
		{
			var result = _fileService.FindFiles(pattern);

			Assert.Equal(1, result.NrOfFiles);
		}

		[Theory]
		[InlineData("web.config")]
		[InlineData("*.hidden")]
		[InlineData("*.secret")]
		public void FindFiles_should_NOT_find_exluded_files(string pattern)
		{
			var result = _fileService.FindFiles(pattern);

			Assert.Equal(0, result.NrOfFiles);
		}

		[Theory]
		[InlineData("*unique.jpg")]
		[InlineData("*lvl-2.png")]
		public void FindFiles_should_find_on_wildcard(string pattern)
		{
			var result = _fileService.FindFiles(pattern);

			Assert.Equal(1, result.NrOfFiles);
		}

		[Theory]
		[InlineData("unique.jpg")]
		[InlineData("lvl-2.png")]
		public void FindFiles_should_find_on_implicit_wildcard(string pattern)
		{
			var result = _fileService.FindFiles(pattern);

			Assert.Equal(1, result.NrOfFiles);
		}

		[Theory]
		[InlineData("search")]
		[InlineData("search.ext")]
		[InlineData("*search.ext")]
		[InlineData("search.ext*")]
		[InlineData("*search.ext*")]
		public void CreateSearchPattern_should_find_on_implicit_wildcard(string pattern)
		{
			var searchPattern = ((FileSystemService)_fileService).CreateSearchPattern(pattern);

			Assert.StartsWith("*", searchPattern);
			Assert.EndsWith("*", searchPattern);
			Assert.Equal(2, searchPattern.Count(x => x == '*'));
		}
	}
}
