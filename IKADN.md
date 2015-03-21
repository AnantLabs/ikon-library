# Introduction #

IKADN stands for Ivan Kravarščan's abstract data notation. It is foundation for concrete solutions for textually representing data.

# Format #

All data notations built on top of IKADN have to comply to following rules for data container:

  * Data is represented as stream of UTF-8 characters
  * Stream of characters is finite
  * Stream contains zero or more valid objects (units of data)

And these rules for data layout:
  * Each object has to start with a certain sign (a non-white space character)
  * Each legal object format has to have unambiguous object boundaries
  * White spaces between objects are ignored

# Grammar #

Above data layout rules can be expressed with following grammar:

D → O`*`<br>
O → S<sub>0</sub>F<sub>0</sub><br>
O → S<sub>1</sub>F<sub>1</sub><br>⋮<br>
O → S<sub>n</sub>F<sub>n</sub><br>
<br>
Where D is a document (stream of characters), O an object, S<sub>i</sub> a sign, and F<sub>i</sub> a object format. It's up to concrete data notation to define a set of signs and formats for its data types.