using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace AudioPlayr
{
    public partial class MainWindow : Window
    {
        private bool isPause = false;
        private Timer timer = new Timer(100);
        private string folderPath;
        private List<string> files = new List<string>();
        private int position = 0;
        private bool isRandom = false;
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
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                folderPath = dialog.SelectedPath;
                Load();
            }
        }
        private void Load()
        {

            files.Clear();
            DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
            foreach (var file in directoryInfo.GetFiles("*.mp3"))
                files.Add(file.FullName);
            printLBItems();
            Play();
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
                LB_Music.Items.Add(item);
            }
            LB_Music.SelectedIndex = 0;
            position = 0;
        }
        private void Next()
        {
            {
                if (position >= files.Count - 1)
                    position = 0;
                else
                    position++;

                media.Source = new Uri(files[position]);
                media.Play();
                while (media.NaturalDuration == Duration.Automatic)
                {
                    Thread.Sleep(10);
                }
                SL_time.Maximum = media.NaturalDuration.TimeSpan.TotalSeconds;
                SL_time.Value = 0;
                LB_Music.SelectedIndex = position;

            }
        }
        private void Back()
        {
            if (position >= 0)
                position = files.Count - 1;
            else
                position--;
            media.Source = new Uri(files[position]);
            media.Play();
            while (media.NaturalDuration == Duration.Automatic)
            {
                Thread.Sleep(10);
            }
            SL_time.Maximum = media.NaturalDuration.TimeSpan.TotalSeconds;
            SL_time.Value = 0;
            LB_Music.SelectedIndex = position;
        }
        private void Play()
        {
            if (files.Count > 0)
            {
                media.Source = new Uri(files[position]);
                SL_time.Value = 0;
                media.Play();
                while (media.NaturalDuration == Duration.Automatic)
                {
                    Thread.Sleep(10);
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


        private void button1_Copy3_Click(object sender, RoutedEventArgs e)
        {
            media.Stop();
            isRandom = !isRandom;
            if (isRandom)
            {
                string temp;
                int rndIndex = 0;
                var rnd = new Random();
                for (int i = 0; i < files.Count; i++)
                {
                    rndIndex = rnd.Next(0, files.Count);
                    temp = files[i];
                    files[i] = files[rndIndex];
                    files[rndIndex] = temp;
                }
            }
            else
            {
                Load();
            }
            printLBItems();
            position = 0;
            Play();
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

        private void BTN_Play_Copy_Click(object sender, RoutedEventArgs e)
        {
            isPause = !isPause;
            if (isPause)
                media.Pause();
            else
                media.Play();
        }

        private void LB_Music_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LB_Music.SelectedIndex >= 0)
            {
                position = LB_Music.SelectedIndex;
                Play();
            }

        }
    }
}
