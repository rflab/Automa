using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Interop;

namespace Automa
{
    /// <summary>
    /// screen.xaml の相互作用ロジック
    /// </summary>
    public partial class ScreenSelection : Window
    {
        public enum Selection { Point, Line, Rectangle };

        public System.Drawing.Point StartPoint { get; set; }
        public System.Drawing.Point EndPoint { get; set; }
        public Selection SelectionShape { get; set; }

        public ScreenSelection()
        {
            InitializeComponent();
        }

        public System.Drawing.Rectangle ShowGetRectDialog(System.Drawing.Bitmap screen, string message)
        {
            image.Source = Imaging.CreateBitmapSourceFromHBitmap(
                screen.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            canvas.UpdateLayout();

            MessageLabel.Content = message;
            SelectionShape = Selection.Rectangle;
            if (ShowDialog() ?? false)
            {
                return new System.Drawing.Rectangle(
                    Math.Min(StartPoint.X, EndPoint.X),
                    Math.Min(StartPoint.Y, EndPoint.Y),
                    Math.Max(1, Math.Abs(StartPoint.X - EndPoint.X)),
                    Math.Max(1, Math.Abs(StartPoint.Y - EndPoint.Y))); ;
            }
            return System.Drawing.Rectangle.Empty;
        }

        public System.Drawing.Point ShowGetPointDialog(System.Drawing.Bitmap screen, string message)
        {
            image.Source = Imaging.CreateBitmapSourceFromHBitmap(
                screen.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            canvas.UpdateLayout();

            MessageLabel.Content = message;
            SelectionShape = Selection.Point;
            if (ShowDialog() ?? false)
            {
                return EndPoint;
            }
            return System.Drawing.Point.Empty;
        }

        public System.Drawing.Point[] ShowGetLineDialog(System.Drawing.Bitmap screen, string message)
        {
            image.Source = Imaging.CreateBitmapSourceFromHBitmap(
                screen.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            canvas.UpdateLayout();

            MessageLabel.Content = message;
            SelectionShape = Selection.Line;
            if (ShowDialog() ?? false)
            {
                return new System.Drawing.Point[] { StartPoint, EndPoint };
            }
            return null;
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                EndPoint = new System.Drawing.Point((int)e.GetPosition(this).X, (int)e.GetPosition(this).Y);
                ReleaseMouseCapture();
                DialogResult = true;
                Close();
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                DialogResult = false;
                Close();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CaptureMouse();
            StartPoint = new System.Drawing.Point((int)e.GetPosition(this).X, (int)e.GetPosition(this).Y); ;

            switch (SelectionShape)
            {
                case Selection.Point:
                    break;
                case Selection.Line:
                    lineShape.X1 = StartPoint.X;
                    lineShape.Y1 = StartPoint.Y;
                    lineShape.X2 = StartPoint.X;
                    lineShape.Y2 = StartPoint.Y;
                    lineShape.Visibility = Visibility.Visible;
                    break;
                case Selection.Rectangle:
                    Canvas.SetLeft(rectShape, StartPoint.X);
                    Canvas.SetTop(rectShape, StartPoint.Y);
                    rectShape.Width = 0;
                    rectShape.Height = 0;
                    rectShape.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsMouseCaptured)
            {
                var cur = e.GetPosition(this);

                switch (SelectionShape)
                {
                    case Selection.Point:
                        break;
                    case Selection.Line:
                        lineShape.X2 = cur.X;
                        lineShape.Y2 = cur.Y;
                        break;
                    case Selection.Rectangle:
                        Canvas.SetLeft(rectShape, Math.Min(StartPoint.X, cur.X));
                        Canvas.SetTop(rectShape, Math.Min(StartPoint.Y, cur.Y));
                        rectShape.Width = Math.Max(1, Math.Abs(StartPoint.X - cur.X));
                        rectShape.Height = Math.Max(1, Math.Abs(StartPoint.Y - cur.Y));
                        break;
                }
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
                Close();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }
    }
}

