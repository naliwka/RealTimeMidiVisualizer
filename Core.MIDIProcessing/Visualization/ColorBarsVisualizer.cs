using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.MIDIProcessing.Visualization
{
    public class ColorBarsVisualizer : IVisualizer
    {
        private readonly double _windowWidth;
        private readonly double _windowHeight;

        private readonly List<VisualElementData> _bars = new();
        private const double BarWidth = 15;
        private const double ShiftSpeed = 5;

        public ColorBarsVisualizer(double windowWidth, double windowHeight)
        {
            _windowWidth = windowWidth;
            _windowHeight = windowHeight;
        }

        public List<VisualElementData> GenerateVisual(MidiEventData midiEvent)
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
            
            double barHeight = MapVelocityToHeight(midiEvent.Velocity);
            double barX = _windowWidth;

            var newBar = new VisualElementData
            {
                X = barX,
                Y = _windowHeight - barHeight,
                Size = barHeight,
                ColorHex = GetColorForNote(midiEvent.Note),
                Shape = "Rectangle",
                Opacity = 0.75
            };
            _bars.Add(newBar);
            return new List<VisualElementData>(_bars);
        }

        private string GetColorForNote(string? note)
        {
            if (note == null) return "#CCCCCC";

            if (note.StartsWith("C#")) return "#FF4500";   // OrangeRed
            if (note.StartsWith("D#")) return "#FFD700";   // Gold
            if (note.StartsWith("F#")) return "#00FF00";   // Lime
            if (note.StartsWith("G#")) return "#1E90FF";   // DodgerBlue
            if (note.StartsWith("A#")) return "#8A2BE2";   // BlueViolet

            if (note.StartsWith('C')) return "#FF0000";    // Red
            if (note.StartsWith('D')) return "#FFA500";    // Orange
            if (note.StartsWith('E')) return "#FFFF00";    // Yellow
            if (note.StartsWith('F')) return "#ADFF2F";    // GreenYellow
            if (note.StartsWith('G')) return "#00CED1";    // DarkTurquoise
            if (note.StartsWith('A')) return "#0000FF";    // Blue
            if (note.StartsWith('B')) return "#EE82EE";    // Violet

            return "#CCCCCC";  // Default gray
        }

        private double MapVelocityToHeight(int velocity)
        {
            const double minHeight = 20;
            double maxHeight = _windowHeight * 0.7;
            double actualHeight = minHeight + velocity / 127.0 * (maxHeight - minHeight);

            return actualHeight;
        }
    }
}
