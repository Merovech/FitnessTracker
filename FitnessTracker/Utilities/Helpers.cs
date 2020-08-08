namespace FitnessTracker.Utilities
{
	internal static class Helpers
	{
		internal delegate bool TryParseFunc<T>(string input, out T value);

		internal static bool TryParse<T>(string input, TryParseFunc<T> parseFunc, out T? outValue) where T : struct
		{
			if (string.IsNullOrEmpty(input))
			{
				outValue = null;
				return true;
			}

			var result = parseFunc(input, out T outVal);
			outValue = outVal;
			return result;
		}
	}
}
