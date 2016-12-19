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
		private const string _excludedExtensions = ".hidden,.secret";
		private const string _excludedFilenames = "web.config";

		public FileSystemServiceTests()
		{
			var conf = new FileSystemConfiguration(Path.Combine(AppContext.BaseDirectory, "../../../_files"))
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
			var excludedExtensionsArray = _excludedExtensions.Split(',');

			Assert.False(contents.Files.Any(x => 
				excludedExtensionsArray.Contains(string.Concat(".", x.Extension))));
		}

		[Fact]
		public void GetDirectory_GivenRoot_ReturnNoHiddenFiles()
		{
			var contents = _fileService.GetDirectory();
			Assert.False(contents.Files.Any(x => x.Name == "web.config"));
			Assert.False(contents.Files.Any(x => x.Name == "_headerContent.md"));
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
            var conf = new FileSystemConfiguration(Path.Combine(AppContext.BaseDirectory, "../../../_files"))
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
	}
}
