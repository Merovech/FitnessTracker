using System.Globalization;
using System.Windows.Controls;

namespace FitnessTracker.ValidationRules
{
	public class MinimumNotEmptyValidationRule : ValidationRule
	{
		private const double MINIMUM = 0;
		private string _message = $"Minimum { MINIMUM }";

		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			if (string.IsNullOrEmpty(value.ToString()))
			{
				return new ValidationResult(false, _message);
			}

			if (!double.TryParse(value.ToString(), NumberStyles.AllowDecimalPoint, cultureInfo, out double result))
			{
				return new ValidationResult(false, _message);
			}

			if (result < MINIMUM)
			{
				return new ValidationResult(false, _message);
			}

			return ValidationResult.ValidResult;
		}
	}
}
