using Core.MIDIProcessing.Models;

namespace Core.MIDIProcessing.Visualization
{
    public interface IVisualizer
    {
        List<VisualElementData> GenerateVisual(MidiEventData midiEvent, double windowWidth, double windowHeight);
    }
}
