
namespace Avat.SingleLanguageEditor;

internal class XliffProcessor
{
    public XliffProcessor(IXLiffHandler xliffHandler) 
        => _xliffHandler = xliffHandler;

    public ITranslationNode Process()
    {
        // var xliffHandler = new XLiffHandler("test.xlf");

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