using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Automa
{
    /// <summary>
    /// RecordStrokeWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class RecordStrokeWindow : Window
    {
        private int offsetX_ = 0;
        private int offsetY_ = 0;
        private uint prevTime_ = 0;
        private List<string> StrokeCodeList = new List<string>();

        public string StrokeCode { get; set; }

        public RecordStrokeWindow(string path=null, System.Drawing.Rectangle? rect=null)
        {
            InitializeComponent();
            if (path != null)
            {
                StrokeCodeList.Add(
                    $"{{\n" +
                    $"    var p = InputDevice.SerchImage(@\"{path}\");\n" +
                    $"    var stroke = new InputDevice.KeyStroke\n" +
                    $"    {{\n" +
                    $"        OffsetX = p.X,\n" +
                    $"        OffsetY = p.Y\n" +
                    $"    }};\n");
                offsetX_ = rect.Value.X;
                offsetY_ = rect.Value.Y;
            }
            else
            {
                StrokeCodeList.Add(
                    $"{{\n" +
                    $"    var stroke = new InputDevice.KeyStroke();\n");
            }

            MouseHook.AddEvent(MouseHookProc);
            MouseHook.Start();
            KeyboardHook.AddEvent(KeyboardHookProc);
            KeyboardHook.Start();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //最後のクリック削除
            StrokeCodeList.RemoveRange(StrokeCodeList.Count - 2, 2); 
            // 最後のクリックまでの移動削除
            int ix = StrokeCodeList.FindLastIndex(x => x.IndexOf("stroke.MouseModeTo")==-1);
            StrokeCodeList.RemoveRange(ix+1, StrokeCodeList.Count - (ix+1));
            StrokeCodeList.Add(
                "    stroke.Send();\n" +
                "}");
            
            StrokeCode = string.Join("", StrokeCodeList);

            MouseHook.Stop();
            KeyboardHook.Stop();
            DialogResult = true;
            Close();
        }

        void MouseHookProc(ref MouseHook.StateMouse s)
        {
            string line = "";
            if (prevTime_ == 0)
                prevTime_ = s.Time;

            // { s.Time}使い方わからぬ。。
            // -> TimeはDelayのためのものではない、
            // 処理の遅延があってもダブルクリックの判定をしたいとかそういうの(https://blogs.msdn.microsoft.com/oldnewthing/20121101-00/?p=6193)
            switch (s.Stroke)
            {
                case MouseHook.Stroke.MOVE:
                    line = $"    stroke.MouseModeTo({s.X - offsetX_}, {s.Y - offsetY_}, {s.Time - prevTime_});\n";
                    break;
                case MouseHook.Stroke.LEFT_DOWN:
                    line = $"    stroke.LeftDown({s.Time - prevTime_});\n";
                    break;
                case MouseHook.Stroke.LEFT_UP:
                    line = $"    stroke.LeftUp({s.Time - prevTime_});\n";
                    break;
                case MouseHook.Stroke.MIDDLE_DOWN:
                    line = $"    stroke.MiddleDown({s.Time - prevTime_});\n";
                    break;
                case MouseHook.Stroke.MIDDLE_UP:
                    line = $"    stroke.MiddleUp({s.Time - prevTime_});\n";
                    break;
                case MouseHook.Stroke.RIGHT_DOWN:
                    line = $"    stroke.RightDown({s.Time - prevTime_});\n";
                    break;
                case MouseHook.Stroke.RIGHT_UP:
                    line = $"    stroke.RightUp({s.Time - prevTime_});\n";
                    break;
                case MouseHook.Stroke.WHEEL_DOWN:
                    line = $"    stroke.MiddleDown({s.Time - prevTime_});\n";
                    break;
                case MouseHook.Stroke.WHEEL_UP:
                    line = $"    stroke.MiddleUp({s.Time - prevTime_});\n";
                    break;
            }

            label.Content = line;
            StrokeCodeList.Add(line);
        }


        void KeyboardHookProc(ref KeyboardHook.StateKeyboard s)
        {
            string line = "";

            switch (s.Stroke)
            {
                case KeyboardHook.Stroke.KEY_DOWN:
                    line = $"    stroke.KeyDown(InputDevice.VirtualKeys.{Enum.GetName(typeof(InputDevice.VirtualKeys), s.Key)});\n";
                    break;
                case KeyboardHook.Stroke.KEY_UP:
                    line = $"    stroke.KeyUp(InputDevice.VirtualKeys.{Enum.GetName(typeof(InputDevice.VirtualKeys), s.Key)});\n";
                    break;
                case KeyboardHook.Stroke.SYSKEY_DOWN:
                    line = $"    stroke.KeyDown(InputDevice.VirtualKeys.{Enum.GetName(typeof(InputDevice.VirtualKeys), s.Key)});\n";
                    break;
                case KeyboardHook.Stroke.SYSKEY_UP:
                    line = $"    stroke.KeyUp(InputDevice.VirtualKeys.{Enum.GetName(typeof(InputDevice.VirtualKeys), s.Key)});\n";
                    break;
            }

            label.Content = line;
            StrokeCodeList.Add(line);
        }
    }
}
