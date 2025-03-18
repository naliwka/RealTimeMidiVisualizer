using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.MIDIProcessing.Visualization
{
    public static class VisualizerFactory
    {
        public static IVisualizer CreateVisualizer(VisualizerType type, double windowWidth, double windowHeight)
        {
            switch (type)
            {
                case VisualizerType.Bubbles:
                    return new BubbleVisualizer(windowWidth, windowHeight);
                default:
                    throw new ArgumentException("Unknown visualizer type");
            }
        }
    }
}
