namespace Core.MIDIProcessing
{
    public interface IVisualGenerator
    {
        List<VisualElementData> GenerateVisual(MidiEventData midiEvent);
    }
}
