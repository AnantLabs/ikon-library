﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

using Ikadn.Ikon.Types;

namespace Ikadn.Ikon.Factories
{
	/// <summary>
	/// IKADN object factory for IKON numeric objects.
	/// </summary>
	public class NumericFactory : IIkadnObjectFactory
	{
		/// <summary>
		/// Sign for IKADN numeric object.
		/// </summary>
		public const char OpeningSign = '=';

		/// <summary>
		/// Number format used for IKON numeric objects.
		/// </summary>
		public static IFormatProvider NumberFormat { get; private set; }

		/// <summary>
		/// Valid styles for IKON numeric objects.
		/// </summary>
		public static readonly NumberStyles NumberStyle = NumberStyles.Float;

		/// <summary>
		/// Sign for IKADN numeric object.
		/// </summary>
		public char Sign
		{
			get { return OpeningSign; }
		}

		/// <summary>
		/// Parses input for a IKADN object.
		/// </summary>
		/// <param name="parser">IKADN parser instance.</param>
		/// <returns>IKADN object generated by factory.</returns>
		public IkadnBaseObject Parse(Ikadn.IkadnParser parser)
		{
			if (parser == null)
				throw new ArgumentNullException("parser");

			parser.Reader.SkipWhiteSpaces();
			if (!parser.Reader.HasNext)
				throw new EndOfStreamException("Trying to read beyond the end of stream. Last read character was at " + parser.Reader.PositionDescription + ".");

			string startPosition = parser.Reader.PositionDescription;
			string numberText = parser.Reader.ReadWhile(ValidChars).Trim();
			if (numberText.Length == 0)
				throw new FormatException("Unexpected character at " + parser.Reader.PositionDescription + ", while reading IKON numeric value");

			if (!SpecialValues.Contains(numberText)) {
				decimal tempD;
				double tempF;
				long tempI;
				if (!decimal.TryParse(numberText, NumberStyle, NumberFormat, out tempD) &&
					!double.TryParse(numberText, NumberStyle, NumberFormat, out tempF) &&
					long.TryParse(numberText, NumberStyle, NumberFormat, out tempI) ||
					!NumberMatcher.IsMatch(numberText)) {
						throw new FormatException("Characters from " + startPosition + " to " + parser.Reader.PositionDescription + " couldn't be parsed as IKON numeric value");
				}
			}

			return new IkonNumeric(numberText);
		}

		static ICollection<char> ValidChars = new HashSet<char>(DefineValidChars());
		static ICollection<string> SpecialValues = new HashSet<string>(new string[]{
			IkonNumeric.PositiveInfinity,
			IkonNumeric.NegativeInfinity,
			IkonNumeric.NotANumber});
		static Regex NumberMatcher = new Regex("[\\+\\-]?[0-9\\.eE]+");

		private static IEnumerable<char> DefineValidChars()
		{
			yield return '-';
			yield return '.';

			for (char c = 'a'; c <= 'z'; c++) yield return c;
			for (char c = 'A'; c <= 'Z'; c++) yield return c;
			for (char c = '0'; c <= '9'; c++) yield return c;
		}

		static NumericFactory()
		{
			NumberFormat = NumberFormatInfo.InvariantInfo;
		}
	}
}
