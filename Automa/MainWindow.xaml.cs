using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using RoslynPad.Editor;
using RoslynPad.Roslyn;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace Automa
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private string workingFileName = "";
        private string defaultFileName = Directory.GetCurrentDirectory() + "\\無題.csx";
        private int INSERT_DELAY = 3000;
        private int AUTO_INSERT_SLEEP_TIME = 5000;
        RoslynHost host;

        public ICommand ExecCommand { get; private set; }
        public ICommand SaveAsCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand OpenCommand { get; private set; }
        public ICommand NewCommand { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            host = new RoslynHost(additionalAssemblies: new[]{
                    Assembly.Load("RoslynPad.Roslyn.Windows"),
                    Assembly.Load("RoslynPad.Editor.Windows")
                },
                references: RoslynHostReferences.Default.With(typeNamespaceImports: new[] { typeof(InputDevice) })
            );

            // 引数にスクリプトがある場合はそれを実行、なければアプリを起動する
            if (App.CommandLineArgs != null)
            {
                Title = App.CommandLineArgs[0];
                workingFileName = App.CommandLineArgs[0];
                using (StreamReader sr = new System.IO.StreamReader(App.CommandLineArgs[0], System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    roslynCodeEditor.Text = sr.ReadToEnd();
                }
                ExecFile(App.CommandLineArgs[0]);
                Close();
            }
            else
            {
                // CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, delegate { SaveCommand(); }));

                ExecCommand = new DelegateCommand(OnExec, CanExec);
                SaveAsCommand = new DelegateCommand(OnSaveAs, CanSaveAs);
                SaveCommand = new DelegateCommand(OnSave, CanSave);
                NewCommand = new DelegateCommand(OnNew, CanNew);
                OpenCommand = new DelegateCommand(OnOpen, CanOpen);
                DataContext = this;

                //// 初期作業フォルダ
                //Directory.CreateDirectory(Path.GetDirectoryName(defaultFileName));
                //Directory.SetCurrentDirectory(Path.GetDirectoryName(defaultFileName));
            }
        }

        private void OnOpen(object param)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.InitialDirectory = Path.GetDirectoryName(workingFileName);
            dialog.Filter = "csxファイル(*.csx)|*.*";
            dialog.Title = "開く";
            dialog.RestoreDirectory = true;
            dialog.CheckPathExists = true;
            dialog.AddExtension = true;
            if (dialog.ShowDialog() ?? false)
            {
                workingFileName = dialog.FileName;
                Title = Path.GetFileName(workingFileName);

                using (StreamReader sr = new System.IO.StreamReader(dialog.FileName, System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    roslynCodeEditor.Text = sr.ReadToEnd();
                }
            }
        }
        private bool CanOpen(object param)
        {
            return true;
        }

        private void OnExec(object param)
        {
            try
            {
                OnSave(param);
                Hide();
                ExecFile(workingFileName);
            }
            catch (CompilationErrorException ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "コンパイルエラー");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "エラー");
            }
            finally
            {
                Show();
            }
        }

        private bool CanExec(object param)
        {
            return true;
        }

        private void OnSave(object param)
        {
            if (File.Exists(workingFileName))
            {
                using (var sw = new StreamWriter(workingFileName, false, System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    sw.Write(roslynCodeEditor.Text);
                }
            }
            else
            {
                OnSaveAs(param);
            }
        }

        private bool CanSave(object param)
        {
            return true;
        }

        private void OnSaveAs(object param)
        {
            var sfd = new Microsoft.Win32.SaveFileDialog();
            if (File.Exists(workingFileName))
            {
                sfd.InitialDirectory = Path.GetDirectoryName(workingFileName);
            }
            else
            {
                sfd.InitialDirectory = Path.GetDirectoryName(defaultFileName);
            }
            
            sfd.FileName = "無題.csx";
            sfd.Filter = "csxファイル(*.csx)|*.*";
            sfd.FilterIndex = 2;
            sfd.Title = "名前を付けて保存";
            sfd.RestoreDirectory = true;
            sfd.OverwritePrompt = true;
            sfd.CheckPathExists = true;
            sfd.AddExtension = true;
            if (sfd.ShowDialog(this) ?? false)
            {
                if (Path.GetExtension(sfd.FileName) == "")
                    workingFileName = sfd.FileName + ".csx";
                else
                    workingFileName = sfd.FileName;

                Title = workingFileName;

                using (var sw = new StreamWriter(workingFileName, false, System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    sw.Write(roslynCodeEditor.Text);
                }
            }
        }

        private bool CanSaveAs(object param)
        {
            return true;
        }

        private void OnNew(object param)
        {
            //roslynCodeEditor.Clear();
            //Title = "新規スクリプト";
            //workingFileName = "";

            var sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.InitialDirectory = Path.GetDirectoryName(workingFileName);
            sfd.Filter = "csxファイル(*.csx)|*.*";
            sfd.FilterIndex = 2;
            sfd.Title = "新規作成";
            sfd.RestoreDirectory = true;
            sfd.OverwritePrompt = true;
            sfd.CheckPathExists = true;
            sfd.AddExtension = true;
            sfd.FileName = "無題.csx";
            if (sfd.ShowDialog(this) ?? false)
            {
                if (Path.GetExtension(sfd.FileName) == "")
                    workingFileName = sfd.FileName + ".csx";
                else
                    workingFileName = sfd.FileName;

                Title = workingFileName;
                roslynCodeEditor.Clear();

                using (var sw = new StreamWriter(workingFileName, false, System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    sw.Write(roslynCodeEditor.Text);
                }
            }
        }

        private bool CanNew(object param)
        {
            return true;
        }

        private void roslynCodeEditor_Drop(object sender, System.Windows.DragEventArgs e)
        {
            var dropFiles = e.Data.GetData(System.Windows.DataFormats.FileDrop) as string[];
            if (dropFiles == null)
                return;

            Title = dropFiles[0];
            workingFileName = dropFiles[0];

            using (StreamReader sr = new System.IO.StreamReader(dropFiles[0], System.Text.Encoding.GetEncoding("shift_jis")))
            {
                roslynCodeEditor.Text = sr.ReadToEnd();
            }
        }

        private void roslynCodeEditor_PreviewDragOver(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop, true))
            {
                e.Effects = System.Windows.DragDropEffects.Copy;
            }
            else
            {
                e.Effects = System.Windows.DragDropEffects.None;
            }
            e.Handled = true;
        }
        
        private void ExecFile(string path)
        {
            try
            {
                Directory.SetCurrentDirectory(Path.GetDirectoryName(path));

                string code = "";
                using (var sr = new StreamReader(path, System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    code = sr.ReadToEnd();
                }
                var options = ScriptOptions.Default
                    .AddImports(
                        "System",
                        "System.IO",
                        "System.Windows",
                        "System.Threading",
                        "System.Diagnostics",
                        "Automa",
                        "Automa.InputDevice")
                    .AddReferences(
                       // Assembly.GetAssembly(typeof(System.Windows.MessageBox)),
                        Assembly.GetExecutingAssembly())
                    .AddImports(host.DefaultImports)
                    .AddReferences(host.DefaultReferences);
                var script = CSharpScript.Create(code, options, typeof(MainWindow));
                script.RunAsync(this);
            }
            catch (CompilationErrorException ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "コンパイルエラー");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "エラー");
            }
        }
        
        private string ShowSaveFileDialog()
        {
            var sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.InitialDirectory = Path.GetDirectoryName(workingFileName);
            sfd.Filter = "PNGファイル(*.png)|*.*";
            sfd.FilterIndex = 2;
            sfd.Title = "画像ファイルの保存";
            sfd.RestoreDirectory = true;
            sfd.OverwritePrompt = true;
            sfd.CheckPathExists = true;
            sfd.AddExtension = true;
            sfd.FileName = DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".png";
            if (sfd.ShowDialog(this) ?? false)
            {
                if (Path.GetExtension(sfd.FileName) == "")
                    return sfd.FileName + ".png";
                else
                    return sfd.FileName;
            }
            return null;
        }

        private void InsertString(string str)
        {
            var ci = roslynCodeEditor.SelectionStart;
            roslynCodeEditor.Text = roslynCodeEditor.Text.Insert(ci, str);
            roslynCodeEditor.SelectionStart = ci + str.Length;
        }

        private void ShowGetClickPosDialog(out System.Drawing.Point pos)
        {
            var cs = new ScreenSelection();
            pos = cs.ShowGetPointDialog(ImageCapture.BitmapFromScreen(), "クリックする座標を指定してください");
        }

        /// <summary>
        /// 画像キャプチャとそこからのクリックオフセット位置を取得するダイアログ
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="offset"></param>
        private void ShowGetClickOffsetFromCapture(out string filepath, out System.Drawing.Point offset)
        {
            filepath = null;
            offset = System.Drawing.Point.Empty;
            var screen = ImageCapture.BitmapFromScreen();

            // スクリーン範囲ドラッグ
            var cs = new ScreenSelection();
            var captureRect = cs.ShowGetRectDialog(screen, "画像領域をドラッグしてください");
            if (captureRect.IsEmpty)
                return;

            // 指定範囲をファイルに保存
            filepath = ShowSaveFileDialog();
            if (filepath == null)
                return;
            Bitmap bmp = screen.Clone(captureRect, screen.PixelFormat);
            bmp.Save(filepath, ImageFormat.Bmp);

            // 相対クリック一取得
            cs = new ScreenSelection();
            var clickPoint = cs.ShowGetPointDialog(screen, "クリックする座標を指定してください");
            if (clickPoint.IsEmpty)
                return;

            // 画像の切り出し
            offset = new System.Drawing.Point();
            offset.X = clickPoint.X - captureRect.X;
            offset.Y = clickPoint.Y - captureRect.Y;
        }

        private void ShowGetDragDialog(out System.Drawing.Point[] line)
        {
            // スクリーン範囲ドラッグ
            var cs = new ScreenSelection();
            line = cs.ShowGetLineDialog(ImageCapture.BitmapFromScreen(), "ドラッグしてください");
        }

        /// <summary>
        /// 画像キャプチャとそこからのドラッグ位置を取得する
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="offset"></param>
        private void ShowGetDragOffsetFromCapture(out string filepath, out System.Drawing.Point[] line)
        {
            filepath = null;
            line = null;

            var screen = ImageCapture.BitmapFromScreen();

            // スクリーン範囲ドラッグ
            var cs = new ScreenSelection();
            var captureRect = cs.ShowGetRectDialog(screen, "画像領域をドラッグしてください");
            if (captureRect.IsEmpty)
                return;

            // 指定範囲をファイルに保存
            filepath = ShowSaveFileDialog();
            if (filepath == null)
                return;
            Bitmap bmp = screen.Clone(captureRect, screen.PixelFormat);
            bmp.Save(filepath, ImageFormat.Bmp);

            // 相対クリック一取得
            cs = new ScreenSelection();
            var absLine = cs.ShowGetLineDialog(screen, "ドラッグしてください");

            // 画像の切り出し
            line = new System.Drawing.Point[2];
            line[0].X = absLine[0].X - captureRect.X;
            line[0].Y = absLine[0].Y - captureRect.Y;
            line[1].X = absLine[1].X - captureRect.X;
            line[1].Y = absLine[1].Y - captureRect.Y;
        }

        private bool ShowImageCaptureDialog(out string filepath, out Rectangle rect)
        {
            filepath = null;
            var screen = ImageCapture.BitmapFromScreen();

            // スクリーン範囲ドラッグ
            var cs = new ScreenSelection();
            rect = cs.ShowGetRectDialog(screen,"画像領域をドラッグしてください");
            if (rect.IsEmpty)
                return false;

            // 指定範囲をファイルに保存
            filepath = ShowSaveFileDialog();
            if (filepath == null)
                return false;
            Bitmap bmp = screen.Clone(rect, screen.PixelFormat);
            bmp.Save(filepath, ImageFormat.Bmp);
            return true;
        }


        private void MenuItem_Click_SendInputストローク(object sender, RoutedEventArgs e)
        {
            var rsw = new RecordStrokeWindow();
            if (rsw.ShowDialog() ?? false)
            {
                InsertString(rsw.StrokeCode);
            }
        }


        private void MenuItem_Click_SendInputストローク画像からの相対座標(object sender, RoutedEventArgs e)
        {
            try
            {
                Hide();
                if (MenuItem_挿入操作開始を遅延する.IsChecked)
                    Thread.Sleep(INSERT_DELAY);

                if (!ShowImageCaptureDialog(out string filepath, out Rectangle rect))
                    return;

                var rsw = new RecordStrokeWindow(Path.GetFileName(filepath), rect);
                if (rsw.ShowDialog() ?? false)
                {
                    InsertString(rsw.StrokeCode);
                }
            }
            finally
            {
                Show();
            }
        }

        private void MenuItem_Click_左クリック_絶対座標(object sender, RoutedEventArgs e)
        {
            try
            {
                Hide();
                if (MenuItem_挿入操作開始を遅延する.IsChecked)
                    Thread.Sleep(INSERT_DELAY);

                ShowGetClickPosDialog(out System.Drawing.Point p);
                if (!p.IsEmpty)
                {
                    InsertString($"InputDevice.LeftClick({p.X}, {p.Y});\n");
                    if (MenuItem_1秒スリープを自動挿入する.IsChecked)
                        InsertString("Thread.Sleep({SLEEP_});\n");
                }
            }
            finally
            {
                Show();
            }
        }

        private void MenuItem_Click_左クリック_画像からの相対座標(object sender, RoutedEventArgs e)
        {
            try
            {
                Hide();
                if (MenuItem_挿入操作開始を遅延する.IsChecked)
                    Thread.Sleep(INSERT_DELAY);

                ShowGetClickOffsetFromCapture(out string filepath, out System.Drawing.Point offset);

                if (filepath != null)
                {
                    InsertString($"InputDevice.LeftClick(\"{Path.GetFileName(filepath)}\", {offset.X}, {offset.Y});\n");
                    if (MenuItem_1秒スリープを自動挿入する.IsChecked)
                        InsertString($"Thread.Sleep({AUTO_INSERT_SLEEP_TIME});\n");
                }
            }
            finally
            {
                Show();
            }
        }

        private void MenuItem_Click_左ダブルクリック_絶対座標(object sender, RoutedEventArgs e)
        {
            try
            {
                Hide();
                if (MenuItem_挿入操作開始を遅延する.IsChecked)
                    Thread.Sleep(INSERT_DELAY);

                ShowGetClickPosDialog(out System.Drawing.Point p);
                if (!p.IsEmpty)
                {
                    InsertString($"InputDevice.LeftDoubleClick({p.X}, {p.Y});\n");
                    if (MenuItem_1秒スリープを自動挿入する.IsChecked)
                        InsertString($"Thread.Sleep({AUTO_INSERT_SLEEP_TIME});\n");
                }
            }
            finally
            {
                Show();
            }

        }


        private void MenuItem_Click_左ダブルクリック_画像からの相対座標(object sender, RoutedEventArgs e)
        {
            try
            {
                Hide();
                if (MenuItem_挿入操作開始を遅延する.IsChecked)
                    Thread.Sleep(INSERT_DELAY);

                ShowGetClickOffsetFromCapture(out string filepath, out System.Drawing.Point offset);
                if (filepath != null)
                {
                    InsertString($"InputDevice.LeftDoubleClick(\"{Path.GetFileName(filepath)}\", {offset.X}, {offset.Y});\n");
                    if (MenuItem_1秒スリープを自動挿入する.IsChecked)
                        InsertString($"Thread.Sleep({AUTO_INSERT_SLEEP_TIME});\n");
                }
            }
            finally
            {
                Show();
            }

        }

        private void MenuItem_Click_左ドラッグ_絶対座標(object sender, RoutedEventArgs e)
        {
            try
            {
                Hide();
                if (MenuItem_挿入操作開始を遅延する.IsChecked)
                    Thread.Sleep(INSERT_DELAY);

                ShowGetDragDialog(out System.Drawing.Point[] p);
                if (p!=null)
                {
                    InsertString($"InputDevice.LeftDrag({p[0].X}, {p[0].Y}, {p[1].X}, {p[1].Y});\n");
                    if (MenuItem_1秒スリープを自動挿入する.IsChecked)
                        InsertString("Thread.Sleep({SLEEP_});\n");
                }
            }
            finally
            {
                Show();
            }
        }

        private void MenuItem_Click_左ドラッグ_画像からの相対座標(object sender, RoutedEventArgs e)
        {
            try
            {
                Hide();
                if (MenuItem_挿入操作開始を遅延する.IsChecked)
                    Thread.Sleep(INSERT_DELAY);

                ShowGetDragOffsetFromCapture(out string filepath, out System.Drawing.Point[] p);
                if (filepath != null)
                {
                    InsertString($"InputDevice.LeftDrag(\"{Path.GetFileName(filepath)}\", {p[0].X}, {p[0].Y}, {p[1].X}, {p[1].Y});\n");
                    if (MenuItem_1秒スリープを自動挿入する.IsChecked)
                        InsertString("Thread.Sleep({SLEEP_});\n");
                }
            }
            finally
            {
                Show();
            }
        }

        private void MenuItem_Click_右クリック_絶対座標(object sender, RoutedEventArgs e)
        {
            try
            {
                Hide();
                if (MenuItem_挿入操作開始を遅延する.IsChecked)
                    Thread.Sleep(INSERT_DELAY);

                ShowGetClickPosDialog(out System.Drawing.Point p);
                if (!p.IsEmpty)
                {
                    InsertString($"InputDevice.RightClick({p.X}, {p.Y});\n");
                    if (MenuItem_1秒スリープを自動挿入する.IsChecked)
                        InsertString($"Thread.Sleep({AUTO_INSERT_SLEEP_TIME});\n");
                }
            }
            finally
            {
                Show();
            }
        }

        private void MenuItem_Click_右クリック_画像からの相対座標(object sender, RoutedEventArgs e)
        {
            try
            {
                Hide();
                if (MenuItem_挿入操作開始を遅延する.IsChecked)
                    Thread.Sleep(INSERT_DELAY);

                ShowGetClickOffsetFromCapture(out string filepath, out System.Drawing.Point offset);

                if (filepath != null)
                {
                    InsertString($"InputDevice.RightClick(\"{Path.GetFileName(filepath)}\", {offset.X}, {offset.Y});\n");
                    if (MenuItem_1秒スリープを自動挿入する.IsChecked)
                        InsertString($"Thread.Sleep({AUTO_INSERT_SLEEP_TIME});\n");
                }
            }
            finally
            {
                Show();
            }
        }

        private void MenuItem_Click_中央クリック_絶対座標(object sender, RoutedEventArgs e)
        {
            try
            {
                Hide();
                if (MenuItem_挿入操作開始を遅延する.IsChecked)
                    Thread.Sleep(INSERT_DELAY);

                ShowGetClickPosDialog(out System.Drawing.Point p);
                if (!p.IsEmpty)
                {
                    InsertString($"InputDevice.CenterClick({p.X}, {p.Y});\n");
                    if (MenuItem_1秒スリープを自動挿入する.IsChecked)
                        InsertString($"Thread.Sleep({AUTO_INSERT_SLEEP_TIME});\n");
                }
            }
            finally
            {
                Show();
            }
        }

        private void MenuItem_Click_中央クリック_画像からの相対座標(object sender, RoutedEventArgs e)
        {
            try
            {
                Hide();
                if (MenuItem_挿入操作開始を遅延する.IsChecked)
                    Thread.Sleep(INSERT_DELAY);

                ShowGetClickOffsetFromCapture(out string filepath, out System.Drawing.Point offset);

                if (filepath != null)
                {
                    InsertString($"InputDevice.CenterClick(\"{Path.GetFileName(filepath)}\", {offset.X}, {offset.Y});\n");
                    if (MenuItem_1秒スリープを自動挿入する.IsChecked)
                        InsertString($"Thread.Sleep({AUTO_INSERT_SLEEP_TIME});\n");
                }
            }
            finally
            {
                Show();
            }
        }
        private void MenuItem_Click_キー送信(object sender, RoutedEventArgs e)
        {
            var skw = new SendKeyWindow();
            if (skw.ShowDialog() ?? false)
            {
                InsertString($"InputDevice.SendString(\"{skw.Text}\");\n");
            }
        }

        private void MenuItem_Click_文字入力(object sender, RoutedEventArgs e)
        {
            var ssw = new SendStringWindow();
            if (ssw.ShowDialog() ?? false)
            {
                InsertString($"InputDevice.SendString(\"{ssw.Text}\");\n");
            }
        }

        private void MenuItem_Click_1秒スリープ(object sender, RoutedEventArgs e)
        {
            InsertString("Thread.Sleep(5000);\n");
        }


        private void MenuItem_Click_メッセージボックス(object sender, RoutedEventArgs e)
        {
            InsertString("MessageBox.Show(\"メッセージ\");\n");
        }


        private void MenuItem_Click_作業ディレクトリを開く(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(Path.GetDirectoryName(workingFileName));
            }
            catch (Exception)
            {
            }
        }
        
        private void MenuItem_Click_挿入操作開始を遅延する(object sender, RoutedEventArgs e)
        {
            MenuItem_挿入操作開始を遅延する.IsChecked = !MenuItem_挿入操作開始を遅延する.IsChecked;
        }
        
        private void MenuItem_Click_終了(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MenuItem_Click_1秒スリープを自動挿入する(object sender, RoutedEventArgs e)
        {
            MenuItem_1秒スリープを自動挿入する.IsChecked = !MenuItem_1秒スリープを自動挿入する.IsChecked;
        }

        private void MenuItem_Click_プロセスの実行(object sender, RoutedEventArgs e)
        {
            InsertString("System.Diagnostics.Process.Start(\"iexplore.exe\");\n");
        }

        private void MenuItem_Click_乱数(object sender, RoutedEventArgs e)
        {
            InsertString(
                "var rand = new Random()\n" +
                "int r = rand.Next(0, 10);\n");
        }

        private void MenuItem_Click_乱数小数(object sender, RoutedEventArgs e)
        {
            InsertString(
                "var rand = new Random()\n" +
                "double r = rand.NextDouble();\n");
        }

        private void MenuItem_Click_部分画像検索(object sender, RoutedEventArgs e)
        {
            ShowImageCaptureDialog(out string path, out Rectangle dummy);
            if (path == null)
                return;
            Uri curDir = new Uri(Directory.GetCurrentDirectory() + "\\");
            Uri imageDir = new Uri(path);
            InsertString($"var p = InputDevice.SerchImage(@\"{curDir.MakeRelativeUri(imageDir).ToString().Replace("/", @"\")}\");\n");
        }

        private void RoslynCodeEditor_Loaded(object sender, RoutedEventArgs e)
        {
            roslynCodeEditor.Initialize(host, new ClassificationHighlightColors(), Directory.GetCurrentDirectory(), String.Empty);
        }

        private void MenuItem_Click_実行用バッチファイルを作成(object sender, RoutedEventArgs e)
        {
            OnSave(null);

            string desktop = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            var sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.InitialDirectory = desktop;
            sfd.Filter = "VBSファイル(*.vbs)|*.*";
            sfd.FilterIndex = 2;
            sfd.Title = "名前を付けて保存";
            sfd.RestoreDirectory = true;
            sfd.OverwritePrompt = true;
            sfd.CheckPathExists = true;
            sfd.AddExtension = true;
            sfd.FileName = Path.GetFileNameWithoutExtension(workingFileName)+".vbs";
            if (sfd.ShowDialog(this) ?? false)
            {
                if (Path.GetExtension(sfd.FileName) != ".vbs")
                    sfd.FileName = sfd.FileName + ".vbs";

                using (var sw = new StreamWriter(sfd.FileName, false, System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    sw.WriteLine($"Set objWShell = CreateObject(\"Wscript.Shell\")");
                    sw.WriteLine($"objWShell.run \"{Assembly.GetExecutingAssembly().Location} {workingFileName}\", vbHide");
                }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            DebugConsole.writeLine("DebugConsole.writeLine(\"\")で書き込んでください");
        }

    }
}
