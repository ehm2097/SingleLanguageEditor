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