using System.Collections.ObjectModel;
using System.Windows;

namespace Automa
{
    public class DataList
    {
        public ObservableCollection<string> ComboList { get; }
        public DataList()
        {
            ComboList = new ObservableCollection<string>();
            ComboList.Add("{BACKSPACE}");
            ComboList.Add("{BREAK}");
            ComboList.Add("{CAPSLOCK");
            ComboList.Add("{DELETE}");
            ComboList.Add("{DOWN}");
            ComboList.Add("{END}");
            ComboList.Add("{ENTER}");
            ComboList.Add("{ESC}");
            ComboList.Add("{HELP}");
            ComboList.Add("{HOME}");
            ComboList.Add("{INSERT}");
            ComboList.Add("{LEFT}");
            ComboList.Add("{NUMLOCK}");
            ComboList.Add("{PGDN}");
            ComboList.Add("{PGUP}");
            //ComboList.Add("{PRTSC}");
            ComboList.Add("{RIGHT}");
            ComboList.Add("{SCROLLLOCK}");
            ComboList.Add("{TAB}");
            ComboList.Add("{UP}");
            ComboList.Add("{F1}");
            ComboList.Add("{F2}");
            ComboList.Add("{F3}");
            ComboList.Add("{F4}");
            ComboList.Add("{F5}");
            ComboList.Add("{F6}");
            ComboList.Add("{F7}");
            ComboList.Add("{F8}");
            ComboList.Add("{F9}");
            ComboList.Add("{F10}");
            ComboList.Add("{F11}");
            ComboList.Add("{F12}");
            ComboList.Add("{F13}");
            ComboList.Add("{F14}");
            ComboList.Add("{F15}");
            ComboList.Add("{F16}");
            ComboList.Add("{ADD}");
            ComboList.Add("{SUBTRACT}");
            ComboList.Add("{MULTIPLY}");
            ComboList.Add("{DIVIDE}");
            ComboList.Add("a");
            ComboList.Add("b");
            ComboList.Add("c");
            ComboList.Add("d");
            ComboList.Add("e");
            ComboList.Add("f");
            ComboList.Add("g");
            ComboList.Add("h");
            ComboList.Add("i");
            ComboList.Add("j");
            ComboList.Add("k");
            ComboList.Add("l");
            ComboList.Add("m");
            ComboList.Add("n");
            ComboList.Add("o");
            ComboList.Add("p");
            ComboList.Add("q");
            ComboList.Add("r");
            ComboList.Add("s");
            ComboList.Add("t");
            ComboList.Add("u");
            ComboList.Add("v");
            ComboList.Add("w");
            ComboList.Add("x");
            ComboList.Add("y");
            ComboList.Add("z");
        }
    }

    /// <summary>
    /// SendKeyWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SendKeyWindow : Window
    {
        public string Text { get; set; }

        public SendKeyWindow()
        {
            InitializeComponent();
            DataContext = new DataList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string str = "";
            str += (ctrl.IsChecked ?? false) ? "^" : "";
            str += (alt.IsChecked ?? false) ? "%" : "";
            str += (shift.IsChecked ?? false) ? "+" : "";
            Text = $"{str}{comboBox.Text}";
            DialogResult = true;
            Close();
        }
    }
}
