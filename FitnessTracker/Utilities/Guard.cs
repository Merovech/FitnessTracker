using System;

namespace FitnessTracker.Utilities
{
	internal static class Guard
	{
		internal static void AgainstNull(object item, string name)
		{
			if (item == null)
			{
				throw new ArgumentNullException($"'{name}' cannot be null.");
			}
		}

		internal static void AgsinstNull(object item, string message)
		{
			if (item == null)
			{
				throw new ArgumentNullException(message);
			}
		}
	}
}
