// MIT License

// Copyright (c) 2024 Andrei Vatasescu

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.ComponentModel;
using System.Data;
using System.Globalization;
using Microsoft.Extensions.Configuration;

namespace Avat.SingleLanguageEditor.DeadKeys;

internal static class KeyComboDefConfig
{
    public static IEnumerable<KeyComboDef> Setup(IConfiguration configuration)
    {
        var section = configuration.GetSection("profile:keyMappings");
        var sectionData = section.Get<IEnumerable<KeyMapping>>()!;
        return sectionData.Select(ToComboDef);
    }   

    private static readonly TypeConverter _converter = TypeDescriptor.GetConverter(typeof(Keys));

    private static Keys ParseKey(string text)
    {
        var value = _converter.ConvertFromString(null, CultureInfo.InvariantCulture, text);
        return value is Keys key? key : Keys.None;
    }

    private static KeyComboDef ToComboDef(KeyMapping mapping) 
        => new(mapping.Keys.Select(ParseKey), mapping.Output[0]);


    private class KeyMapping
    {
        public KeyMapping(string[] keys, string output) 
            => (_keys, _output) = (keys, output);

        public IEnumerable<string> Keys => _keys;
        public string Output => _output;

        private readonly IEnumerable<string> _keys;
        private readonly string _output;
    }    
}

