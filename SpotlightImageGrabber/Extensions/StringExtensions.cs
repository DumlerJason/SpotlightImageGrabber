namespace SpotlightImageGrabber
{
	public static class StringExtensions
	{
		public static bool ContainsCaseInsensitive(this string source, string value)
		{
			if (source == null && value == null) return true;
			if (source == null || value == null) return false;
			return source.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
		}

		public static bool EqualsCaseInsensitive(this string source, string value)
		{
			if (source == null && value == null) return true;
			if (source == null || value == null) return false;
			return string.Equals(source, value, StringComparison.OrdinalIgnoreCase);
		}

	}
}
