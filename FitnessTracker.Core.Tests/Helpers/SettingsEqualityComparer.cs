using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FitnessTracker.Core.Models;

namespace FitnessTracker.Core.Tests.Helpers
{
	public class SettingsEqualityComparer : IEqualityComparer<SystemSettings>
	{
		public bool Equals(SystemSettings x, SystemSettings y)
		{
			return x.IsDarkTheme == y.IsDarkTheme &&
				x.WeightGraphMaximum == y.WeightGraphMaximum &&
				x.WeightGraphMinimum == y.WeightGraphMinimum &&
				x.WeightUnit == y.WeightUnit;
		}

		public int GetHashCode([DisallowNull] SystemSettings obj)
		{
			// Algorithm taken from https://stackoverflow.com/a/263416/112829.
			// Prime numbers were chosen at random from https://primes.utm.edu/curios/index.php?start=5&stop=5.

			// Safe to overflow; we can just wrap with no problem.
			var hash = 32969;
			hash = (21707 * hash) ^ obj.IsDarkTheme.GetHashCode();
			hash = (21707 * hash) ^ obj.WeightGraphMaximum.GetHashCode();
			hash = (21707 * hash) ^ obj.WeightGraphMinimum.GetHashCode();
			hash = (21707 * hash) ^ obj.WeightUnit.GetHashCode();

			return hash;
		}
	}
}
