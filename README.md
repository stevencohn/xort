**xort**

Sorts the contents of an XML file based on the contents of a given template file.
This can be used to align two versions of an XML file to enable textual comparison
to find differences between the versions.

Sorting is based on element name plus common attribute names and values.

Usage: **xort** _template.xml_ _unsorted.xml_ [debug]

The template.xml doesn't not need to have data, just enough elements to give structure
to the file. See the template.xml file included in this repo.

The optional debug argument indicates that you want the results spit out to the console
rather than to an output file.
