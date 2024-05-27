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

namespace Avat.SingleLanguageEditor.Forms;

partial class Explorer
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) { components.Dispose(); }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();

        var mainMenu = new MenuStrip();
        mainMenu.Dock = DockStyle.Top;

        var fileMenu = new ToolStripMenuItem("File");
        mainMenu.Items.Add(fileMenu);

        var openFileMenu = new ToolStripMenuItem("Open...");
        fileMenu.DropDownItems.Add(openFileMenu);

        var saveFileMenu = new ToolStripMenuItem("Save");
        _isfileOpenListeners.Add(b => { saveFileMenu.Enabled = b; });
        fileMenu.DropDownItems.Add(saveFileMenu);

        var saveAsFileMenu = new ToolStripMenuItem("Save As...");
        _isfileOpenListeners.Add(b => { saveAsFileMenu.Enabled = b; });
        fileMenu.DropDownItems.Add(saveAsFileMenu);

        var closeFileMenu = new ToolStripMenuItem("Close");
        _isfileOpenListeners.Add(b => { closeFileMenu.Enabled = b; });
        fileMenu.DropDownItems.Add(closeFileMenu);

        var exitFileMenu = new ToolStripMenuItem("Exit");
        fileMenu.DropDownItems.Add(exitFileMenu);

        var editMenu = new ToolStripMenuItem("Edit");
        mainMenu.Items.Add(editMenu);

        var quickEditMenu = new ToolStripMenuItem("Quick edit");
        editMenu.DropDownItems.Add(quickEditMenu);

        var allQuickEditMenu = new ToolStripMenuItem("All translations");
        allQuickEditMenu.Click += OnEditFromCurrentUnit();
        quickEditMenu.DropDownItems.Add(allQuickEditMenu);

        var pendingQuickEditMenu = new ToolStripMenuItem("Translations to review");
        pendingQuickEditMenu.Click += OnEditFromCurrentUnit(u => u.State != TranslationStates.Translated);
        quickEditMenu.DropDownItems.Add(pendingQuickEditMenu);

        var missingQuickEditMenu = new ToolStripMenuItem("Missing translations");
        missingQuickEditMenu.Click += OnEditFromCurrentUnit(u => u.State == TranslationStates.NeedsTranslation);
        quickEditMenu.DropDownItems.Add(missingQuickEditMenu);

        var viewMenu = new ToolStripMenuItem("View");
        mainMenu.Items.Add(viewMenu);

        var allViewMenu = new ToolStripMenuItem("All translations");
        allViewMenu.CheckOnClick = true;
        allViewMenu.Checked = true;
        allViewMenu.Click += OnFilterView();
        viewMenu.DropDownItems.Add(allViewMenu);

        var pendingViewMenu = new ToolStripMenuItem("Translations to review");
        pendingViewMenu.CheckOnClick = true;
        pendingViewMenu.Click += OnFilterView(u => u.State != TranslationStates.Translated);
        viewMenu.DropDownItems.Add(pendingViewMenu);

        var missingViewMenu = new ToolStripMenuItem("Missing translations");
        missingViewMenu.CheckOnClick = true;
        missingQuickEditMenu.Click += OnFilterView(u => u.State == TranslationStates.NeedsTranslation);
        viewMenu.DropDownItems.Add(missingViewMenu);

        MainMenuStrip = mainMenu;

        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(800, 450);
        DataBindings.Add(new Binding("Text", _xliffHandler, nameof(XLiffHandlerProxy.NavigationFormText)));

        var trUnitMenu = new ContextMenuStrip();
        trUnitMenu.Opening += BeforeOpenMenu;
        trUnitMenu.Items.Add(CreateMenuitem("Confirm translation", u => u.Confirm));
        trUnitMenu.Items.Add(CreateMenuitem("Reject translation", u => u.Reject));
        trUnitMenu.Items.Add(CreateMenuitem("Remove translation", u => u.Remove));
        trUnitMenu.Items.Add(CreateMenuitem("Question translation", u => u.Question));

        var splitContainer = new SplitContainer();
        splitContainer.FixedPanel = FixedPanel.Panel1;
        splitContainer.Dock = DockStyle.Fill;
        Controls.Add(splitContainer);

        _treeView.Dock = DockStyle.Fill;
        MakeTranslationTree(_treeView);
        _treeView.AfterSelect += AfterSelectHandler;
        splitContainer.Panel1.Controls.Add(_treeView);

        _listView.View = View.Details;
        _listView.Dock = DockStyle.Fill;
        _listView.Columns.Add("ID", 200, HorizontalAlignment.Left);
        _listView.Columns.Add("State", 100, HorizontalAlignment.Left);
        _listView.Columns.Add("Source", 400, HorizontalAlignment.Left);
        _listView.Columns.Add("Target", 400, HorizontalAlignment.Left);
        _listView.ContextMenuStrip = trUnitMenu;
        _listView.MultiSelect = false;
        _listView.ItemSelectionChanged += AfterListSelectionChanged;
        splitContainer.Panel2.Controls.Add(_listView);

        Controls.Add(mainMenu);

        openFileMenu.Click += (object _1, EventArgs _2) => { OnOpenTranslationFile(); };
        closeFileMenu.Click += (object _1, EventArgs _2) => { OnCloseTranslationFile(); };
        saveAsFileMenu.Click += OnSaveIntoDifferentFile;
        saveFileMenu.Click += OnSaveTranslationFile;
        exitFileMenu.Click += (object _1, EventArgs _2) => { Close(); };

        FormClosing += BeforeClosingForm;

        splitContainer.SplitterDistance = 300;
        _xliffHandler.IsFileOpenChanged += IsOpenFileChanged;
        IsOpenFileChanged(this, EventArgs.Empty);
    }
}
