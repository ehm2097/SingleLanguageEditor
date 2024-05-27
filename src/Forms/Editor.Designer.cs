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

internal partial class Editor : Form
{
    private System.ComponentModel.IContainer components = null;
    private TextBox _targetText;
    private MultiLabel _sourceLabels;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) { components.Dispose(); }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();

        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(800, 510);
        Text = "Translation Editor";

        var editorPanel = new Panel();
        editorPanel.Dock = DockStyle.Fill;
        editorPanel.Padding = new Padding(20);
        Controls.Add(editorPanel);

        var actionPanel = new Panel();
        actionPanel.Dock = DockStyle.Bottom;
        actionPanel.Padding = new Padding(20);
        actionPanel.Height = 80;
        Controls.Add(actionPanel);

        var targetText = new TextBox();
        targetText.Multiline = true;
        targetText.Height = 110;
        targetText.Dock = DockStyle.Fill;
        targetText.ReadOnly = false;
        targetText.TabStop = true;
        targetText.KeyDown += HandleKeyDown;
        editorPanel.Controls.Add(targetText);
        _targetText = targetText;

        var multiLabel = new MultiLabel();
        multiLabel.Dock = DockStyle.Top;
        multiLabel.BorderStyle = BorderStyle.FixedSingle;
        multiLabel.UseText += SourceLabelsUseText;
        editorPanel.Controls.Add(multiLabel);
        _sourceLabels = multiLabel;

        var skipButton = new Button();
        skipButton.Text = "Skip";
        skipButton.Width = 80;        
        skipButton.Dock = DockStyle.Left;
        skipButton.Click += OnClickSkipButton;
        actionPanel.Controls.Add(skipButton);

        var okButton = new Button();
        okButton.Text = "OK";
        okButton.Width = 80;        
        okButton.Dock = DockStyle.Left;
        okButton.Click += OnClickOkButton;
        actionPanel.Controls.Add(okButton);

        var closeButton = new Button();
        closeButton.Text = "Close";
        closeButton.Width = 80;
        closeButton.Dock = DockStyle.Right;
        closeButton.Click += (object _1, EventArgs _2) => { Close(); };
        actionPanel.Controls.Add(closeButton);
        
        AcceptButton = okButton;
    }
}
