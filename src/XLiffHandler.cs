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

using System.Xml;

namespace Avat.SingleLanguageEditor;

internal class XLiffHandler : IXLiffHandler
{
    public XLiffHandler(string fileName)
    {
        var table = new NameTable();
        _ns = new XmlNamespaceManager(table);
        _ns.AddNamespace("t", "urn:oasis:names:tc:xliff:document:1.2");

        _doc = new XmlDocument();
        _doc.Load(fileName);
    }

    public void Save(string fileName) => _doc.Save(fileName);

    public IEnumerable<TranslationUnitProxy> GetTranslationUnits() => _doc
        .SelectNodes("/t:xliff/t:file/t:body/t:group/t:trans-unit", _ns)!
        .Cast<XmlElement>()
        .Select(e => new TranslationUnitProxy(e, e.GetAttribute("id")));

    public IEnumerable<string> GetGeneratorNotes(object unit) => ((XmlElement)unit)
        .SelectNodes("./t:note[@from='Xliff Generator']", _ns)!
        .OfType<XmlElement>()
        .Select(n => n.InnerText);

    public ITranslationUnit GetTranslationUnit(object data)
        => new TranslationUnit((XmlElement)data, this);

    public event EventHandler? HasChanged;

    private readonly XmlDocument _doc;
    private readonly XmlNamespaceManager _ns;
    private readonly StateMapping _stateMapping = new();

    private void ApplyCandidateTranslation(string sourceText, string targetText)
    {
        var candidateTargets = _doc
            .SelectNodes("/t:xliff/t:file/t:body/t:group/t:trans-unit", _ns)!
            .Cast<XmlElement>()
            .Select(e => new TranslationUnit(e, this))
            .Where(u => u.SourceText.Equals(sourceText, StringComparison.CurrentCultureIgnoreCase))
            .Where(u => u.State == TranslationStates.NeedsTranslation);
        foreach(var candidateTarget in candidateTargets)
        {
            candidateTarget.CandidateTargetText = targetText;
        }
    }

    private class TranslationUnit : ITranslationUnit
    {
        public TranslationUnit(XmlElement xmlUnit, XLiffHandler handler)
        {
            _handler = handler;
            _xmlSource = (XmlElement)xmlUnit.SelectSingleNode("./t:source", _handler._ns)!;
            _xmlTarget = xmlUnit.SelectSingleNode("./t:target", _handler._ns) as XmlElement;

            foreach (XmlElement xmlNote in xmlUnit.SelectNodes("./t:note", _handler._ns)!)
            {
                var note = new TranslationNote(xmlNote.GetAttribute("from"), xmlNote.InnerText,
                    int.Parse(xmlNote.GetAttribute("priority")));
                _notes.Add(note);
            }
        }

        public TranslationStates State => GetState();
        public string SourceText => _xmlSource.InnerText;
        public string TargetText { get => GetTargetText(); set => SetTargetText(value); }
        public string CandidateTargetText { get => GetTargetText(); set => SetCandidateTargetText(value); }

        public IEnumerable<string> DeveloperNotes => _notes
            .Where(n => n.From.Equals("Developer"))
            .Where(n => !string.IsNullOrEmpty(n.Text))
            .OrderBy(n => n.Priority)
            .Select(n => n.Text);

        public IEnumerable<string> GeneratorNotes => _notes
            .Where(n => n.From.Equals("Xliff Generator"))
            .Where(n => !string.IsNullOrEmpty(n.Text))
            .OrderBy(n => n.Priority)
            .Select(n => n.Text);

        public Action? Confirm => GetConfirmAction();
        public Action? Reject => GetRejectAction();
        public Action? Remove => GetRemoveAction();
        public Action? Question => GetQuestionAction();

        private readonly XLiffHandler _handler;
        private readonly XmlElement _xmlSource;
        private readonly XmlElement? _xmlTarget;
        private readonly List<TranslationNote> _notes = new();

        private string GetTargetText()
            => _xmlTarget != null ? _xmlTarget.InnerText : string.Empty;

        private void SetTargetText(string value)
        {
            _xmlTarget!.InnerText = value;
            _xmlTarget!.SetAttribute("state", _handler._stateMapping.GetText(TranslationStates.Translated));
            _handler.ApplyCandidateTranslation(SourceText, value);
            _handler.HasChanged?.Invoke(this, new EventArgs());
        }

        private void SetCandidateTargetText(string value)
        {
            _xmlTarget!.InnerText = value;
            _xmlTarget!.SetAttribute("state", _handler._stateMapping.GetText(TranslationStates.NeedsReview));
            _handler.HasChanged?.Invoke(this, new EventArgs());
        }

        private TranslationStates GetState()
        {
            if (_xmlTarget == null) return TranslationStates.NeedsTranslation;
            var textState = _xmlTarget.GetAttribute("state");
            return _handler._stateMapping.GetState(textState);
        }

        private Action? GetConfirmAction()
        {
            if (GetState() != TranslationStates.NeedsReview) return null;
            else return () =>
            {
                _xmlTarget!.SetAttribute("state", _handler._stateMapping.GetText(TranslationStates.Translated));
            _handler.HasChanged?.Invoke(this, new EventArgs());
            };
        }

        private Action? GetRejectAction()
        {
            if (GetState() != TranslationStates.NeedsReview) return null;
            else return () =>
            {
                _xmlTarget!.SetAttribute("state", _handler._stateMapping.GetText(TranslationStates.NeedsTranslation));
                _xmlTarget.InnerText = "";
            _handler.HasChanged?.Invoke(this, new EventArgs());
            };
        }

        private Action? GetRemoveAction()
        {
            if (GetState() != TranslationStates.Translated) return null;
            else return () =>
            {
                _xmlTarget!.SetAttribute("state", _handler._stateMapping.GetText(TranslationStates.NeedsTranslation));
                _xmlTarget.InnerText = "";
            _handler.HasChanged?.Invoke(this, new EventArgs());
            };
        }

        private Action? GetQuestionAction()
        {
            if (GetState() != TranslationStates.Translated) return null;
            else return () =>
            {
                _xmlTarget!.SetAttribute("state", _handler._stateMapping.GetText(TranslationStates.NeedsReview));
            _handler.HasChanged?.Invoke(this, new EventArgs());
            };
        }
    }

    private class TranslationNote
    {
        public TranslationNote(string from, string text, int priority)
            => (From, Text, Priority) = (from, text, priority);

        public readonly string From;
        public readonly string Text;
        public readonly int Priority;
    }
}