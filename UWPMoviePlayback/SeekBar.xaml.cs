using System;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace UWPMoviePlayback
{
    public class SeekBarValueChangedEventArgs : System.EventArgs
    {
        private double value;

        public SeekBarValueChangedEventArgs(double value)
        {
            this.value = value;
        }

        public double Value
        {
            get
            {
                return this.value;
            }
        }
    }

    public class SeekBarProps : INotifyPropertyChanged
    {
        double hPos;
        public double HPos
        {
            get
            {
                return hPos;
            }
            set
            {
                if (hPos == value)
                {
                    return;
                }
                hPos = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("HPos"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public sealed partial class SeekBar : UserControl
    {
        private SeekBarProps seekBarProps;
        private double thumbX = 0;
        private bool dragEntered = false;
        private double value = 0;

        public delegate void ValueChangedDelegate(object sender, SeekBarValueChangedEventArgs e);
        public event ValueChangedDelegate ValueChanged;

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
          DependencyProperty.Register(
            "Value", typeof(double), typeof(SeekBar),
            new PropertyMetadata(null, new PropertyChangedCallback(OnValueChanged))
          );

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thisInstance = d as SeekBar;
            thisInstance.MoveThumb((double)e.NewValue);
        }

        public SeekBar()
        {
            this.InitializeComponent();
            seekBarProps = new SeekBarProps();
            this.SliderBase.DataContext = seekBarProps;
            this.value = 0;
        }

        public void MoveThumb(double value)
        {
            var width = this.SliderBase.ActualWidth - this.PositionThumb.ActualWidth;
            this.value = value;
            if (this.dragEntered == false)
            {
                seekBarProps.HPos = width * value;
            }
        }

        private void PositionThumb_DragDelta(object sender, Windows.UI.Xaml.Controls.Primitives.DragDeltaEventArgs e)
        {
            var width = this.SliderBase.ActualWidth - this.PositionThumb.ActualWidth;
            thumbX += e.HorizontalChange;
            seekBarProps.HPos = Math.Min(width, Math.Max(0, thumbX));
        }

        private void PositionThumb_DragStarted(object sender, Windows.UI.Xaml.Controls.Primitives.DragStartedEventArgs e)
        {
            thumbX = seekBarProps.HPos;
            dragEntered = true;
        }

        private void PositionThumb_DragCompleted(object sender, Windows.UI.Xaml.Controls.Primitives.DragCompletedEventArgs e)
        {
            var width = this.SliderBase.ActualWidth - this.PositionThumb.ActualWidth;
            dragEntered = false;
            if (this.ValueChanged != null)
            {
                this.value= seekBarProps.HPos / width;
                this.ValueChanged(this, new SeekBarValueChangedEventArgs(this.value));
            }
        }

        private void PositionThumb_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var width = this.SliderBase.ActualWidth - this.PositionThumb.ActualWidth;
            if (this.dragEntered == false)
            {
                seekBarProps.HPos = width * this.value;
            }
        }
    }
}
