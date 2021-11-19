using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessTracker.Core.Models;
using FitnessTracker.Core.Services.Implementations;
using FitnessTracker.Core.Services.Interfaces;
using FitnessTracker.Core.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FitnessTracker.Core.Tests.Services
{
	[TestClass]
	[TestCategory("SettingsService")]
	public class SettingsServiceTests
	{
		protected static string TEST_SETTINGS_FILENAME => "test-settings.json";

		protected SettingsServiceBuilder Builder
		{
			get;
			set;
		}

		protected ISettingsService Service
		{
			get;
			set;
		}

		protected SettingsEqualityComparer Comparer { get; } = new();

		[TestInitialize]
		public virtual void InitializeTest()
		{
			Builder = new SettingsServiceBuilder();
			Service = Builder.Build();
		}

		[TestCleanup]
		public void CleanupTest()
		{
			if (File.Exists(TEST_SETTINGS_FILENAME))
			{
				File.Delete(TEST_SETTINGS_FILENAME);
			}
		}

		[TestClass]
		public class ConstructorTests : SettingsServiceTests
		{
			[TestInitialize]
			public override void InitializeTest()
			{
				Builder = new SettingsServiceBuilder();
			}

			[TestMethod]
			public void Should_Initialize_Successfully_With_Valid_Arguments()
			{
				var service = Builder.Build();
				Assert.IsNotNull(service, "Failed to initialize correctly.");
			}

			[TestMethod]
			[ExpectedException(typeof(ArgumentNullException))]
			public void Should_Fail_With_Null_Config_Service()
			{
				Builder.ConfigurationService = null;
				_ = Builder.Build();
			}
		}

		[TestClass]
		public class ReadSettngsTests : SettingsServiceTests
		{
			[TestMethod]
			public void Should_Get_Default_Settings_When_File_Does_Not_Exist()
			{
				var settings = new SystemSettings();
				var readSettings = Service.ReadSettings();

				Assert.IsNotNull(readSettings);
				Assert.IsTrue(Comparer.Equals(settings, readSettings), "Incorrect settings were read from disk when no file exists.");
			}

			[TestMethod]
			public void Reading_Settings_When_No_File_Exists_Should_Create_One()
			{
				_ = Service.ReadSettings();
				Assert.IsTrue(File.Exists(TEST_SETTINGS_FILENAME), "No default settings file was created after trying to read from a nonexistent settings file.");
			}

			[TestMethod]
			public void Should_Successfully_Read_Settings()
			{
				// Pretty redundant with the verification portion of the WriteSettingsTests, but we still
				// should have a unit test here in case we ever need to change stuff.
				var settings = new SystemSettings
				{
					IsDarkTheme = true,
					WeightGraphMaximum = -300,
					WeightGraphMinimum = 300,
					WeightUnit = WeightUnit.Pounds
				};

				Service.SaveSettings(settings);
				var readSettings = Service.ReadSettings();
				Assert.IsTrue(Comparer.Equals(settings, readSettings), "Incorrect settings were read from disk.");
			}

			[TestMethod]
			public void Should_Successfully_Read_Changes()
			{
				var settings = new SystemSettings
				{
					IsDarkTheme = true,
					WeightGraphMaximum = -300,
					WeightGraphMinimum = 300,
					WeightUnit = WeightUnit.Pounds
				};

				Service.SaveSettings(settings);
				var readSettings = Service.ReadSettings();
				Assert.IsTrue(Comparer.Equals(settings, readSettings), "Incorrect settings were read from disk.");

				settings.IsDarkTheme = false;
				Service.SaveSettings(settings);
				readSettings = Service.ReadSettings();
				Assert.IsTrue(Comparer.Equals(settings, readSettings), "Incorrect settings were read from disk.");
			}
		}

		[TestClass]
		public class WriteSettingsTests : SettingsServiceTests
		{
			[TestMethod]
			public void Should_Write_Successfully_When_No_File_Exists()
			{
				var settings = new SystemSettings();
				Service.SaveSettings(settings);

				// Test that something wrote.
				Assert.IsTrue(File.Exists(TEST_SETTINGS_FILENAME), "File was not written successfully.");
				var fileInfo = new FileInfo(TEST_SETTINGS_FILENAME);
				Assert.AreNotEqual(0, fileInfo.Length, "No content was written to the file.");			
			}

			[TestMethod]
			[ExpectedException(typeof(ArgumentNullException))]
			public void Should_Throw_On_Null_Settings()
			{
				Service.SaveSettings(null);
			}

			[TestMethod]
			public void Should_Write_Correct_Settings()
			{
				var settings = new SystemSettings
				{
					IsDarkTheme = true,
					WeightUnit = WeightUnit.Kilograms,
					WeightGraphMinimum = -100,
					WeightGraphMaximum = 100
				};

				Service.SaveSettings(settings);

				var writtenSettings = Service.ReadSettings();
				Assert.IsTrue(Comparer.Equals(writtenSettings, settings), "Incorrect settings were written to disk.");
			}

			[TestMethod]
			public void Should_Overwrite_Existing_Settings()
			{
				var settings = new SystemSettings
				{
					IsDarkTheme = true,
					WeightUnit = WeightUnit.Kilograms,
					WeightGraphMinimum = -100,
					WeightGraphMaximum = 100
				};

				Service.SaveSettings(settings);
				settings.IsDarkTheme = false;
				settings.WeightUnit = WeightUnit.Pounds;
				Service.SaveSettings(settings);

				var writtenSettings = Service.ReadSettings();
				Assert.IsTrue(Comparer.Equals(writtenSettings, settings), "Settings were not overwritten on second write.");
			}

			[TestMethod]
			public void Should_Not_Write_Missing_Optional_Settings()
			{
				var settings = new SystemSettings
				{
					IsDarkTheme = true,
					WeightUnit = WeightUnit.Kilograms,
					WeightGraphMaximum = 100
				};

				Service.SaveSettings(settings);
				var contents = File.ReadAllText(TEST_SETTINGS_FILENAME);
				Assert.IsFalse(contents.Contains("weightGraphMinimum"), "Null value was written to the settings file.");
			}
		}

		protected class SettingsServiceBuilder
		{
			public IConfigurationService ConfigurationService
			{
				get;
				set;
			}

			public SettingsServiceBuilder()
			{
				CreateMocks();
			}

			public ISettingsService Build()
			{
				return new SettingsService(ConfigurationService);
			}

			private void CreateMocks()
			{
				var mock = new Mock<IConfigurationService>();
				mock.Setup(svc => svc.SettingsFilename).Returns(TEST_SETTINGS_FILENAME);
				ConfigurationService = mock.Object;
			}
		}
	}
}
