#region

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using ChatApp.WPF.Parser;

#endregion

namespace ChatApp.WPF
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly KakaoParser _kakao = new KakaoParser();
        public readonly DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            _kakao.Init();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timer.Interval = TimeSpan.FromTicks(5000);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
        }

        private void SelectPlayerButton_OnClick(object sender, RoutedEventArgs e)
        {
            MyContextMenu.Items.Clear();

            if (_kakao.FindPotPlayer())
            {
                foreach (var windowInfo in _kakao.GetPlayerList())
                {
                    MyContextMenu.Items.Add(windowInfo.caption);
                }
            }
            else
            {
                MyContextMenu.Items.Add("Can't Find Player");
            }
            MyContextMenu.IsOpen = true;
        }
    }
}