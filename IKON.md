# Introduction #

IKON stands for Ivan Kravarščan's object notation. It's built upon [IKADN](IKADN.md), very similar to JSON and designed with following goals

  * Can present numeric and textual atoms, arrays and composite objects
  * Objects can be named and referred by name
  * Objects are tagged
  * Plain text (UTF-8) presentation
  * Human readable
  * Human editable
  * Minimum syntax overhead
  * Minimum formatting noise
  * Culture independent
  * Platform independent

# Data types #

IKON supports two simple data types, numeric and textual, two complex types, array and composite and references.

## Numeric ##

Numeric data is simple data that represents a single numeric value. There is no explicit limit to the range and precision of a value.

Sign: `=`
Tag: `"IKON.Numeric"` (string)

Examples:
| IKON | Explanation |
|:-----|:------------|
| `=123` |  |
| `=-123` | Negative number start with '-' sign |
| `=1234567` | No thousands separator allowed |
| `=123.456` | Decimal numbers have '.' as decimal separator |
| `=12.3e-6` | Scientific E notation is allowed |
| `=   -15` | White spaces before number are allowed |
| `=Inf` | Infinity|
| `=NaN` | Not a number |

White spaces between type sign ('=') and text representing a number are valid and don't impact numeric value. In order to simplify data presentation structure for human reader/editor, text representing a number may contain following classes of characters:

  * ASCII letters (a ... z, A ... Z)
  * ASCII digits (0 ... 9)
  * minus symbol (-)
  * period (.)

Validation whether such text is a numeric data is done after consuming (reading and advancing to the next character) the text from input stream. Textual representation is a valid numeric data if it is either number in scientific E notation or special value. In case of E notation, decimal separator is period (.) and thousands separator is not allowed. Special values are not a number (NaN), positive infinity (Inf) and negative infinity (-Inf). In both cases letter case is ignored.

## Quoted text ##

Quoted text is simple data that represents a sequence of UTF-8 characters, a text.

Sign: `"` (double quotation mark)

Tag: `"IKON.Text"` (string)

Examples:
| IKON | Explanation |
|:-----|:------------|
| `"Hello world!"` |  |
| `"Hello world!\nNew line"` | \n is substituted with new line character |
| `"Backslash: \\"` | \\ is substituted with a single slash |
| `"Double quote: \""` | \" is substituted with a quotation mark |

Much like C strings, quoted text data is bounded with unescaped double quotation mark and escape sequences start with a backslash. Purpose of escape sequences is to represent a control character (such as double quotation mark that would otherwise be treated as end of text), to represent text flow characters (such as new line) and to present other "difficult to type characters". List of escape sequences:

| Sequence | Explanation |
|:---------|:------------|
| `\\` | Backslash |
| `\"` | Double quotation |
| `\n` | New line |
| `\r` | Carrige return |
| `\t` | Horizontal tabulation |
| `\uXXXX` | UTF-16 character (X are hexadecimal digits) |
| `\UXXXXXXXX` | UTF-32 character |

## Text block ##

Text block too is a simple data that represents a sequence of UTF-8 characters.

Sign: `§` (section sign)

Tag: `"IKON.Text"` (string)

Example IKON data:
```
§ \s\s\s
   This is text block that spans 
   multiple lines.

   And is presumably easier to
   read and edit then quoted text.
\
```

Text block data consists of an indentation specification (optional), a textual content and a closing sign ("\").

Indentation specification specifies how is the textual content indented relative to the line with the opening sign ("§"). It is omitted, a single tab is presumed. Specification must be in the same line as the opening sign and consists of either `\s` (space) or `\t` (tab) codes, no white spaces between them. Example above has three spaces specified.

Textual content is indented raw text. Raw text is accepted as is (except indentation), there are no escape codes or other specially interpreted sequences. Indentation should be equal to indentation of the line with the opening sign plus characters in indentation specification. Extra indentation is treated as textual content.

Text block ends with a line containing closing sign and less indentation then textual content.

## Array ##

Array is complex data that represents an ordered sequence of nested IKADN data.

Sign: `[`

Tag: `"IKON.Array"` (string)

Examples:
| IKON | Explanation |
|:-----|:------------|
| `[=1 =2 =3]` |  |
| `[      =4`<br><code>  =7</code><br><code>  ]</code> <table><thead><th> white spaces around elements are allowed </th></thead><tbody>
<tr><td>  <code>[ =10 "foo" [ ] [ =Inf ] ]</code> </td><td> elements of different types are allowed </td></tr></tbody></table>

Arrays end with the matching right square bracket (matching with the left square bracket used as object's type sign). As shown in examples, content between brackets is treated as if it was in the root of IKADN document, white spaces between objects are ignored and no explicit separators between objects.<br>
<br>
<h2>Composite</h2>

Composite is complex data that represent mappings from identifier to IKADN object. Also, composites have a special identifier for a object's tag.<br>
<br>
Sign: <code>{</code>

Tag: data defined string<br>
<br>
Example:<br>
<table><thead><th> IKON </th><th> Explanation </th></thead><tbody>
<tr><td> <code>{ Person</code><br><code>    name "Peter"</code><br><code>    age =27</code><br><code>}</code> </td><td> Tag is "Person" </td></tr></tbody></table>

First identifier after type sign is the objects tag. It can be used as object class but end user can use it for other purposes. Tag identifier is followed by key-value pairs where key is an identifier and value . Composite ends with the matching right curly bracket. Identifier is a word like sequence of following characters:<br>
<br>
<ul><li>ASCII letters (a ... z, A ... Z), case sensitive<br>
</li><li>ASCII digits (0 ... 9)<br>
</li><li>underscore symbol (<code>_</code>)</li></ul>

Much like in IKON arrays, nested IKADN objects can have spaces around them and there is no explicit separator between key-value pairs.<br>
<br>
<h2>Reference</h2>

References on it's do not contain data, instead they refer to objects with data. Objects followed by anchor can be referred by anchor's name.<br>
<br>
Anchor sign: <code>@</code><br>
Reference sign: <code>#</code>

Anchor tag: <code>"IKON.Reference"</code> (string) used internally, should not be visible to user code<br>
Reference tag: not aplicable<br>
<br>
Examples:<br>
<table><thead><th> IKON </th><th> Explanation </th></thead><tbody>
<tr><td> <code>=3.14159 @pi</code><br><code>#pi</code> </td><td> Numeric object with anchor named "pi" and reference to that numeric object </td></tr>
<tr><td> <code>=255 @MaxByte @Count</code> </td><td> Multiple anchors are allowed </td></tr></tbody></table>

Anchor and reference names follow the same rules as identifier in composite objects.