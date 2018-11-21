using System.Windows;

namespace Automa
{
    /// <summary>
    /// InsertTextWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SendStringWindow : Window
    {
        public string Text { get; set; }

        public SendStringWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Text = textBox.Text;
            DialogResult = true;
        }
    }
}
