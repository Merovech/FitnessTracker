﻿using System;
using System.Windows.Markup;

namespace FitnessTracker.UI.Extensions
{
	// Taken from https://brianlagunas.com/a-better-way-to-data-bind-enums-in-wpf/
	public class EnumBindingSourceExtension : MarkupExtension
	{
		private Type _enumType;

		public Type EnumType
		{
			get => _enumType;
			set
			{
				if (value != _enumType)
				{
					if (null != value)
					{
						Type enumType = Nullable.GetUnderlyingType(value) ?? value;
						if (!enumType.IsEnum)
							throw new ArgumentException("Type must be for an Enum.");
					}

					_enumType = value;
				}
			}
		}

		public EnumBindingSourceExtension()
		{
		}

		public EnumBindingSourceExtension(Type enumType)
		{
			this.EnumType = enumType;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			if (null == this._enumType)
				throw new InvalidOperationException("The EnumType must be specified.");

			Type actualEnumType = Nullable.GetUnderlyingType(this._enumType) ?? this._enumType;
			Array enumValues = Enum.GetValues(actualEnumType);

			if (actualEnumType == this._enumType)
				return enumValues;

			Array tempArray = Array.CreateInstance(actualEnumType, enumValues.Length + 1);
			enumValues.CopyTo(tempArray, 1);
			return tempArray;
		}
	}
}
