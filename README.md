# SingleLanguageEditor
A wannabe replacement for the MultiLanguageEditor

## Dead keys configuration

Dead key mappings are defined in the PROFILE.JSON file that should be located in the same file as the executable. Each mapping definition should be added to the __$.profile.keyMappings__ array. 

A mapping definition consists of an object having two properties:
* __key__ is an array of keys that make up the key combination;
* __output__ is the UNICODE character produced by this key combination 

Keys should be defined within the __keys__ array in the same order as they are expected. Keys that require modifiers are prefixed with these modifiers (__Shift__, __Ctrl__ and __Alt__ are supported) and the __+__ (plus) sign. For a list of all key codes, see https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.keys.

Example: __["Ctrl+Oemcomma", "A"]__ means that __Ctrl-,__ is pressed first and then __A__.

The characters produced by the dead key mappings should be formatted as text representing the __\u__ UNICODE notation.

A sample file, __profile.ro_using_commas.json__ is included in the project, that can be used as is (by renaming it into __profile.json__) or used as a guide to define other dead key mappings. By default, it provides the following mappings - useful for Romanian language:
* Ctrl+, A produces the __ă__ character;
* Ctrl+, Shift+A produces the __Ă__ character;
* Ctrl+, S produces the __ş__ character;
* Ctrl+, Shift+S produces the __Ş__ character;
* Ctrl+, T produces the __ţ__ character;
* Ctrl+, Shift+T produces the __Ţ__ character;
 