using System;

namespace FitnessTracker.Utilities
{
	public static class Guard
	{
		public static void AgainstNull(object item, string name)
		{
			if (item == null)
			{
				throw new ArgumentNullException($"'{name}' cannot be null.");
			}
		}
	}
}
