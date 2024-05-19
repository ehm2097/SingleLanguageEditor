namespace MLE;

internal interface IXLiffHandler
{
    IEnumerable<string> GetGeneratorNotes(object unit);
    ITranslationUnit GetTranslationUnit(object data);
    IEnumerable<TranslationUnitProxy> GetTranslationUnits();
    void Save(string fileName);
}
