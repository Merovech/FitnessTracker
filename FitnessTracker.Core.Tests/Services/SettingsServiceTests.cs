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
using FitnessTracker.Core.Tests.Helpers.Builders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FitnessTracker.Core.Tests.Services
{
	[TestClass]
	public class SettingsServiceTests : TestBase<ISettingsService, SettingsServiceBuilder>
	{
		protected SettingsEqualityComparer Comparer { get; } = new();

		[TestInitialize]
		public virtual void InitializeTest()
		{
			Builder = new SettingsServiceBuilder();
			Target = Builder.Build();
		}

		[TestCleanup]
		public void CleanupTest()
		{
			if (File.Exists(Constants.TEST_SETTINGS_FILENAME))
			{
				File.Delete(Constants.TEST_SETTINGS_FILENAME);
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

			[TestMethod]
			[ExpectedException(typeof(ArgumentNullException))]
			public void Should_Fail_With_Null_Logger()
			{
				Builder.Logger = null;
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
				var readSettings = Target.ReadSettings();

				Assert.IsNotNull(readSettings);
				Assert.IsTrue(Comparer.Equals(settings, readSettings), "Incorrect settings were read from disk when no file exists.");
			}

			[TestMethod]
			public void Reading_Settings_When_No_File_Exists_Should_Create_One()
			{
				_ = Target.ReadSettings();
				Assert.IsTrue(File.Exists(Constants.TEST_SETTINGS_FILENAME), "No default settings file was created after trying to read from a nonexistent settings file.");
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

				Target.SaveSettings(settings);
				var readSettings = Target.ReadSettings();
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

				Target.SaveSettings(settings);
				var readSettings = Target.ReadSettings();
				Assert.IsTrue(Comparer.Equals(settings, readSettings), "Incorrect settings were read from disk.");

				settings.IsDarkTheme = false;
				Target.SaveSettings(settings);
				readSettings = Target.ReadSettings();
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
				Target.SaveSettings(settings);

				// Test that something wrote.
				Assert.IsTrue(File.Exists(Constants.TEST_SETTINGS_FILENAME), "File was not written successfully.");
				var fileInfo = new FileInfo(Constants.TEST_SETTINGS_FILENAME);
				Assert.AreNotEqual(0, fileInfo.Length, "No content was written to the file.");			
			}

			[TestMethod]
			[ExpectedException(typeof(ArgumentNullException))]
			public void Should_Throw_On_Null_Settings()
			{
				Target.SaveSettings(null);
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

				Target.SaveSettings(settings);

				var writtenSettings = Target.ReadSettings();
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

				Target.SaveSettings(settings);
				settings.IsDarkTheme = false;
				settings.WeightUnit = WeightUnit.Pounds;
				Target.SaveSettings(settings);

				var writtenSettings = Target.ReadSettings();
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

				Target.SaveSettings(settings);
				var contents = File.ReadAllText(Constants.TEST_SETTINGS_FILENAME);
				Assert.IsFalse(contents.Contains("weightGraphMinimum"), "Null value was written to the settings file.");
			}
		}
	}
}
