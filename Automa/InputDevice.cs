using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Automa
{
    public class InputDevice
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public int dwExtraInfo;
        };

        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public short wVk;
            public short wScan;
            public int dwFlags;
            public int time;
            public int dwExtraInfo;
        };

        [StructLayout(LayoutKind.Sequential)]
        private struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        };

        [StructLayout(LayoutKind.Explicit)]
        private struct INPUT
        {
            [FieldOffset(0)] public int type;
            [FieldOffset(4)] public MOUSEINPUT m;
            [FieldOffset(4)] public KEYBDINPUT k;
            [FieldOffset(4)] public HARDWAREINPUT h;
        };

        [DllImport("user32.dll")]
        private extern static void SendInput(int nInputs, ref INPUT pInputs, int cbsize);

        [DllImport("user32.dll", EntryPoint = "MapVirtualKeyA")]
        private extern static int MapVirtualKey(int wCode, int wMapType);

        private const int INPUT_MOUSE = 0;                  // マウスイベント
        private const int INPUT_KEYBOARD = 1;               // キーボードイベント
        private const int INPUT_HARDWARE = 2;               // ハードウェアイベント
        private const int MOUSEEVENTF_MOVE = 0x1;           // マウスを移動する
        private const int MOUSEEVENTF_ABSOLUTE = 0x8000;    // 絶対座標指定
        private const int MOUSEEVENTF_LEFTDOWN = 0x2;       // 左　ボタンを押す
        private const int MOUSEEVENTF_LEFTUP = 0x4;         // 左　ボタンを離す
        private const int MOUSEEVENTF_RIGHTDOWN = 0x8;      // 右　ボタンを押す
        private const int MOUSEEVENTF_RIGHTUP = 0x10;       // 右　ボタンを離す
        private const int MOUSEEVENTF_MIDDLEDOWN = 0x20;    // 中央ボタンを押す
        private const int MOUSEEVENTF_MIDDLEUP = 0x40;      // 中央ボタンを離す
        private const int MOUSEEVENTF_WHEEL = 0x800;        // ホイールを回転する
        private const int WHEEL_DELTA = 120;                // ホイール回転値
        private const int KEYEVENTF_KEYDOWN = 0x0;          // キーを押す
        private const int KEYEVENTF_KEYUP = 0x2;            // キーを離す
        private const int KEYEVENTF_EXTENDEDKEY = 0x1;      // 拡張コード

        public enum VirtualKeys : ushort
        {

            LeftButton = 0x01,
            RightButton = 0x02,
            Cancel = 0x03,
            MiddleButton = 0x04,
            ExtraButton1 = 0x05,
            ExtraButton2 = 0x06,
            Back = 0x08,
            Tab = 0x09,
            Clear = 0x0C,
            Return = 0x0D,
            Shift = 0x10,
            Control = 0x11,
            Menu = 0x12,
            Pause = 0x13,
            CapsLock = 0x14,
            Kana = 0x15,
            Hangeul = 0x15,
            Hangul = 0x15,
            Junja = 0x17,
            Final = 0x18,
            Hanja = 0x19,
            Kanji = 0x19,
            Escape = 0x1B,
            Convert = 0x1C,
            NonConvert = 0x1D,
            Accept = 0x1E,
            ModeChange = 0x1F,
            Space = 0x20,
            Prior = 0x21,
            Next = 0x22,
            End = 0x23,
            Home = 0x24,
            Left = 0x25,
            Up = 0x26,
            Right = 0x27,
            Down = 0x28,
            Select = 0x29,
            Print = 0x2A,
            Execute = 0x2B,
            Snapshot = 0x2C,
            Insert = 0x2D,
            Delete = 0x2E,
            Help = 0x2F,
            N0 = 0x30,
            N1 = 0x31,
            N2 = 0x32,
            N3 = 0x33,
            N4 = 0x34,
            N5 = 0x35,
            N6 = 0x36,
            N7 = 0x37,
            N8 = 0x38,
            N9 = 0x39,
            A = 0x41,
            B = 0x42,
            C = 0x43,
            D = 0x44,
            E = 0x45,
            F = 0x46,
            G = 0x47,
            H = 0x48,
            I = 0x49,
            J = 0x4A,
            K = 0x4B,
            L = 0x4C,
            M = 0x4D,
            N = 0x4E,
            O = 0x4F,
            P = 0x50,
            Q = 0x51,
            R = 0x52,
            S = 0x53,
            T = 0x54,
            U = 0x55,
            V = 0x56,
            W = 0x57,
            X = 0x58,
            Y = 0x59,
            Z = 0x5A,
            LeftWindows = 0x5B,
            RightWindows = 0x5C,
            Application = 0x5D,
            Sleep = 0x5F,
            Numpad0 = 0x60,
            Numpad1 = 0x61,
            Numpad2 = 0x62,
            Numpad3 = 0x63,
            Numpad4 = 0x64,
            Numpad5 = 0x65,
            Numpad6 = 0x66,
            Numpad7 = 0x67,
            Numpad8 = 0x68,
            Numpad9 = 0x69,
            Multiply = 0x6A,
            Add = 0x6B,
            Separator = 0x6C,
            Subtract = 0x6D,
            Decimal = 0x6E,
            Divide = 0x6F,
            F1 = 0x70,
            F2 = 0x71,
            F3 = 0x72,
            F4 = 0x73,
            F5 = 0x74,
            F6 = 0x75,
            F7 = 0x76,
            F8 = 0x77,
            F9 = 0x78,
            F10 = 0x79,
            F11 = 0x7A,
            F12 = 0x7B,
            F13 = 0x7C,
            F14 = 0x7D,
            F15 = 0x7E,
            F16 = 0x7F,
            F17 = 0x80,
            F18 = 0x81,
            F19 = 0x82,
            F20 = 0x83,
            F21 = 0x84,
            F22 = 0x85,
            F23 = 0x86,
            F24 = 0x87,
            NumLock = 0x90,
            ScrollLock = 0x91,
            NEC_Equal = 0x92,
            Fujitsu_Jisho = 0x92,
            Fujitsu_Masshou = 0x93,
            Fujitsu_Touroku = 0x94,
            Fujitsu_Loya = 0x95,
            Fujitsu_Roya = 0x96,
            LeftShift = 0xA0,
            RightShift = 0xA1,
            LeftControl = 0xA2,
            RightControl = 0xA3,
            LeftMenu = 0xA4,
            RightMenu = 0xA5,
            BrowserBack = 0xA6,
            BrowserForward = 0xA7,
            BrowserRefresh = 0xA8,
            BrowserStop = 0xA9,
            BrowserSearch = 0xAA,
            BrowserFavorites = 0xAB,
            BrowserHome = 0xAC,
            VolumeMute = 0xAD,
            VolumeDown = 0xAE,
            VolumeUp = 0xAF,
            MediaNextTrack = 0xB0,
            MediaPrevTrack = 0xB1,
            MediaStop = 0xB2,
            MediaPlayPause = 0xB3,
            LaunchMail = 0xB4,
            LaunchMediaSelect = 0xB5,
            LaunchApplication1 = 0xB6,
            LaunchApplication2 = 0xB7,
            OEM1 = 0xBA,
            OEMPlus = 0xBB,
            OEMComma = 0xBC,
            OEMMinus = 0xBD,
            OEMPeriod = 0xBE,
            OEM2 = 0xBF,
            OEM3 = 0xC0,
            OEM4 = 0xDB,
            OEM5 = 0xDC,
            OEM6 = 0xDD,
            OEM7 = 0xDE,
            OEM8 = 0xDF,
            OEMAX = 0xE1,
            OEM102 = 0xE2,
            ICOHelp = 0xE3,
            ICO00 = 0xE4,
            ProcessKey = 0xE5,
            ICOClear = 0xE6,
            Packet = 0xE7,
            OEMReset = 0xE9,
            OEMJump = 0xEA,
            OEMPA1 = 0xEB,
            OEMPA2 = 0xEC,
            OEMPA3 = 0xED,
            OEMWSCtrl = 0xEE,
            OEMCUSel = 0xEF,
            OEMATTN = 0xF0,
            OEMFinish = 0xF1,
            OEMCopy = 0xF2,
            OEMAuto = 0xF3,
            OEMENLW = 0xF4,
            OEMBackTab = 0xF5,
            ATTN = 0xF6,
            CRSel = 0xF7,
            EXSel = 0xF8,
            EREOF = 0xF9,
            Play = 0xFA,
            Zoom = 0xFB,
            Noname = 0xFC,
            PA1 = 0xFD,
            OEMClear = 0xFE
        }

        public class KeyStroke
        {
            public int OffsetX { get; set; }
            public int OffsetY { get; set; }
            private List<INPUT> stroke = new List<INPUT>();

            public KeyStroke()
            {
                OffsetX = 0;
                OffsetY = 0;
            }

            private void AddKeyInput(VirtualKeys key, bool up)
            {
                INPUT i = new INPUT();
                i.type = INPUT_KEYBOARD;
                i.k.wVk = (short)key;
                i.k.wScan = (short)MapVirtualKey((int)key, 0);
                i.k.dwFlags = up ? KEYEVENTF_KEYUP : KEYEVENTF_KEYDOWN;
                i.k.dwExtraInfo = 0;
                stroke.Add(i);
            }
            public void KeyDown(VirtualKeys key) { AddKeyInput(key, false); }
            public void KeyUp(VirtualKeys key) { AddKeyInput(key, true); }
            public void KeyPush(VirtualKeys key) { KeyDown(key); KeyUp(key); }
            public void KeyPushWithShift(VirtualKeys key)
            {
                KeyDown(VirtualKeys.Shift);
                KeyPush(key);
                KeyUp(VirtualKeys.Shift);
            }

            private void AddMouseInput(
                int dx, int dy, int mouseData, int dwFlags, int time, int dwExtraInfo)
            {
                INPUT i = new INPUT();
                i.type = INPUT_MOUSE;
                i.m.dx = (int)(dx * (65535 / System.Windows.SystemParameters.PrimaryScreenWidth));
                i.m.dy = (int)(dy * (65535 / System.Windows.SystemParameters.PrimaryScreenHeight));
                i.m.dwFlags = dwFlags; // MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE;
                i.m.time = time;
                i.m.dwExtraInfo = dwExtraInfo;
                stroke.Add(i);
            }
            public void LeftDown(int time) { AddMouseInput(0, 0, 0, MOUSEEVENTF_LEFTDOWN, time, 0); }
            public void LeftUp(int time) { AddMouseInput(0, 0, 0, MOUSEEVENTF_LEFTUP, time, 0); }
            public void LeftClick(int time) { LeftDown(time); LeftUp(10); }
            public void LeftDoubleClick(int time) { LeftClick(time); LeftClick(10); }
            public void RightDown(int time) { AddMouseInput(0, 0, 0, MOUSEEVENTF_RIGHTDOWN, time, 0); }
            public void RightUp(int time) { AddMouseInput(0, 0, 0, MOUSEEVENTF_RIGHTUP, time, 0); }
            public void RightClick(int time) { RightDown(time); RightUp(10); }
            public void MiddleDown(int time) { AddMouseInput(0, 0, 0, MOUSEEVENTF_MIDDLEDOWN, time, 0); }
            public void MiddleUp(int time) { AddMouseInput(0, 0, 0, MOUSEEVENTF_MIDDLEUP, time, 0); }
            public void MiddleClick(int time) { MiddleDown(time); MiddleUp(10); }
            public void MouseModeTo(int x, int y, int time) { AddMouseInput(x, y, 0, MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE, time, 0); }
            public void MouseMove(int dx, int dy, int time) { AddMouseInput(dx, dy, 0, MOUSEEVENTF_MOVE, time, 0); }

            public void Send()
            {
                var input = stroke.ToArray();
                if (input.Length == 0)
                    return;

                // マウスは早すぎるとドラッグを取りこぼすので手動でsleep入れる
                // timeの値を正しくつかえばSendInputで解釈してくれるのかもだが、うまくできない...orz
                // →そうではないが、とりあえずtimeを使ってdelayする
                int prevTime = 0;
                if (input[0].type == INPUT_MOUSE)
                    prevTime = input[0].m.time;
                else if (input[0].type == INPUT_KEYBOARD)
                    prevTime = input[0].k.time;
                else if (input[0].type == INPUT_HARDWARE)
                    prevTime = 0;
                for (int i = 0; i < input.Length; i++)
                {
                    if (input[i].type == INPUT_MOUSE)
                    {
                        Thread.Sleep(input[i].m.time - prevTime);
                        prevTime = input[i].m.time;
                        input[i].m.time = 0;

                        input[i].m.dx += (int)(OffsetX * (65535 / System.Windows.SystemParameters.PrimaryScreenWidth));
                        input[i].m.dy += (int)(OffsetY * (65535 / System.Windows.SystemParameters.PrimaryScreenHeight));
                    }
                    else if (input[i].type == INPUT_KEYBOARD)
                    {
                        // Thread.Sleep(input[i].k.time - prevTime);
                        prevTime = input[i].k.time;
                        // input[i].m.time = 0;
                    }
                    else if (input[i].type == INPUT_HARDWARE)
                    {
                        Thread.Sleep(10);
                    }

                    SendInput(1, ref input[i], Marshal.SizeOf(input[i]));
                }
            }
        }

        static public System.Drawing.Point SerchImage(string img_path, int retry_num = 10,
            int retry_sleep = 1000, double err_threshold = 0.05)
        {
            for (int i = 0; i < retry_num; i++)
            {
                Bitmap screen = ImageCapture.BitmapFromScreen();
                Bitmap bmp = (Bitmap)Bitmap.FromFile(img_path);
                var pos = ImageSearch.Search(screen, bmp, err_threshold);
                if (!pos.IsEmpty)
                    return pos;

                Console.WriteLine($"{Path.GetFileName(img_path)} not found.");
                Thread.Sleep(retry_sleep);
            }

            MessageBox.Show($"画像が見つかりません {img_path}\n");
            return System.Drawing.Point.Empty;
        }

        static public void LeftClick(int x, int y, int delay_ms = 1000)
        {
            Thread.Sleep(delay_ms);
            var stroke = new KeyStroke();
            stroke.MouseModeTo(x, y, 0);
            stroke.LeftClick(0);
            stroke.Send();
        }

        static public void LeftClick(string img_path, int x, int y, int delay_ms = 1000, 
            int retry_num = 10, int retry_sleep = 1000, double err_threshold = 0.05)
        {
            Thread.Sleep(delay_ms);
            var p = SerchImage(img_path, retry_num, retry_sleep, err_threshold);
            if (p.IsEmpty)
            {
                // if (System.Windows.MessageBox.Show("画像が一致しません。続行しますか？", "エラー", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                //     throw new Exception("画像が一致しませんでした。OKで続行します。");
                System.Windows.MessageBox.Show($"画像({Path.GetFileName(img_path)})との一致が見つかりません。" + Environment.NewLine + "OKで続行します。");
            }
            else
            {
                LeftClick(p.X + x, p.Y + y, 0);
            }
        }

        static public void LeftDoubleClick(int x, int y, int delay_ms = 1000)
        {
            Thread.Sleep(delay_ms);
            var stroke = new KeyStroke();
            stroke.MouseModeTo(x, y, 0);
            stroke.LeftDoubleClick(0);
            stroke.Send();
        }

        static public void LeftDoubleClick(string img_path, int x, int y, int delay_ms = 1000,
            int retry_num = 10, int retry_sleep = 1000, double err_threshold = 0.05)
        {
            Thread.Sleep(delay_ms);
            var p = SerchImage(img_path, retry_num, retry_sleep, err_threshold);
            if (p.IsEmpty)
            {
                // if (System.Windows.MessageBox.Show("画像が一致しません。続行しますか？", "エラー", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                //     throw new Exception("画像が一致しませんでした。OKで続行します。");
                System.Windows.MessageBox.Show($"画像({Path.GetFileName(img_path)})との一致が見つかりません。" + Environment.NewLine + "OKで続行します。");
            }
            else
            {
                LeftDoubleClick(p.X + x, p.Y + y, 0);
            }
        }


        static public void LeftDrag(int x1, int y1, int x2, int y2, int delay_ms = 1000)
        {
            Thread.Sleep(delay_ms);
            var stroke = new KeyStroke();
            stroke.MouseModeTo(x1, y1, 0);
            stroke.LeftDown(200);
            stroke.MouseModeTo(x2, y2, 400);
            stroke.LeftUp(600);
            stroke.Send();
        }

        static public void LeftDrag(string img_path, int x1, int y1, int x2, int y2, int delay_ms = 1000,
            int retry_num = 10, int retry_sleep = 1000, double err_threshold = 0.05)
        {
            Thread.Sleep(delay_ms);
            var p = SerchImage(img_path, retry_num, retry_sleep, err_threshold);
            if (p.IsEmpty)
            {
                // if (System.Windows.MessageBox.Show("画像が一致しません。続行しますか？", "エラー", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                //     throw new Exception("画像が一致しませんでした。OKで続行します。");
                System.Windows.MessageBox.Show($"画像({Path.GetFileName(img_path)})との一致が見つかりません。" + Environment.NewLine + "OKで続行します。");
            }
            else
            {
                LeftDrag(p.X + x1, p.Y + y1, p.X + x2, p.Y + y2, 0);
            }
        }

        static public void RightClick(int x, int y, int delay_ms = 1000)
        {
            //Thread.Sleep(delay_ms);
            var stroke = new KeyStroke();
            stroke.MouseModeTo(x, y, 0);
            stroke.RightClick(delay_ms);
            stroke.Send();
        }

        static public void RightClick(string img_path, int x, int y, int delay_ms = 1000,
            int retry_num = 10, int retry_sleep = 1000, double err_threshold = 0.05)
        {
            Thread.Sleep(delay_ms);
            var p = SerchImage(img_path, retry_num, retry_sleep, err_threshold);

            if (p.IsEmpty)
            {
                // if (System.Windows.MessageBox.Show("画像が一致しません。続行しますか？", "エラー", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                //     throw new Exception("画像が一致しませんでした。OKで続行します。");
                System.Windows.MessageBox.Show($"画像({Path.GetFileName(img_path)})との一致が見つかりません。" + Environment.NewLine + "OKで続行します。");
            }
            else
            {
                RightClick(p.X + x, p.Y + y, 0);
            }
        }

        static public void CenterClick(int x, int y, int delay_ms = 1000)
        {
            //Thread.Sleep(delay_ms);
            var stroke = new KeyStroke();
            stroke.MouseModeTo(x, y, 0);
            stroke.MiddleClick(delay_ms);
            stroke.Send();
        }

        static public void CenterClick(string img_path, int x, int y, int delay_ms = 1000,
            int retry_num = 10, int retry_sleep = 1000, double err_threshold = 0.05)
        {
            Thread.Sleep(delay_ms);
            var p = SerchImage(img_path, retry_num, retry_sleep, err_threshold);

            if (p.IsEmpty)
            {
                // if (System.Windows.MessageBox.Show("画像が一致しません。続行しますか？", "エラー", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                //     throw new Exception("画像が一致しませんでした。OKで続行します。");
                System.Windows.MessageBox.Show($"画像({Path.GetFileName(img_path)})との一致が見つかりません。" + Environment.NewLine + "OKで続行します。");
            }
            else
            {
                CenterClick(p.X + x, p.Y + y, 0);
            }
        }

        static public void MouseMoveTo(int x, int y, int delay_ms = 1000)
        {
            //Thread.Sleep(delay_ms);
            var stroke = new KeyStroke();
            stroke.MouseModeTo(x, y, 0);
            stroke.RightClick(delay_ms);
            stroke.Send();
        }

        static public void SendString(string str, int delay_ms = 1000)
        {
            Thread.Sleep(delay_ms);
            SendKeys.SendWait(str);
        }

        static public void Sleep(int ms)
        {
            Thread.Sleep(ms);
        }
    }
}
