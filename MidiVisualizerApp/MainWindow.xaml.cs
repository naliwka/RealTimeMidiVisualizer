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

namespace MidiVisualizerApp
{
    public partial class MainWindow : Window
    {
        private MidiListener _midiListener;
        private  _bubbleVisualizer;
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}