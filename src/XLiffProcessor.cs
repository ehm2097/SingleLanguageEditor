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

namespace Avat.SingleLanguageEditor;

internal class XliffProcessor
{
    public XliffProcessor(IXLiffHandler xliffHandler) 
        => _xliffHandler = xliffHandler;

    public ITranslationNode Process()
    {
        var ids = _xliffHandler.GetTranslationUnits();

        var nodeFactory = new TranslationNodeFactory(_xliffHandler);
        var nodes = nodeFactory.Create(ids);

        return new TranslationRoot(nodes);
    }

    private static void ShowNodes(IEnumerable<ITranslationNode> nodes, string indent)
    {
        foreach(var node in nodes)
        {
            Console.WriteLine($"{indent}{node.LocalId}");
            ShowNodes(node.Subnodes, $"{indent}   ");
        }
    }

    private class TranslationRoot: ITranslationNode
    {
        public TranslationRoot(IEnumerable<ITranslationNode> nodes)
            => _subnodes = nodes;

        public string LocalId => "Translations";
        public string LocalName => LocalId;
        public object? Data => null;
        public IEnumerable<ITranslationNode> Subnodes => _subnodes;

        private readonly IEnumerable<ITranslationNode> _subnodes;
    }

    private readonly IXLiffHandler _xliffHandler;
}