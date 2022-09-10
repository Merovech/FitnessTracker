using System;

namespace FitnessTracker.Core
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
	public class DependencyInjectionTypeAttribute : Attribute
	{
		public DependencyInjectionType Type
		{
			get; private set;
		}

		public DependencyInjectionTypeAttribute(DependencyInjectionType type)
		{
			Type = type;
		}
	}
}
