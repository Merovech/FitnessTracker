using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FitnessTracker.Core.Models;

namespace FitnessTracker.Core.Tests.Helpers
{
	class DailyRecordEqualityComparer : IEqualityComparer<DailyRecord>
	{
		public bool Equals(DailyRecord x, DailyRecord y)
		{
			return x.Date == y.Date &&
				x.Weight == y.Weight &&
				x.MovingWeightAverage == y.MovingWeightAverage;
		}

		public int GetHashCode([DisallowNull] DailyRecord obj)
		{
			// Algorithm taken from https://stackoverflow.com/a/263416/112829.
			// Prime numbers were chosen at random from https://primes.utm.edu/curios/index.php?start=5&stop=5.

			// Safe to overflow; we can just wrap with no problem.
			unchecked
			{
				int hash = 23029;
				hash = (15511 * hash) ^ obj.Date.GetHashCode();
				hash = (15511 * hash) ^ obj.Weight.GetHashCode();
				hash = (15511 * hash) ^ obj.MovingWeightAverage.GetHashCode();
				return hash;
			}
		}
	}
}
