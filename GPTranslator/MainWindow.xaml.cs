using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GPTranslator
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		private string mevcutDil = "auto";
		private string çevrilenDil = "en";
		private string metin;
		private string çeviri;

		public MainWindow()
		{
			InitializeComponent();
			grid.DataContext = this;

		}

		public string Metin
		{
			get
			{
				if (!string.IsNullOrEmpty(metin))
				{
					Task.Factory.StartNew(() => Çeviri = DileÇevir(metin, MevcutDil, ÇevrilenDil), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
				}
				return metin;
			}

			set
			{
				if (metin != value)
				{
					metin = value;
					OnPropertyChanged(nameof(Metin));
					OnPropertyChanged(nameof(Çeviri));
				}
			}
		}

		public string Çeviri
		{
			get { return çeviri; }

			set
			{
				if (çeviri != value)
				{
					çeviri = value;
					OnPropertyChanged(nameof(Çeviri));
				}
			}
		}

		public string MevcutDil
		{
			get { return mevcutDil; }

			set
			{
				if (mevcutDil != value)
				{
					mevcutDil = value;
					OnPropertyChanged(nameof(MevcutDil));
					OnPropertyChanged(nameof(Metin));
				}
			}
		}

		public string ÇevrilenDil
		{
			get { return çevrilenDil; }

			set
			{
				if (çevrilenDil != value)
				{
					çevrilenDil = value;
					OnPropertyChanged(nameof(ÇevrilenDil));
					OnPropertyChanged(nameof(Metin));
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		private static string DileÇevir(string text, string from = "auto", string to = "en")
		{
			try
			{
				if (BağlantıVarmı())
				{
					var wc = new WebClient();
					wc.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0");
					wc.Headers.Add(HttpRequestHeader.AcceptCharset, "UTF-8");
					wc.Encoding = Encoding.UTF8;
					var url = $"http://translate.google.com.tr/m?hl=en&sl={from}&tl={to}&ie=UTF-8&prev=_m&q={Uri.EscapeUriString(text)}";
					var page = wc.DownloadString(url);
					int count = page.IndexOf("<div dir=\"ltr\" class=\"t0\">");
					if (count >= 0)
					{
						page = page.Remove(0, count).Replace("<div dir=\"ltr\" class=\"t0\">", "").Replace("&#39;", "'");
						var last = page.IndexOf("</div>");
						return page.Remove(last, page.Length - last);
					}
				}
				return string.Empty;
			}
			catch (Exception)
			{
				return string.Empty;
			}
		}

		private static bool BağlantıVarmı()
		{
			try
			{
				var i = Dns.GetHostEntry("www.google.com");
				return true;
			}
			catch
			{
				return false;
			}
		}

		private void MenuItem_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

		private void HakkındaItem_Click(object sender, RoutedEventArgs e) => Process.Start("https://www.facebook.com/profile.php?id=100002547964778");

		private void BtnCap_Click(object sender, RoutedEventArgs e)
		{
			if (BağlantıVarmı())
			{
				var captureWindow = new CaptureWindow();
				(sender as Button).DataContext = captureWindow;
				captureWindow.ShowDialog();
				var ms = captureWindow.EkranMetni;
				Task.Factory.StartNew(() => Metin = ms.OcrYap(), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);

			}
		}
	}
}
