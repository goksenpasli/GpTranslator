using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace GPTranslator
{
    /// <summary>
    /// Interaction logic for CaptureWindow.xaml
    /// </summary>
    public partial class CaptureWindow : Window,INotifyPropertyChanged
    {
        public double x;
        public double y;
        public double width;
        public double height;
        public bool isMouseDown;
        private byte[] ekranMetni;

        public byte[] EkranMetni
        {
            get { return ekranMetni; }

            set
            {
                if (ekranMetni != value)
                {
                    ekranMetni = value;
                    OnPropertyChanged(nameof(EkranMetni));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public CaptureWindow()
        {
            InitializeComponent();
            Cursor = Cursors.Cross;
        }

        protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isMouseDown = true;
            x = e.GetPosition(null).X;
            y = e.GetPosition(null).Y;
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                double curx = e.GetPosition(null).X;
                double cury = e.GetPosition(null).Y;

                System.Windows.Shapes.Rectangle r = new System.Windows.Shapes.Rectangle
                {
                    Stroke = new SolidColorBrush(Colors.Red),
                    Fill = new SolidColorBrush(Colors.White),
                    StrokeThickness = 2,
                    Width = Math.Abs(curx - x),
                    Height = Math.Abs(cury - y)
                };
                cnv.Children.Clear();
                cnv.Children.Add(r);
                Canvas.SetLeft(r, x);
                Canvas.SetTop(r, y);
                if (e.LeftButton == MouseButtonState.Released)
                {
                    cnv.Children.Clear();
                    width = e.GetPosition(null).X - x;
                    height = e.GetPosition(null).Y - y;
                    EkranMetni = CaptureScreen(x, y, width, height);
                    x = y = 0;
                    isMouseDown = false;
                    Cursor = Cursors.Arrow;
                    Close();
                }
            }
        }

        private byte[] CaptureScreen(double x, double y, double width, double height)
        {
            try
            {
                var size = new Size((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight);
                var pointOfOrigin = new Point((int)x, (int)y);
                using (var ms = new MemoryStream())
                {
                    using (Bitmap bitmap = new Bitmap((int)width, (int)height))
                    {

                        using (Graphics graphics = Graphics.FromImage(bitmap))
                        {
                            graphics.CopyFromScreen(pointOfOrigin, new Point(0, 0), size);
                            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                            return ms.ToArray();
                        }

                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Seçim Geçerli Değil", "GPTranslator", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return null;
            }
        }

    }
}
