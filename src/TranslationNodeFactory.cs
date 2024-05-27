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