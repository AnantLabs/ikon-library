﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Ikon.Ston.Values;
using Ikon;

namespace Ikston_Unit_Tests
{
	[TestClass]
	public class ReferenceTests
	{
		static string NamingTestInput =
			"\"some text\" @text" + Environment.NewLine +
			"=3 @number" + Environment.NewLine +
			"=Infinity @notNumber" + Environment.NewLine +
			"[] @array" + Environment.NewLine +
			"{ NestedStuff" + Environment.NewLine +
			"\t" + "atr1 =5 @nestedNumber" + Environment.NewLine +
			"\t" + "atr2 \"more text\" @nestedText" + Environment.NewLine +
			"} @composite @otherName@noSpace";

		static string ReferencingTestInput =
			"\"the probe\" @probe" + Environment.NewLine +
			"#probe" + Environment.NewLine +
			"[" + Environment.NewLine +
			"\t" + "#probe" + Environment.NewLine +
			"]" + Environment.NewLine +
			"{ NestedStuff" + Environment.NewLine +
			"\t" + "child #probe" + Environment.NewLine +
			"}";

		[TestMethod]
		public void ReferenceNamingText()
		{
			Parser parser = new Ikon.Ston.Parser(new StringReader(NamingTestInput));
			var values = parser.ParseAll();

			Assert.AreEqual(TextValue.ValueTypeName, parser.GetNamedValue("text").TypeName);
		}

		[TestMethod]
		public void ReferenceNamingNumber()
		{
			Parser parser = new Ikon.Ston.Parser(new StringReader(NamingTestInput));
			var values = parser.ParseAll();

			Assert.AreEqual(NumericValue.ValueTypeName, parser.GetNamedValue("number").TypeName);
		}

		[TestMethod]
		public void ReferenceNamingInfinity()
		{
			Parser parser = new Ikon.Ston.Parser(new StringReader(NamingTestInput));
			var values = parser.ParseAll();

			Assert.AreEqual(NumericValue.ValueTypeName, parser.GetNamedValue("notNumber").TypeName);
		}

		[TestMethod]
		public void ReferenceNamingArray()
		{
			Parser parser = new Ikon.Ston.Parser(new StringReader(NamingTestInput));
			var values = parser.ParseAll();

			Assert.AreEqual(ArrayValue.ValueTypeName, parser.GetNamedValue("array").TypeName);
		}

		[TestMethod]
		public void ReferenceNamingComposite()
		{
			Parser parser = new Ikon.Ston.Parser(new StringReader(NamingTestInput));
			var values = parser.ParseAll();

			Assert.AreEqual("NestedStuff", parser.GetNamedValue("composite").TypeName);
		}

		[TestMethod]
		public void ReferenceNamingNestedNumber()
		{
			Parser parser = new Ikon.Ston.Parser(new StringReader(NamingTestInput));
			var values = parser.ParseAll();

			Assert.AreEqual(NumericValue.ValueTypeName, parser.GetNamedValue("nestedNumber").TypeName);
		}

		[TestMethod]
		public void ReferenceNamingNestedText()
		{
			Parser parser = new Ikon.Ston.Parser(new StringReader(NamingTestInput));
			var values = parser.ParseAll();

			Assert.AreEqual(TextValue.ValueTypeName, parser.GetNamedValue("nestedText").TypeName);
		}

		[TestMethod]
		public void ReferenceNamingSecondName()
		{
			Parser parser = new Ikon.Ston.Parser(new StringReader(NamingTestInput));
			var values = parser.ParseAll();

			Assert.AreEqual("NestedStuff", parser.GetNamedValue("otherName").TypeName);
		}

		[TestMethod]
		public void ReferenceNamingNoSpace()
		{
			Parser parser = new Ikon.Ston.Parser(new StringReader(NamingTestInput));
			var values = parser.ParseAll();

			Assert.AreEqual("NestedStuff", parser.GetNamedValue("noSpace").TypeName);
		}

		[TestMethod]
		public void ReferencingRootType()
		{
			Parser parser = new Ikon.Ston.Parser(new StringReader(ReferencingTestInput));
			var probe = parser.ParseNext();
			var rootValue = parser.ParseNext();

			Assert.AreEqual(TextValue.ValueTypeName, rootValue.TypeName);
		}

		[TestMethod]
		public void ReferencingRootValue()
		{
			Parser parser = new Ikon.Ston.Parser(new StringReader(ReferencingTestInput));
			var probe = parser.ParseNext();
			var rootValue = parser.ParseNext() as TextValue;

			Assert.AreEqual("the probe", rootValue.GetText);
		}

		[TestMethod]
		public void ReferencingArrayElement()
		{
			Parser parser = new Ikon.Ston.Parser(new StringReader(ReferencingTestInput));
			var probe = parser.ParseNext();
			var rootValue = parser.ParseNext();
			var array = parser.ParseNext() as ArrayValue;

			Assert.AreEqual("the probe", (array.GetList[0] as TextValue).GetText);
		}

		[TestMethod]
		public void ReferencingCompositionChild()
		{
			Parser parser = new Ikon.Ston.Parser(new StringReader(ReferencingTestInput));
			var probe = parser.ParseNext();
			var rootValue = parser.ParseNext();
			var array = parser.ParseNext() as ArrayValue;
			var composite = parser.ParseNext() as ObjectValue;

			Assert.AreEqual("the probe", (composite["child"] as TextValue).GetText);
		}


		[TestMethod]
		public void ReferenceWriteNameRoot()
		{
			string expected = "\"something\" @name";

			StringBuilder output = new StringBuilder();
			IkonWriter writer = new IkonWriter(new StringWriter(output));

			var value = new TextValue("something");
			value.ReferenceNames.Add("name");
			value.Compose(writer);

			Assert.AreEqual(expected, output.ToString().Trim());
		}

		[TestMethod]
		public void ReferenceWriteNameArray()
		{
			string expected = "[" + Environment.NewLine +
				"\t" + "\"something\" @name" + Environment.NewLine + 
				"]";

			StringBuilder output = new StringBuilder();
			IkonWriter writer = new IkonWriter(new StringWriter(output));

			var namedValue = new TextValue("something");
			namedValue.ReferenceNames.Add("name");

			var array = new ArrayValue();
			array.GetList.Add(namedValue);
			array.Compose(writer);

			Assert.AreEqual(expected, output.ToString().Trim());
		}

		[TestMethod]
		public void ReferenceWriteNameComposite()
		{
			string expected = "{ Composite" + Environment.NewLine +
				"\t" + "child \"something\" @name" + Environment.NewLine +
				"}";

			StringBuilder output = new StringBuilder();
			IkonWriter writer = new IkonWriter(new StringWriter(output));

			var namedValue = new TextValue("something");
			namedValue.ReferenceNames.Add("name");

			var composite = new ObjectValue("Composite");
			composite["child"] = namedValue;
			composite.Compose(writer);

			Assert.AreEqual(expected, output.ToString().Trim());
		}
	}
}