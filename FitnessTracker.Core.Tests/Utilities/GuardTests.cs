using System;
using FitnessTracker.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FitnessTracker.Core.Tests.Utilities
{
	[TestClass]
	public class GuardTests
	{
		[TestMethod]
		public void AgainstNull_Should_Throw_On_Null_Item()
		{
			try
			{
				Guard.AgainstNull(null, "NullItem");
				Assert.Fail();
			}
			catch (ArgumentNullException ex)
			{
				Assert.AreEqual(ex.Message, "Value cannot be null. (Parameter 'NullItem')");
			}
		}

		[TestMethod]
		public void AgainstNull_Should_Not_Throw_On_Valid_Item()
		{
			try
			{
				Guard.AgainstNull(new object(), "NullItem");
			}
			catch (Exception)
			{
				Assert.Fail("Guard.AgainstNull should not throw on a non-null item.");
			}
		}
	}
}
