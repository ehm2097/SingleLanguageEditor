namespace Avat.SingleLanguageEditor;

internal interface ITranslationUnit
{
    TranslationStates State { get; }
    string SourceText { get; }
    string TargetText { get; set; }

    IEnumerable<string> DeveloperNotes { get; }
    IEnumerable<string> GeneratorNotes { get; }

    Action? Confirm { get; }
    Action? Reject { get; }
    Action? Remove { get; }
    Action? Question { get; }
}