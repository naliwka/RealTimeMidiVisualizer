using Core.MIDIProcessing.Helpers;
using Core.MIDIProcessing.Models;

namespace Core.MIDIProcessing.Visualization
{
    public class BubbleVisualizer : IVisualizer
    {
        private readonly double _windowWidth;
        private readonly double _windowHeight;
        private readonly NoteColorProvider _colorProvider;

        public BubbleVisualizer(double windowWidth, double windowHeight, NoteColorProvider colorProvider)
        {
            _windowWidth = windowWidth;
            _windowHeight = windowHeight;
            _colorProvider = colorProvider ?? new NoteColorProvider();
        }
        public List<VisualElementData> GenerateVisual(MidiEventData midiEvent)
        {
            var rnd = new Random();

            var bubble = new VisualElementData
            {
                X = rnd.NextDouble() * _windowWidth,
                Y = GetNoteY(midiEvent.NoteNumber),
                Size = MapVelocityToSize(midiEvent.Velocity),
                ColorHex = _colorProvider.GetColorForNote(midiEvent.Note),
                Shape = "Circle",
                Opacity = 0.75
            };
            return new List<VisualElementData> { bubble };
        }

        private double MapVelocityToSize(int velocity)
        {
            double minSize = 3;
            double maxSize = 150;
            return minSize + velocity / 127.0 * (maxSize - minSize);
        }

        private double GetNoteY(int noteNumber)
        {
            double noteHeight = noteNumber / 127.0;
            return _windowHeight * (1.0 - noteHeight);
        }
    }
}
