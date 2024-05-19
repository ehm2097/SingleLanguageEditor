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

