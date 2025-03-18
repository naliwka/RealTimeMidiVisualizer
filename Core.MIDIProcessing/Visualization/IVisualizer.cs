namespace Core.MIDIProcessing.Visualization
{
    public interface IVisualizer
    {
        List<VisualElementData> GenerateVisual(MidiEventData midiEvent);
    }
}
