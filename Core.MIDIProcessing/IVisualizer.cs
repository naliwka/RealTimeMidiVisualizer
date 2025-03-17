namespace Core.MIDIProcessing
{
    public interface IVisualizer
    {
        List<VisualElementData> GenerateVisual(MidiEventData midiEvent);
    }
}
