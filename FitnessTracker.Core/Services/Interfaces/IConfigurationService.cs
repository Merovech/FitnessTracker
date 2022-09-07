﻿namespace FitnessTracker.Core.Services.Interfaces
{
	public interface IConfigurationService
	{
		public string DatabaseConnectionString { get; }

		public string SettingsFileName { get; }
	}
}
