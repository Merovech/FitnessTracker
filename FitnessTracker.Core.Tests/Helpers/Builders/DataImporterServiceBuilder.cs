using System.Collections.Generic;
using System.Threading.Tasks;
using FitnessTracker.Core.ImportPreparer.Interfaces;
using FitnessTracker.Core.Models;
using FitnessTracker.Core.Services.Implementations;
using FitnessTracker.Core.Services.Interfaces;
using Moq;

namespace FitnessTracker.Core.Tests.Helpers.Builders
{
	public class DataImporterServiceBuilder : IBuilder<IDataImporterService>
	{
		private Mock<IDatabaseService> _databaseServiceMock;
		private Mock<IImportPreparerFactory> _importPreparerFactoryMock;
		private Mock<IImportPreparer> _importPreparerMock;

		public static int ImportedRecordsCount => 50;

		public IDatabaseService DatabaseService
		{
			get;
			set;
		}

		public IImportPreparerFactory ImportPreparerFactory
		{
			get;
			set;
		}

		public DataImporterServiceBuilder()
		{
			SetupMocks(true);
		}

		public IDataImporterService Build()
		{
			return new DataImporterService(DatabaseService, ImportPreparerFactory);
		}

		public IDataImporterService BuildWithNoImportPreparer()
		{
			SetupMocks(false);
			return new DataImporterService(DatabaseService, ImportPreparerFactory);
		}

		public void VerifyUpsertIsCalled(Times callTimes)
		{
			_databaseServiceMock.Verify(svc => svc.UpsertRecords(It.IsAny<IEnumerable<DailyRecord>>()), callTimes);
		}

		public void VerifyGetImporterFactoryIsCalled(Times callTimes)
		{
			_importPreparerFactoryMock.Verify(f => f.GetImportPreparer(It.IsAny<FileType>()), callTimes);
		}

		public void VerifyGetRecordsIsCalled(Times callTimes)
		{
			_importPreparerMock.Verify(p => p.GetRecords(It.IsAny<string>()), callTimes);
		}

		private void SetupMocks(bool includePreparer)
		{
			_databaseServiceMock = new Mock<IDatabaseService>();
			_importPreparerFactoryMock = new Mock<IImportPreparerFactory>();
			_importPreparerMock = new Mock<IImportPreparer>();

			if (includePreparer)
			{
				_importPreparerMock.Setup(m => m.GetRecords(It.IsAny<string>()))
					.Returns(Task.FromResult((IEnumerable<DailyRecord>)TestDataGenerator.GenerateRandomRecords(ImportedRecordsCount)));

				_importPreparerFactoryMock.Setup(m => m.GetImportPreparer(It.IsAny<FileType>()))
					.Returns(_importPreparerMock.Object);
			}

			DatabaseService = _databaseServiceMock.Object;
			ImportPreparerFactory = _importPreparerFactoryMock.Object;
		}
	}
}
