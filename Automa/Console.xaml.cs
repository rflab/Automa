using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Automa
{
    public partial class ConsoleWindow : Window
    {
        private int line_length { get; set; }

        private int _lines_length = 999;
        public int lines_length
        {
            get { return this._lines_length; }
            set
            {
                if (value < 0)
                {
                    throw new Exception("lines length must be larger than or equal 0");
                }

                this._lines_length = value == 0 ? int.MaxValue : value;
                this.updateLinesLength();
            }
        }

        public ConsoleWindow()
        {
            InitializeComponent();
            this.updateLineLength();
        }

        public void write(string str)
        {
            var tb_lines = this.lines.Children;
            var text = tb_lines.Cast<TextBlock>().Last().Text;
            str = str.Replace("\r\n", "\n");
            str = str.Replace("\r", "\n");

            var text_lines = (text + str).Split('\n').SelectMany(e =>
            {
                var lines = new List<string>();
                var ll = this.line_length;

                var s = e;
                while (s.Length > ll)
                {
                    lines.Add(s.Substring(0, ll));
                    s = s.Substring(ll);
                }

                lines.Add(s);

                return lines;
            });

            tb_lines.RemoveAt(tb_lines.Count - 1);
            foreach (var line in text_lines)
            {
                tb_lines.Add(new TextBlock() { Text = line });
            }

            this.updateLinesLength();
        }

        public void writeLine(string str)
        {
            this.write(str + "\n");
        }

        public void clear()
        {
            this.lines.Children.Clear();
            this.lines.Children.Add(new TextBlock());
        }

        public void updateLineLength()
        {
            var formatted_text = new FormattedText(
                "x", CultureInfo.CurrentUICulture, System.Windows.FlowDirection.LeftToRight,
                new Typeface(this.FontFamily, this.FontStyle, this.FontWeight, this.FontStretch),
                this.FontSize, Brushes.White);

            this.line_length = (int)(this.lines.ActualWidth / formatted_text.Width);
        }

        public void updateLinesLength()
        {
            while (this.lines.Children.Count > this.lines_length)
            {
                this.lines.Children.RemoveAt(0);
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.updateLineLength();
        }
    }
}
