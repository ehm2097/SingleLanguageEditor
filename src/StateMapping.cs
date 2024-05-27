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