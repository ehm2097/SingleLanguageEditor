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

namespace Avat.SingleLanguageEditor;

internal class XLiffHandlerProxy : IXLiffHandler, INotifyPropertyChanged
{
    public void Open(string fileName) 
    {
        _fileName = fileName;
        var wrapped = new XLiffHandler(_fileName);
        wrapped.HasChanged += (object? _1, EventArgs _2) => {
            _isDirty = true;
            NavigationFormTextChanged();
        };
        _wrapped = wrapped;
        _isDirty = false;
        NavigationFormTextChanged();
        DoIsFileOpenChanged();
    }

    public void Close()
    { 
        _wrapped = null;
        _fileName = null;
        _isDirty = false;
        NavigationFormTextChanged();
        DoIsFileOpenChanged();
    }

    public bool IsOpen => _wrapped != null;

    public bool IsDirty => _isDirty;

    public IEnumerable<string> GetGeneratorNotes(object unit) 
        => GetWrapped().GetGeneratorNotes(unit);

    public ITranslationUnit GetTranslationUnit(object data) 
        => GetWrapped().GetTranslationUnit(data);

    public IEnumerable<TranslationUnitProxy> GetTranslationUnits()
        => GetWrapped().GetTranslationUnits();

    public void Save(string fileName)
    { 
        GetWrapped().Save(fileName!);
        _fileName = fileName;
        _isDirty = false;
        NavigationFormTextChanged();
    }

    public void Save() => Save(_fileName!);

    public string NavigationFormText => GetNavigationFormText();
    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler? IsFileOpenChanged;

    private IXLiffHandler? _wrapped = null;
    private string? _fileName = null;
    private bool _isDirty = false;

    private const string BaseNavigationFormText = "Translation Browser";

    private IXLiffHandler GetWrapped()
    {
        if (_wrapped == null) throw new InvalidOperationException();
        return _wrapped;
    }

    private string GetNavigationFormText()
    {
        if (_wrapped == null) return BaseNavigationFormText;
        var dirtySymbol = _isDirty ? "*" : string.Empty;
        return $"{BaseNavigationFormText} - {_fileName}{dirtySymbol}";
    }

    private void NavigationFormTextChanged()
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NavigationFormText)));

    private void DoIsFileOpenChanged()
        => IsFileOpenChanged?.Invoke(this, EventArgs.Empty);
}