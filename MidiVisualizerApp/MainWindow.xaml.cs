using System.Text;
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
using Core.MIDIProcessing;
using Infrastructure.MIDIInput;
using NAudio.Midi;

namespace MidiVisualizerApp
{
    public partial class MainWindow : Window
    {
        private MidiListener? _midiListener;
        private IVisualizer? _visualizer;
        private int _frameCount = 0;
        private DateTime _lastFpsUpdate = DateTime.Now;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                LoadMidiDevices();
            };
        }
        private void LoadMidiDevices()
        {
            MidiDeviceComboBox.Items.Clear();
            int deviceCount = MidiIn.NumberOfDevices;

            for (int i = 0; i < deviceCount; i++)
            {
                var deviceInfo = MidiIn.DeviceInfo(i);
                MidiDeviceComboBox.Items.Add(new ComboBoxItem
                {
                    Content = new MidiDeviceItem
                    {
                        Index = i,
                        Name = deviceInfo.ProductName
                    }
                });
            }

            if (MidiDeviceComboBox.Items.Count > 0)
                MidiDeviceComboBox.SelectedIndex = 0;
        }

        private void StartListening_Click(object sender, RoutedEventArgs e)
        {
            if (MidiDeviceComboBox.SelectedItem is ComboBoxItem comboBoxItem &&
         comboBoxItem.Content is MidiDeviceItem selectedDevice)
            {

                double width = MyCanvas.ActualWidth;
                double height = MyCanvas.ActualHeight;

                _visualizer = VisualizerFactory.CreateVisualizer(
                    VisualizerType.Bubbles, width, height);

                _midiListener = new MidiListener();

                _midiListener.OnMidiEventReceived += MidiEventReceived;

                _midiListener.StartListening(selectedDevice.Index);
            }
        }

        private void StopListening_Click(object sender, RoutedEventArgs e)
        {
            if (_midiListener != null)
            {
                _midiListener.StopListening();

                _midiListener.OnMidiEventReceived -= MidiEventReceived;
                _midiListener = null;

                MessageBox.Show("Stopped listening to MIDI input.",
                                "MIDI", MessageBoxButton.OK, MessageBoxImage.Information);
                MyCanvas.Children.Clear();
            }
        }

        private void MidiEventReceived(MidiEventData midiEvent)
        {
            if (_visualizer == null) return;

            Dispatcher.Invoke(() => NoteText.Text = $"{midiEvent.Note}");

            var visuals = _visualizer.GenerateVisual(midiEvent);

            Dispatcher.Invoke(() => RenderVisuals(visuals));
        }
        private void RenderVisuals(List<VisualElementData> visuals)
        {
            foreach (var visual in visuals)
            {
                var ellipse = new Ellipse
                {
                    Width = visual.Size,
                    Height = visual.Size,
                    Fill = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString(visual.ColorHex)),
                    Opacity = visual.Opacity
                };

                Canvas.SetLeft(ellipse, visual.X);
                Canvas.SetTop(ellipse, visual.Y);
                MyCanvas.Children.Add(ellipse);

                AnimateAndRemove(ellipse);
            }
            UpdateFps();
        }
        private void UpdateFps()
        {
            _frameCount++;
            var now = DateTime.Now;

            if ((now - _lastFpsUpdate).TotalSeconds >= 1)
            {
                Dispatcher.Invoke(() => FpsText.Text = $"{_frameCount}");
                _frameCount = 0;
                _lastFpsUpdate = now;
            }
        }
        private void AnimateAndRemove(UIElement element)
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(30) };
            double shrinkRate = 0.95;

            timer.Tick += (s, e) =>
            {
                if (element is Ellipse ellipse)
                {
                    ellipse.Width *= shrinkRate;
                    ellipse.Height *= shrinkRate;
                    ellipse.Opacity *= shrinkRate;

                    if (ellipse.Opacity < 0.05)
                    {
                        timer.Stop();
                        MyCanvas.Children.Remove(ellipse);
                    }
                }
            };
            timer.Start();
        }
    }
    public class MidiDeviceItem
    {
        public int Index { get; set; }
        public string Name { get; set; } = "";

        public override string ToString() => Name;
    }

}
