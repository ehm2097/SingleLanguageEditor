namespace Avat.SingleLanguageEditor;

internal class UseTextEventArgs : EventArgs
{
    public UseTextEventArgs(string text)
        => Text = text;

    public readonly string Text;
}

internal delegate void UseTextEvent(object sender, UseTextEventArgs args);

internal class QualifiedText
{
    public QualifiedText(string category, string text)
        => (Category, Text) = (category, text);

    public readonly string Category;
    public readonly string Text;
}

internal class MultiLabel : Panel
{
    public IEnumerable<QualifiedText> TextItems
    {
        get { return GetItems(); }
        set { SetItems(value); }
    }

    public event UseTextEvent? UseText;

    protected override void OnPaint(PaintEventArgs e)
    {
        var backBrush = new SolidBrush(BackColor);
        var forePen = new Pen(ForeColor);

        e.Graphics.PageUnit = GraphicsUnit.Pixel;
        e.Graphics.FillRectangle(backBrush, ClientRectangle);

        var textWidth = ClientSize.Width - 3 * Padding - _useButtonWidth - CodeWidth;

        Size measure(string text)
        {
            var size = e.Graphics.MeasureString(text, Font);
            return new Size()
            {
                Width = (int)Math.Round(size.Width),
                Height = (int)Math.Round(size.Height)
            };
        }

        int currentTop = 0;
        foreach (var item in _items)
        {
            var info = item.Layout.PrepareDraw(textWidth, measure);

            foreach (var line in info.Lines)
            {
                var point = line.GetPoint();
                point.X += CodeWidth;
                point.Y += currentTop;
                e.Graphics.DrawString(line.Text, Font, Brushes.Black, point);

                if (line == info.Lines.First())
                {
                    point.X = Padding;
                    var codeFont = new Font(Font, FontStyle.Bold);
                    e.Graphics.DrawString(info.Code, codeFont, Brushes.Black, point);
                }
            }

            if (_isLayoutNeeded)
            {
                item.UseButton.Left = ClientSize.Width - item.UseButton.Width - Padding;
                item.UseButton.Height = info.Height - 2 * Padding;
                item.UseButton.Top = currentTop + Padding;
            }

            if (currentTop > 0)
            {
                var leftPoint = new Point() { X = 0, Y = currentTop };
                var rightPoint = new Point() { X = ClientSize.Width, Y = currentTop };
                e.Graphics.DrawLine(forePen, leftPoint, rightPoint);
            }

            currentTop += info.Height;
        }

        if (_isLayoutNeeded)
        {
            _isLayoutNeeded = false;
            Height = currentTop;
        }
    }

    protected override void OnResize(EventArgs eventargs)
    {
        _isLayoutNeeded = true;
        Invalidate();
    }

    private new const int Padding = 10;
    private const int MinHeight = 80;
    private const int CodeWidth = 80;

    private readonly int _useButtonWidth = 50;

    private bool _isLayoutNeeded = true;

    private IEnumerable<Item> _items = Array.Empty<Item>();

    private IEnumerable<QualifiedText> GetItems()
        => _items.Select(item => item.Content);

    private void SetItems(IEnumerable<QualifiedText> items)
    {
        Controls.Clear();
        _items = items
            .Select(item => new Item(item, this))
            .ToArray();
        _isLayoutNeeded = true;
        Invalidate();
    }

    private void DoUseText(string text)
        => UseText?.Invoke(this, new UseTextEventArgs(text));

    private class DrawTextBuilder
    {
        public DrawTextBuilder(int padding, int minHeight, string code)
            => (_padding, _minHeight, _code) = (padding, minHeight, code);

        public void AddLine(string text, int height)
        {
            if (_top == 0) _top += _padding;
            _lines.Add(new(text, _top, _padding, height));
            _top += height;
        }

        public DrawTextInfo Build() => new(_lines, Math.Max(_top + _padding, _minHeight), _code);

        private readonly int _padding;
        private readonly int _minHeight;
        private readonly string _code;
        private readonly List<DrawLineInfo> _lines = new();
        private int _top = 0;
    }


    private class DrawTextInfo
    {
        public DrawTextInfo(IEnumerable<DrawLineInfo> lines, int height, string code)
            => (Lines, Height, Code) = (lines, height, code);

        public readonly IEnumerable<DrawLineInfo> Lines;
        public readonly int Height;
        public readonly string Code;
    }


    private class DrawLineInfo
    {
        public DrawLineInfo(string text, int top, int left, int height)
            => (Text, Top, Left, Height) = (text, top, left, height);

        public readonly string Text;
        public readonly int Top;
        public readonly int Left;
        public readonly int Height;

        public PointF GetPoint() => new() { X = Left, Y = Top };
    }


    private class TextLayout
    {
        public TextLayout(QualifiedText qualifiedText, int padding, int minHeight)
        {
            _words = qualifiedText.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            _padding = padding;
            _minHeight = minHeight;
            _code = qualifiedText.Category;
        }

        public DrawTextInfo PrepareDraw(int width, Func<string, Size> measure)
        {
            var builder = new DrawTextBuilder(_padding, _minHeight, _code);
            var currentLine = string.Empty;
            int safetyCount = 10;
            Size tentativeSize = new();

            foreach (var word in _words)
            {
                var tentativeLine = AddToLine(currentLine, word);
                tentativeSize = measure(tentativeLine);

                if (tentativeSize.Width > width)
                {
                    if (currentLine.Length > 0) builder.AddLine(currentLine, tentativeSize.Height);
                    currentLine = word;
                    var tooLongNextWord = false;
                    while (measure(currentLine).Width > width)
                    {
                        tooLongNextWord = true;
                        currentLine = currentLine[..^1];
                    }

                    if (tooLongNextWord)
                    {
                        builder.AddLine($"{currentLine}.", tentativeSize.Height);
                        currentLine = string.Empty;
                    }

                    if (safetyCount-- <= 0) break;
                }
                else
                {
                    currentLine = tentativeLine;
                }
            }

            if (currentLine.Length > 0) builder.AddLine(currentLine, tentativeSize.Height);
            // Console.WriteLine("----------------------");
            // foreach(var line in builder.Build().Lines) { Console.WriteLine($"at {line.Left}, {line.Top}: {line.Text}"); } 
            return builder.Build();
        }

        private static string AddToLine(string line, string word)
            => line.Length == 0 ? word : $"{line} {word}";

        private readonly string[] _words;
        private readonly int _padding;
        private readonly int _minHeight;
        private readonly string _code;
    }


    private class Item
    {
        public Item(QualifiedText qualifiedText, MultiLabel parent)
        {
            Content = qualifiedText;
            Layout = new TextLayout(qualifiedText, Padding, MinHeight);

            UseButton = new Button
            {
                Width = 50,
                Text = ">>",
                Top = Padding,
                BackColor = SystemColors.ButtonFace
            };

            UseButton.Click += (sender, args) => { parent.DoUseText(qualifiedText.Text); };
            parent.Controls.Add(UseButton);
        }

        public readonly QualifiedText Content;
        public readonly TextLayout Layout;
        public readonly Button UseButton;
    }
}