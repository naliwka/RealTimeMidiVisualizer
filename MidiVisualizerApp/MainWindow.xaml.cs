using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Core.MIDIProcessing;
using Infrastructure.MIDIInput;
using NAudio.Midi;
using Infrastructure.Logging;
using Core.MIDIProcessing.Visualization;
using MidiVisualizerApp.Models;
using System.Windows.Media.Media3D;
using Core.MIDIProcessing.Helpers;
using MidiVisualizerApp.Helpers;
using Core.MIDIProcessing.Logging;
using Core.MIDIProcessing.Models;

namespace MidiVisualizerApp
{
    public partial class MainWindow : Window
    {
        private MidiListener? _midiListener;
        private IVisualizer? _visualizer;
        private IEventLogger? _logger = new RedisLogger();

        private int _frameCount = 0;
        private DateTime _lastFpsUpdate = DateTime.Now;

        public MainWindow()
        {
            InitializeComponent();
            PopulateVisualizerTypeComboBox();

            Loaded += (s, e) => LoadMidiDevices();
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

        private VisualizerType GetSelectedVisualizerType()
        {
            var selectedItem = VisualizerTypeComboBox.SelectedItem as VisualizerTypeItem;
            return selectedItem?.Type ?? VisualizerType.Bubbles;
        }

        private void StartListening_Click(object sender, RoutedEventArgs e)
        {
            if (_midiListener != null)
            {
                MessageBox.Show("Already listening to MIDI input.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MidiDeviceComboBox.SelectedItem is ComboBoxItem comboBoxItem &&
                comboBoxItem.Content is MidiDeviceItem selectedDevice)
            {
                InitializeVisualizer();
                InitializeMidiListener(selectedDevice.Index);
            }
        }

        private void InitializeVisualizer()
        {
            double width = MyCanvas.ActualWidth;
            double height = MyCanvas.ActualHeight;
            var selectedType = GetSelectedVisualizerType();
            var colorProvider = new NoteColorProvider();

            _visualizer = VisualizerFactory.CreateVisualizer(selectedType, width, height, colorProvider);
        }

        private void InitializeMidiListener(int deviceIndex)
        {
            _midiListener = new MidiListener();
            _midiListener.OnNoteOnReceived += MidiNoteOnReceived;
            _midiListener.OnMidiEventReceived += MidiNoteOffReceived;
            _midiListener.StartListening(deviceIndex);
        }

        private void StopListening_Click(object sender, RoutedEventArgs e)
        {
            if (_midiListener != null)
            {
                _midiListener.StopListening();
                _midiListener.OnNoteOnReceived -= MidiNoteOnReceived;
                _midiListener.OnMidiEventReceived -= MidiNoteOffReceived;
                _midiListener = null;

                MessageBox.Show("Stopped listening to MIDI input.", "MIDI", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearCanvas();
            }
        }

        private void ClearCanvas()
        {
            MyCanvas.Children.Clear();
            NoteText.Text = "-";
            FpsText.Text = "0";
        }

        private void MidiNoteOnReceived(MidiEventData midiEvent)
        {
            if (_visualizer == null) return;

            var visuals = _visualizer.GenerateVisual(midiEvent);

            Dispatcher.Invoke(() =>
            {
                RenderVisuals(visuals);
                NoteText.Text = midiEvent.Note;
                UpdateFps();
            });
        }

        private void MidiNoteOffReceived(MidiEventData midiEvent)
        {
            Dispatcher.Invoke(() =>
            {
                UpdateFps();
                if (EnableLoggingCheckBox.IsChecked == true)
                    _logger?.Log(midiEvent);
            });
        }

        private void RenderVisuals(List<VisualElementData> visuals)
        {
            foreach (var visual in visuals)
            {
                Shape shape = ShapeFactory.CreateShape(visual);
                Canvas.SetLeft(shape, visual.X);
                Canvas.SetTop(shape, visual.Y);
                MyCanvas.Children.Add(shape);
                AnimateAndRemove(shape);
            }
        }

        private void UpdateFps()
        {
            _frameCount++;
            var now = DateTime.Now;

            if ((now - _lastFpsUpdate).TotalSeconds >= 1)
            {
                FpsText.Text = $"{_frameCount}";
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
                if (element is Shape shape)
                {
                    if (shape is Ellipse)
                    {
                        shape.Width *= shrinkRate;
                        shape.Height *= shrinkRate;
                        shape.Opacity *= shrinkRate;
                    }

                    if (shape.Opacity < 0.05)
                    {
                        timer.Stop();
                        MyCanvas.Children.Remove(shape);
                    }
                }
            };
            timer.Start();
        }

        private void PopulateVisualizerTypeComboBox()
        {
            var items = Enum.GetValues(typeof(VisualizerType))
                .Cast<VisualizerType>()
                .Select(vt => new VisualizerTypeItem
                {
                    Type = vt,
                    DisplayName = vt.ToString().Replace("ColorBars", "Color Bars")
                })
                .ToList();

            VisualizerTypeComboBox.ItemsSource = items;
            VisualizerTypeComboBox.SelectedIndex = 0;
        }
    }
    public class MidiDeviceItem
    {
        public int Index { get; set; }
        public string Name { get; set; } = "";

        public override string ToString() => Name;
    }

}
