using System;
using System.ComponentModel;
using Windows.Media.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPMoviePlayback
{
    public class PlayerProps : INotifyPropertyChanged
    {
        private void RaisePropChgEvent(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
        long duration;
        public long Duration
        {
            get
            {
                return duration;
            }
            set
            {
                if (duration == value)
                {
                    return;
                }
                duration = value;
                RaisePropChgEvent("Duration");
            }
        }

        long position;
        public long Position
        {
            get
            {
                return position;
            }
            set
            {
                if (position == value)
                {
                    return;
                }
                position = value;
                RaisePropChgEvent("Position");
            }
        }

        double moviePos;
        public double MoviePos
        {
            get
            {
                return moviePos;
            }
            set
            {
                if (moviePos == value)
                {
                    return;
                }
                moviePos = value;
                RaisePropChgEvent("MoviePos");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    };


    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        PlayerProps playerProps;
        private DispatcherTimer timer;

        public MainPage()
        {
            this.InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.1);
            timer.Tick += timer_Tick;

            this.seekBar.IsEnabled = false;
            playerProps = new PlayerProps();
            this.DataContext = playerProps;
        }

        private async void playBtn_Click(object sender, RoutedEventArgs e)
        {
            var uri = new Uri("ms-appx:///Assets/fishes.wmv");
            var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);
            if (file != null)
            {
                this.mediaElement.SetPlaybackSource(MediaSource.CreateFromStorageFile(file));
                this.mediaElement.Play();
            }
        }

        private void stopBtn_Click(object sender, RoutedEventArgs e)
        {
            this.mediaElement.Stop();
            this.timer.Stop();
        }

        private void mediaElement_Opened(object sender, RoutedEventArgs e)
        {
            this.playerProps.Position = this.mediaElement.Position.Ticks;
            this.playerProps.Duration = this.mediaElement.NaturalDuration.TimeSpan.Ticks;
            this.timer.Start();
            this.seekBar.IsEnabled = true;
        }

        void timer_Tick(object sender, object e)
        {
            this.playerProps.Position = this.mediaElement.Position.Ticks;
            this.playerProps.MoviePos = (this.playerProps.Position * 1000 / this.playerProps.Duration) / 1000.0;
        }

        private void SeekBar_ValueChanged(object sender, SeekBarValueChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ValueChanged : " + e.Value);
            long ticks = (long)(e.Value * playerProps.Duration);
            this.mediaElement.Position = TimeSpan.FromTicks(ticks);
        }
    }
}
