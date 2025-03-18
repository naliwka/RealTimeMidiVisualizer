namespace Core.MIDIProcessing.Visualization
{
    public class BubbleVisualizer : IVisualizer
    {
        private readonly double _windowWidth;
        private readonly double _windowHeight;
        public BubbleVisualizer(double windowWidth, double windowHeight)
        {
            _windowWidth = windowWidth;
            _windowHeight = windowHeight;
        }
        public List<VisualElementData> GenerateVisual(MidiEventData midiEvent)
        {
            var rnd = new Random();

            var bubble = new VisualElementData
            {
                X = rnd.NextDouble() * _windowWidth,
                Y = GetNoteY(midiEvent.NoteNumber),
                Size = MapVelocityToSize(midiEvent.Velocity),
                ColorHex = GetColorForNote(midiEvent.Note),
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

        private string GetColorForNote(string? note)
        {
            if (note == null) return "#CCCCCC";
            if (note.StartsWith('C')) return "#FF0000";
            if (note.StartsWith('D')) return "#FFA500";
            if (note.StartsWith('E')) return "#FFFF00";
            if (note.StartsWith('F')) return "#008000";
            if (note.StartsWith('G')) return "#0000FF";
            if (note.StartsWith('A')) return "#4B0082";
            if (note.StartsWith('B')) return "#EE82EE";
            return "#CCCCCC";
        }

        private double GetNoteY(int noteNumber)
        {
            double noteHeight = noteNumber / 127.0;
            return _windowHeight * (1.0 - noteHeight);
        }
    }
}
