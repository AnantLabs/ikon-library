﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Ikadn;
using Ikadn.Ikon.Values;
using Ikadn.Utilities;

namespace Ikadn.Ikon.Factories
{
	/// <summary>
	/// IKADN value factory for IKON  textual values.
	/// </summary>
	public class TextFactory : IIkadnObjectFactory
	{
		/// <summary>
		/// Sign for IKADN textual value.
		/// </summary>
		public const char OpeningSign = '"';

		/// <summary>
		/// Closing character for IKON textual value in textual
		/// representation.
		/// </summary>
		public const char ClosingChar = '"';

		const char EscapeChar = '\\';
		
		static Dictionary<char, char> EscapeCodes = DefineEscapeCodes();

		/// <summary>
		/// Sign for IKADN textual value.
		/// </summary>
		public char Sign
		{
			get { return OpeningSign; }
		}

		/// <summary>
		/// Parses input for a IKADN value.
		/// </summary>
		/// <param name="parser">IKADN parser instance.</param>
		/// <returns>IKADN value generated by factory.</returns>
		public IkadnBaseObject Parse(Ikadn.IkadnParser parser)
		{
			if (parser == null)
				throw new System.ArgumentNullException("parser");

			bool escaping = false;
			string text = parser.Reader.ReadConditionally(nextChar =>
			{
				char c = (char)nextChar;
				if (escaping) {
					escaping = false;
					if (!EscapeCodes.ContainsKey(c)) 
						throw new FormatException("Unsupported string escape sequence: \\" + nextChar);
					return new ReadingDecision(EscapeCodes[c], CharacterAction.Substitute);
				}
				switch (nextChar) {
					case EscapeChar:
						escaping = true;
						return new ReadingDecision(c, CharacterAction.Skip);
					case ClosingChar:
						return new ReadingDecision(c, CharacterAction.Stop);
					default:
						return new ReadingDecision(c, CharacterAction.AcceptAsIs);
				}
			});

			if (parser.Reader.Peek() != ClosingChar)
				throw new EndOfStreamException("Unexpected end of stream at " + parser.Reader.PositionDescription + " while reading IKON identifier.");
			parser.Reader.Read();

			return new TextValue(text);
		}

		private static Dictionary<char, char> DefineEscapeCodes()
		{
			Dictionary<char, char> res = new Dictionary<char, char>();

			res.Add('\\', '\\');
			res.Add('"', '"');
			res.Add('n', '\n');
			res.Add('r', '\r');
			res.Add('t', '\t');

			return res;
		}
	}
}
