using System.Windows;

namespace Automa
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        public static string[] CommandLineArgs { get; private set; }
        private void App_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length == 0)
                return;
            CommandLineArgs = e.Args;
        }
    }
}
