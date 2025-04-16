using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.MIDIProcessing.Helpers;

namespace Core.MIDIProcessing.Visualization
{
    public static class VisualizerFactory
    {
        public static IVisualizer CreateVisualizer(VisualizerType type, NoteColorProvider? provider = null)
        {
            switch (type)
            {
                case VisualizerType.Bubbles:
                    return new BubbleVisualizer(provider);
                case VisualizerType.ColorBars:
                    return new ColorBarsVisualizer(provider);
                default:
                    throw new ArgumentException("Unknown visualizer type");
            }
        }
    }
}
