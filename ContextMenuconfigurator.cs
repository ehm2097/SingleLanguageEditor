using System.ComponentModel;

namespace Avat.SingleLanguageEditor;

internal class ContextMenuconfigurator
{
    public void AddMenuItem(ToolStripItem item, Func<ITranslationUnit, Action?> GetAction)
    {
        _configActions.Add(u => {
            var visible = (u != null) && (GetAction(u) != null);
            item.Visible = visible;
            return visible;
        });
    }

    public bool Apply(ITranslationUnit? translationUnit)
    {
        var isAnythingVisible = false;
        foreach(var action in _configActions) 
        { 
            isAnythingVisible |= action.Invoke(translationUnit); 
        }
        return isAnythingVisible;
    }

    private List<Func<ITranslationUnit?, bool>> _configActions = new();
}