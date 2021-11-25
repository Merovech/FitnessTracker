using FitnessTracker.Core.ImportPreparer.Implementations;
using FitnessTracker.Core.ImportPreparer.Interfaces;

namespace FitnessTracker.Core.Tests.Helpers.Builders
{
	public class SqliteImportPreparerBuilder : IBuilder<IImportPreparer>
	{
		public IImportPreparer Build()
		{
			return new SqliteImportPreparer();
		}
	}
}
