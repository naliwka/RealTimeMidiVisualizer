using Core.MIDIProcessing.Helpers;
using Core.MIDIProcessing.Models;

namespace Core.MIDIProcessing.Visualization
{
    public class BubbleVisualizer : IVisualizer
    {

        private readonly NoteColorProvider _colorProvider;

        public BubbleVisualizer(NoteColorProvider colorProvider)
        {
            _colorProvider = colorProvider ?? new NoteColorProvider();
        }
        public List<VisualElementData> GenerateVisual(MidiEventData midiEvent, double windowWidth, double windowHeight)
        {
            var rnd = new Random();
            double minSize = 3;
            double maxSize = 150;

            var bubble = new VisualElementData
            {
                X = rnd.NextDouble() * windowWidth,
                Y = GetNoteY(midiEvent.NoteNumber, windowHeight),
                Size = Mapper.MapToRange(midiEvent.Velocity, minSize, maxSize),
                ColorHex = _colorProvider.GetColorForNote(midiEvent.Note),
                Shape = "Circle",
                Opacity = 0.75
            };
            return new List<VisualElementData> { bubble };
        }

        private double GetNoteY(int noteNumber, double windowHeight)
        {
            double noteHeight = noteNumber / 127.0;
            return windowHeight * (1.0 - noteHeight);
        }
    }
}
