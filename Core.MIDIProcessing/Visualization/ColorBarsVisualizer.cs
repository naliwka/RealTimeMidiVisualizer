using Core.MIDIProcessing.Helpers;
using Core.MIDIProcessing.Models;

namespace Core.MIDIProcessing.Visualization
{
    public class ColorBarsVisualizer : IVisualizer
    {
        private readonly NoteColorProvider _colorProvider;

        private readonly List<VisualElementData> _bars = new();
        private const double ShiftSpeed = 20;

        public ColorBarsVisualizer(NoteColorProvider colorProvider)
        {
            _colorProvider = _colorProvider ?? new NoteColorProvider();
        }

        public List<VisualElementData> GenerateVisual(MidiEventData midiEvent, double windowWidth, double windowHeight)
        {
            for (int i = _bars.Count - 1; i >= 0; i--)
            {
                var bar = _bars[i];
                bar.X -= ShiftSpeed;

                if (bar.X + bar.Size < 0)
                {
                    _bars.RemoveAt(i);
                }
            }
            
            double barHeight = MapVelocityToHeight(midiEvent.Velocity, windowHeight);
            double barX = windowWidth;

            var newBar = new VisualElementData
            {
                X = barX,
                Y = windowHeight - barHeight,
                Size = barHeight,
                ColorHex = _colorProvider.GetColorForNote(midiEvent.Note),
                Shape = "Rectangle",
                Opacity = 0.75
            };
            _bars.Add(newBar);
            return new List<VisualElementData>(_bars);
        }

        private double MapVelocityToHeight(int velocity, double windowHeight)
        {
            const double minHeight = 20;
            double maxHeight = windowHeight * 0.7;
            double actualHeight = minHeight + velocity / 127.0 * (maxHeight - minHeight);

            return actualHeight;
        }
    }
}
