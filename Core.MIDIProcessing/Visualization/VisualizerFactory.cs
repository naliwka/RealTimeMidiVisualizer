using Core.MIDIProcessing.Helpers;

namespace Core.MIDIProcessing.Visualization
{
    public static class VisualizerFactory
    {
        public static IVisualizer CreateVisualizer(VisualizerType type, NoteColorProvider? provider = null)
        {
            var safeProvider = provider ?? new NoteColorProvider();
            switch (type)
            {
                case VisualizerType.Bubbles:
                    return new BubbleVisualizer(safeProvider);
                case VisualizerType.ColorBars:
                    return new ColorBarsVisualizer(safeProvider);
                default:
                    throw new ArgumentException("Unknown visualizer type");
            }
        }
    }
}
