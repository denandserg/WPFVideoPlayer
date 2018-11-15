using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Win32;

namespace WPFVideoPLAYER
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer _timer = new DispatcherTimer();
        Dictionary<string,string> fileDictionary = new Dictionary<string,string>();

        


        public MainWindow()
        {
            
            InitializeComponent();
            _timer.Interval = TimeSpan.FromMilliseconds(1000);
            _timer.Tick += new EventHandler(ticktock);
            _timer.Start();
            
            
        }

        void ticktock(object sender, EventArgs e)
        {
            ScorePlay.Content = ($"{sliderPlay.Value:0.0}");
            MediaProgress.Value = _media.Position.TotalSeconds;
            TrackTimeLabel.Content = ($"{_media.Position.Hours:00}:{_media.Position.Minutes:00}:{_media.Position.Seconds:00}");
            if(_media.Volume==0)
                imgMute.Source = new BitmapImage(new Uri("Resources/mute.png", UriKind.RelativeOrAbsolute));
            else
                imgMute.Source = new BitmapImage(new Uri("Resources/volume.png", UriKind.RelativeOrAbsolute));
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs ergs)
        {
            
            
        }
        
        public void _mediaMediaOpened(object sender, RoutedEventArgs e)
        {
            MediaProgress.Maximum = _media.NaturalDuration.TimeSpan.TotalSeconds;
            var totalDurationTime = _media.NaturalDuration.TimeSpan;
            ContentTitle.Content = $"{fileDictionary[HistoryListView.SelectedItem.ToString()]}  \t(Duration: {totalDurationTime})";


        }

        
        private void ButtonPlay_OnClick(object sender, RoutedEventArgs e)
        {
            _media.Play();
            sliderPlay.Value = 1.0;
        }

        private void ButtonStop_OnClick(object sender, RoutedEventArgs e)
        {
            _media.Stop();
        }

        private void ButtonPause_OnClick(object sender, RoutedEventArgs e)
        {
            if (PauseButton.Content.Equals("Pause"))
            {
                _media.Pause();
                PauseButton.Content = "Resume";
            }
            else 
            {
               _media.Play();
                PauseButton.Content = "Pause";
            }
                
            
        }

        private void MediaProgress_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            double x = e.GetPosition(MediaProgress).X;
            double pos = x*100 /MediaProgress.ActualWidth;
            _media.Position = TimeSpan.FromSeconds(_media.NaturalDuration.TimeSpan.TotalSeconds / 100.0 * pos);
        }

        private void OpenButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Files (*.*)|*.*| Video Files (*.mp4) |*.mp4";
            dlg.FilterIndex = 2;
            bool? res = dlg.ShowDialog();
            if (res.HasValue && res.Value)
            {
                fileDictionary.Add(dlg.SafeFileName, dlg.FileName);
                HistoryListView.Items.Add(dlg.SafeFileName);
                HistoryListView.SelectedIndex = 0;
                _media.MediaOpened += _mediaMediaOpened;
                _media.Source = new Uri(dlg.FileName);
                _media.Play();
                
            }
        }

        private void ClearHistoryButtonOnClick(object sender, RoutedEventArgs e)
        {
            HistoryListView.Items.Clear();
            fileDictionary.Clear();
            
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((Slider)sender).SelectionEnd = e.NewValue;
            _media.Volume = VolumeSlider.Value;
            
        }

        
        private void HistoryListView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DoubleClickHistory();
        }

        public void DoubleClickHistory()
        {
            _media.Source = new Uri(fileDictionary[HistoryListView.SelectedItem.ToString()]);
            _media.MediaOpened += _mediaMediaOpened;
            
            _media.Play();
           
        }

        

        private void NextButton_OnClickButton_Click()
        {
            if (HistoryListView.HasItems)
                HistoryListView.SelectedIndex++;
            DoubleClickHistory();
        }

        private void MuteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_media.Volume == 0.0)
            {
                imgMute.Source = new BitmapImage(new Uri("Resources/volume.png",UriKind.RelativeOrAbsolute));
                VolumeSlider.IsEnabled = true;
                _media.Volume = VolumeSlider.Value;
            }

            else
            {

                imgMute.Source = new BitmapImage(new Uri("Resources/mute.png", UriKind.RelativeOrAbsolute));
                _media.Volume = 0.0;
                VolumeSlider.IsEnabled = false;
            }
            
           
        }

        private void sliderPlay_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _media.SpeedRatio = e.NewValue;
        }

        private void AddPlayList_OnClick(object sender, RoutedEventArgs e)
        {
            HistoryListView.Items.Clear();
            fileDictionary.Clear();
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Files (*.*)|*.*| Video Files (*.mp4) |*.mp4";
            dlg.FilterIndex = 2;
            dlg.Multiselect = true;
            bool? res = dlg.ShowDialog();
            if (res.HasValue && res.Value)
            {
                foreach (var itemFileName in dlg.FileNames)
                {
                    fileDictionary.Add(System.IO.Path.GetFileName(itemFileName), itemFileName);
                }

                foreach (var tr in fileDictionary)
                {   
                    HistoryListView.Items.Add(tr.Key);
                }

                HistoryListView.SelectedIndex = 0;
            }
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            if (HistoryListView.HasItems && HistoryListView.SelectedIndex >= 1)
            {
                HistoryListView.SelectedIndex--;
                DoubleClickHistory();
            }
                


        }

        private void DeleteButton_OnClickButton_Click(object sender, RoutedEventArgs e)
        {
            HistoryListView.Items.Remove(HistoryListView.SelectedItem);
            HistoryListView.SelectedIndex = 0;
        }

        private void NextButton_OnClickButton_Click(object sender, RoutedEventArgs e)
        {
            if (HistoryListView.HasItems)
            {
                HistoryListView.SelectedIndex++;
                DoubleClickHistory();
            }
                

        }

        private void _media_OnMediaEnded(object sender, RoutedEventArgs e)
        {
            NextButton_OnClickButton_Click(sender,e);
        }
    }
}
