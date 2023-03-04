using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ListBox = System.Windows.Controls.ListBox;
using Timer = System.Timers.Timer;

namespace AudioPlayr
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Timer timer = new Timer(100);
        private string folderPath;
        private List<string> files = new List<string>();
        private int position = 0;
        // private bool IsPaused = false;
        private bool IsRepeat = false;
        public MainWindow()
        {
            InitializeComponent();
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (!media.HasAudio || SL_time.IsMouseCaptured)
                    return;

                SL_time.Value = media.Position.TotalSeconds;
                LB_curTime.Content = media.Position.ToString();
                LB_EndTime.Content = (media.NaturalDuration - media.Position).ToString();

            }));
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = @"K:\Projects\teach\retas\AudioPlayr\AudioPlayr\Ливадный Андрей - Борт 618 (читает Scaners)";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                folderPath = dialog.SelectedPath;
                DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
                directoryInfo.GetFiles("*.mp3");
                foreach (var file in directoryInfo.GetFiles())
                    files.Add(file.FullName);
                printLBItems();
                Play();
            }

        }

        private void printLBItems()
        {
            LB_Music.Items.Clear();
            foreach (var file in files)
            {
                ListBoxItem item = new ListBoxItem()
                {
                    Content = file.Split('\\').Last(),
                };
                item.Selected += Item_Selected;
                LB_Music.Items.Add(item);
            }
            LB_Music.SelectedIndex = 0;
            position = 0;
        }
        private void Item_Selected(object sender, RoutedEventArgs e)
        {
            if (LB_Music.SelectedIndex >= 0)
            {
                position = LB_Music.SelectedIndex;
                Play();
            }
        }

        private void Next()
        {
            if (position >= files.Count - 1)
                position = 0;
            else
                position++;

            media.Source = new Uri(files[position]);
            SL_time.Maximum = media.NaturalDuration.TimeSpan.TotalSeconds;
            SL_time.Value = 0;
            LB_Music.SelectedIndex = position;
            media.Play();

        }
        private void Back()
        {
            if (position >= 0)
                position = files.Count - 1;
            else
                position--;
            media.Source = new Uri(files[position]);
            SL_time.Maximum = media.NaturalDuration.TimeSpan.TotalSeconds;
            SL_time.Value = 0;
            LB_Music.SelectedIndex = position;
            media.Play();
        }
        private void Play()
        {
            if (files.Count > 0)
            {
                media.Source = new Uri(files[position]);
                SL_time.Value = 0;
                while (media.NaturalDuration == Duration.Automatic)
                {
                    Thread.Sleep(10);
                    media.Play();

                }
                SL_time.Maximum = media.NaturalDuration.TimeSpan.TotalSeconds;

            }
        }
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Back();
        }

        private void button1_Copy_Click(object sender, RoutedEventArgs e)
        {
            Next();
        }

        private void button1_Copy1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void button1_Copy2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void button1_Copy3_Click(object sender, RoutedEventArgs e)
        {
            media.Stop();
            string temp;
            int rndIndex = 0;
            var rnd = new Random();
            for (int i = 0; i < files.Count; i++)
            {
                rndIndex = rnd.Next(0,files.Count);
                temp = files[i];
                files[i] = files[rndIndex];
                files[rndIndex] = temp;
            }
            printLBItems();
        }

        private void SL_time_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            media.Position = TimeSpan.FromSeconds(e.NewValue);
        }

        private void slider_Copy_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //звук
            if (!IsLoaded)
                return;
            media.Volume = e.NewValue / 100;
            LB_Volume.Content = $"громкость: {(int)e.NewValue}%";
        }


        private void BTN_Next_Click(object sender, RoutedEventArgs e)
        {
            Next();
        }

        private void BTN_Back_Click(object sender, RoutedEventArgs e)
        {
            Back();
        }

        private void BTN_Play_Click(object sender, RoutedEventArgs e)
        {
            Play();
        }

        private void BTN_Repeat_Click(object sender, RoutedEventArgs e)
        {
            IsRepeat = !IsRepeat;
        }

        private void media_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (IsRepeat)
            {
                media.Position = new TimeSpan(0, 0, 1);
                media.Play();
            }
            else
            {
                Next();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            timer.Stop();
        }

        private void LB_Music_Selected(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
