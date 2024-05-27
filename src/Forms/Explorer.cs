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
using Microsoft.Extensions.Configuration;

namespace Avat.SingleLanguageEditor.Forms;

public partial class Explorer : Form
{
    public Explorer(IConfiguration configuration)
    {
        _configuration = configuration;

        InitializeComponent();

        var initialFilePath = _configuration["TranslationFile"];
        if(initialFilePath != null) OpenTranslationFile(initialFilePath);
    }

    private readonly XLiffHandlerProxy _xliffHandler = new();
    private readonly TreeView _treeView = new();
    private readonly ListView _listView = new();
    private readonly ContextMenuconfigurator _contextMenuConfigurator = new();
    private readonly List<Action<bool>> _isfileOpenListeners = new();
    private TreeNode? _currentTreeNode = null;
    private ListViewItem? _currentListItem = null;
    private ITranslationUnit? _currentTranslationUnit = null;
    private TranslationFileView? _view;
    private readonly IConfiguration _configuration;

    private void MakeTranslationTree(TreeView treeView)
    {
        if(_xliffHandler.IsOpen)
        {
            var processor = new XliffProcessor(_xliffHandler);
            var root = processor.Process();
            _view = new TranslationFileView(root, _xliffHandler);
            treeView.Nodes.Add(_view.TreeRoot);
            treeView.SelectedNode = _view.TreeRoot;
        }
        else 
        {
            _view = null;
            treeView.Nodes.Clear();
        }
    }

    private ToolStripItem CreateMenuitem(string text, Func<ITranslationUnit, Action?> GetAction)
    {
        var item = new ToolStripMenuItem(text);
        
        item.Click += (object? _, EventArgs _) => {
            GetAction(_currentTranslationUnit!)!.Invoke();
            _currentListItem!.SubItems[1].Text = $"{_currentTranslationUnit!.State}";
            _currentListItem!.SubItems[3].Text = $"{_currentTranslationUnit!.TargetText}";
        };

        _contextMenuConfigurator.AddMenuItem(item, GetAction);
        return item;
    }

    private void UpdateListItem()
    {
        var translationUnit = _view!.GetTranslationUnit(_currentListItem);
        if(translationUnit != null)
        {
            _currentListItem!.SubItems[3].Text = translationUnit.TargetText;
        }
    }

    private void LaunchQuickEdit(Func<ITranslationUnit, bool> condition)
    {
        var translationUnits = GetTranslationUnits();
        if (translationUnits == null) return;

        var filteredTranslationUnits = translationUnits.Where(condition);

        if(filteredTranslationUnits.Any())
        {
            var editor = new Editor(filteredTranslationUnits, _configuration);
            editor.ShowDialog();
            UpdateListItem();
        }
    }

    private IEnumerable<ITranslationUnit>? GetTranslationUnits()
    {
        if (_currentListItem != null) return _view!.GetTranslationUnits(_currentListItem);
        if (_currentTreeNode != null) return _view!.GetTranslationUnits(_currentTreeNode);
        return null;
    }

    private bool AllowClosingTranslationFile()
    {
        if (!_xliffHandler.IsOpen) return true;
        if (!_xliffHandler.IsDirty) return true;
        var result = MessageBox.Show(
            "Save translation file", "Closing", 
            MessageBoxButtons.YesNoCancel, 
            MessageBoxIcon.Question);
        if(result == DialogResult.Cancel) return false;
        if(result == DialogResult.Yes) _xliffHandler.Save();
        return true;
    }

    private void OpenTranslationFile(string filePath)
    {
        if (_xliffHandler.IsOpen) OnCloseTranslationFile();
        _xliffHandler.Open(filePath);
        MakeTranslationTree(_treeView);
    }


    private void AfterSelectHandler(object? _, TreeViewEventArgs e)
    {
        _currentTreeNode = e.Node;
        if (_currentTreeNode == null) return;
        _listView.Items.Clear();
        _listView.Items.AddRange(_view!.CreateListItems(_currentTreeNode).ToArray());
    }

    private void OnOpenTranslationFile()
    {
        var dlg = new OpenFileDialog {
            Filter = "Translation files (*.xlf)|*.xlf|All files (*.*)|*.*",
            Multiselect = false
        };
        var result = dlg.ShowDialog();
        if (result != DialogResult.OK) return;

        OpenTranslationFile(dlg.FileName);
    }

    private void OnSaveTranslationFile(object? _1, EventArgs _2)
        => _xliffHandler.Save();

    private void OnSaveIntoDifferentFile(object? _1, EventArgs _2)
    {
        var dlg = new SaveFileDialog {
            Filter = "Translation files (*.xlf)|*.xlf|All files (*.*)|*.*"
        };
        var result = dlg.ShowDialog();
        if (result != DialogResult.OK) return;

        if (_xliffHandler.IsOpen) _xliffHandler.Save(dlg.FileName);
    }

    private void OnCloseTranslationFile()
    {
        if (!AllowClosingTranslationFile()) return;
        _xliffHandler.Close();
        MakeTranslationTree(_treeView);
    }

    private void AfterListSelectionChanged(object? sender, ListViewItemSelectionChangedEventArgs args)
    {
        _currentListItem = _listView.SelectedItems.OfType<ListViewItem>().FirstOrDefault();
        _currentTranslationUnit = _view!.GetTranslationUnit(_currentListItem);
    }

    private void BeforeOpenMenu(object? _, CancelEventArgs args)
    {
        args.Cancel = ! _contextMenuConfigurator.Apply(_currentTranslationUnit);
    }

    private EventHandler OnEditFromCurrentUnit() 
    {
        return (_1, _2) => { LaunchQuickEdit(u => true); };
    }

    private EventHandler OnEditFromCurrentUnit(Func<ITranslationUnit, bool> condition) 
    {
        return (_1, _2) => { LaunchQuickEdit(condition); };
    }

    private EventHandler OnFilterView() => OnFilterView(u => true);

    private EventHandler OnFilterView(Func<ITranslationUnit, bool> condition) 
    {
        return (_1, _2) => { /*LaunchQuickEdit(condition);*/ };
    }

    private void BeforeClosingForm(object? _1, FormClosingEventArgs args)
    {
        args.Cancel = !AllowClosingTranslationFile();
    }

    private void IsOpenFileChanged(object? _1, EventArgs _2)
    {
        foreach(var listener in _isfileOpenListeners) { listener(_xliffHandler.IsOpen); };   
    }
}
