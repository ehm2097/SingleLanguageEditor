using System.Text;

namespace Avat.SingleLanguageEditor;

internal class TranslationUnitProxy
{
    public TranslationUnitProxy(object data, string fullId)
    {
        _fullId = fullId;
        _data = data;
        var rawParts = _fullId.Split('-').Select(p => p.Trim());
        foreach (var rawPart in rawParts)
        {
            var subParts = rawPart.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            _idParts.AddRange(subParts);
        }
    }

    public string FullId => _fullId;
    public object Data => _data;
    public int Level => _idParts.Count;

    public string GetIdPart(int level) => _idParts[level];

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach(var part in _idParts) { sb.AppendFormat("[{0}]", part); }
        return sb.ToString();
    }

    private readonly string _fullId;
    private readonly List<string> _idParts = new();
    private readonly object _data;
}