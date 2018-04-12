namespace CoreBrowser
{
	public class CoreBrowserConfiguration
	{
		public string FilesRootFolder { get; set; }
		public string GaTrackingUA { get; set; }
		public string TitleSuffix { get; set; }
		public string CharactersWhitespaceTokens { get; set; }
		public string[] ExcludedFileExtension { get; set; }
		public string[] ExcludedFileNames { get; set; }
		public string DirectoryHeaderFileName { get; set; }
	}
}
