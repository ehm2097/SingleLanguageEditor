namespace MLE;

internal interface ITranslationNode
{
    string LocalId { get; }
    string LocalName { get; }
    object? Data { get; }
    IEnumerable<ITranslationNode> Subnodes { get; }
}