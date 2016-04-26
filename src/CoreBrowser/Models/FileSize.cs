using System;

namespace CoreBrowser.Models
{
	public class FileSize
	{
		private const long OneByte = 1;
		private const long OneKb = OneByte * 1024;
		private const long OneMb = OneKb * 1024;
		private const long OneGb = OneMb * 1024;
		private const long OneTb = OneGb * 1024;

		public long Length { get; private set; }

		public FileSize(long sizeInBytes)
		{
			this.Length = sizeInBytes > 0 ? sizeInBytes : 0;
		}

		public string ToPrettySize(int decimalPlaces = 0)
		{
			var asTb = Math.Round((double)Length / OneTb, decimalPlaces);
			var asGb = Math.Round((double)Length / OneGb, decimalPlaces);
			var asMb = Math.Round((double)Length / OneMb, decimalPlaces);
			var asKb = Math.Round((double)Length / OneKb, decimalPlaces);
			string chosenValue = asTb >= 1 ? string.Format("{0}Tb", asTb)
				: asGb >= 1 ? string.Format("{0}GB", asGb)
				: asMb >= 1 ? string.Format("{0}MB", asMb)
				: asKb >= 1 ? string.Format("{0}KB", asKb)
				: string.Format("{0}B", Math.Round((double)Length, decimalPlaces));
			return chosenValue;
		}
	}
}