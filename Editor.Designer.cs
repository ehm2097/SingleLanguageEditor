namespace Avat.SingleLanguageEditor;

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
