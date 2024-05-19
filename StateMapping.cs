using System.Windows.Forms.VisualStyles;

namespace MLE;

internal class StateMapping
{
    public StateMapping()
    {
        AddMapping("needs-review-translation", TranslationStates.NeedsReview, true);
        AddMapping("needs-translation", TranslationStates.NeedsTranslation);
        AddMapping("new", TranslationStates.NeedsTranslation, true);
        AddMapping("translated", TranslationStates.Translated);
        AddMapping("final", TranslationStates.Translated, true);
    }

    public TranslationStates GetState(string textState)
        => _textToState.ContainsKey(textState) ? _textToState[textState] : TranslationStates.NotAvailable;

    public string GetText(TranslationStates state)
        => _stateToText.ContainsKey(state) ? _stateToText[state] : string.Empty;

    private readonly Dictionary<string, TranslationStates> _textToState = new();
    private readonly Dictionary<TranslationStates, string> _stateToText = new();

    private void AddMapping(string text, TranslationStates state) => AddMapping(text, state, false);

    private void AddMapping(string text, TranslationStates state, bool isDefault)
    {
        _textToState[text] = state;
        if(isDefault) _stateToText[state] = text;
    }
}