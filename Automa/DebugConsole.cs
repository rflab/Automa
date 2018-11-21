namespace Automa
{
    public static class DebugConsole
    {
        public static ConsoleWindow window { get; private set; }
        private static bool window_closed { get; set; }

        private static int? _lines_length = null;
        public static int? lines_length
        {
            get { return _lines_length; }
            set
            {
                _lines_length = value;
                if (window_is_alive)
                {
                    window.lines_length = value.Value;
                }
            }
        }

        private static bool window_is_alive
            => window != null && !window_closed;

        private static void reshow()
        {
            close();
            window = new ConsoleWindow();
            if (lines_length != null)
            {
                window.lines_length = lines_length.Value;
            }
            window.Show();
            window_closed = false;
            window.Closed += (sender, e) =>
            {
                window_closed = true;
            };
        }

        public static void show()
        {
            if (window_is_alive)
            {
                window.Show();
            }
            else
            {
                reshow();
            }
        }

        public static void hide()
        {
            if (window_is_alive)
            {
                window.Hide();
            }
        }

        public static void close()
        {
            if (window_is_alive)
            {
                window.Close();
            }
        }

        public static void write(string str)
        {
            show();
            window.write(str);
        }

        public static void writeIfExistConsole(string str)
        {
            if (window_is_alive)
            {
                show();
                window.write(str);
            }
        }

        public static void writeLine(string str)
        {
            show();
            window.writeLine(str);
        }

        public static void writeLineIfExistConsole(string str)
        {
            if (window_is_alive)
            {
                show();
                window.writeLine(str);
            }
        }

        public static void clear()
        {
            if (window_is_alive)
            {
                window.clear();
            }
        }
    }
}