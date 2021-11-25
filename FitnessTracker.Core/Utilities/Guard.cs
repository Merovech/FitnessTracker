using System;
using System.Collections.Generic;
using System.Linq;

namespace FitnessTracker.Utilities
{
	public static class Guard
	{
		public static void AgainstNull(object item, string name)
		{
			if (item == null)
			{
				throw new ArgumentNullException(name);
			}
		}

		public static void AgainstEmptyList<T>(IEnumerable<T> list, string name)
		{
			if (!list.Any())
			{
				throw new InvalidOperationException($"'{name}' cannot be empty.");
			}
		}

		public static void AgainstEmptyString(string item, string name)
		{
			if (item == string.Empty)
			{
				throw new InvalidOperationException($"'{name}' cannot be empty.");
			}
		}
	}
}
