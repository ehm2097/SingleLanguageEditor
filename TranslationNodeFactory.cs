using System.Text.RegularExpressions;

namespace Avat.SingleLanguageEditor;

internal class TranslationNodeFactory
{
    public TranslationNodeFactory(IXLiffHandler xliffHandler)
        => _xliffhandler = xliffHandler;

    public IEnumerable<ITranslationNode> Create(IEnumerable<TranslationUnitProxy> ids)
        => InternalCreate(ids, 0).Cast<ITranslationNode>();

    private string GetLocalName(string localId, TranslationUnitProxy unit, int level)
        => localId.Any(c => !char.IsDigit(c)) ? localId : GetLocalName(unit, level);

    private string GetLocalName(TranslationUnitProxy unit, int level)
    {
        var pattern = $"^{Regex.Replace(unit.FullId, @"\d+", @"(.+)")}$";
        foreach(var note in _xliffhandler.GetGeneratorNotes(unit.Data))
        {
            var match = Regex.Match(note, pattern);
            if(match.Success) { return match.Groups[level / 2 + 1].Value; } 
        }
        return "N/A";
    }

    private readonly IXLiffHandler _xliffhandler;

    private IEnumerable<TranslationNode> InternalCreate(IEnumerable<TranslationUnitProxy> ids, int level) 
        => ids
            .Where(i => i.Level > level)
            .GroupBy(i => i.GetIdPart(level))
            .Select(g => new TranslationNode(this, level, g));

    private class TranslationNode: ITranslationNode
    {
        public TranslationNode(TranslationNodeFactory factory, int level, IGrouping<string, TranslationUnitProxy> group)
        {
            var unit = group.First();

            _data = level == unit.Level - 1 ? unit.Data : null;
            _localId = group.Key;
            _localName = factory.GetLocalName(group.Key, unit, level);
            _subnodes = new(factory.InternalCreate(group, level + 1));
        }

        public object? Data => _data;
        public string LocalId => _localId;
        public string LocalName => _localName;
        public IEnumerable<ITranslationNode> Subnodes 
            => _subnodes.Cast<ITranslationNode>();

        private readonly object? _data;
        private readonly string _localId;
        private readonly string _localName;
        private readonly List<TranslationNode> _subnodes;
    }
}