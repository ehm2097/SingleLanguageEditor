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

using Microsoft.Extensions.Configuration;
using Avat.SingleLanguageEditor.DeadKeys;

namespace Avat.SingleLanguageEditor.Forms;

internal partial class Editor : Form
{
    public Editor(IEnumerable<ITranslationUnit> translationUnits, IConfiguration configuration)
    {
        _dkManager = new(configuration);
        _translationUnits = translationUnits.GetEnumerator();
        _translationUnits.MoveNext();

        InitializeComponent();

        ShowTranslationUnit();
    }

    private readonly IEnumerator<ITranslationUnit> _translationUnits;
    private readonly DeadKeyManager _dkManager;

    private void ShowTranslationUnit()
    {
        var translationUnit = _translationUnits.Current!;
        var notesText = FormatNotes(translationUnit);

        var labels = new QualifiedText[] { new ("SOURCE", translationUnit.SourceText) }
            .Union(translationUnit.GeneratorNotes.Select(note => new QualifiedText("TOOL", note)))
            .Union(translationUnit.DeveloperNotes.Select(note => new QualifiedText("DEV", note)));
        Console.WriteLine(translationUnit.DeveloperNotes.Count());

        _sourceLabels.TextItems = labels; 
        _targetText.Text = translationUnit.TargetText;
        _targetText.Focus();
        _dkManager.Reset();
    }

    private void Advance()
    {
        if(_translationUnits.MoveNext()) ShowTranslationUnit();
        else Close();
    }

    private static string FormatNotes(ITranslationUnit unit) 
        => Array.Empty<string>()
            .Union(FormatNotes("DEVELOPER", unit.DeveloperNotes))
            .Union(FormatNotes("GENERATOR", unit.GeneratorNotes))
            .Aggregate(string.Empty, (text, item) => text.Length == 0 ? item : $"{text}\r\n{item}");
    
    private static IEnumerable<string> FormatNotes(string prefix, IEnumerable<string> items) 
        => items.Select(n => $"[{prefix}] {n}");

    private void OnClickOkButton(object _1, EventArgs _2)
    {
        _translationUnits.Current.TargetText = _targetText.Text;
        Advance();
    }

    private void OnClickSkipButton(object _1, EventArgs _2)
    {
        Advance();
    }

    private void HandleKeyDown(object? sender, KeyEventArgs args)
    {
        if(_dkManager.Put(args.KeyData))
        {
            args.SuppressKeyPress = true;
            var actualChar = _dkManager.Get();
            if (actualChar != null) SendKeys.Send($"{actualChar.Value}");
        }
        else 
        {
            args.SuppressKeyPress = false;
        }
    }

    private void SourceLabelsUseText(object? sender, UseTextEventArgs args)
    {
        _targetText.Text = args.Text;
    }
}
