namespace Core.MIDIProcessing
{
    public interface IEventLogger
    {
        void Log(MidiEventData midiEvent);
    }
}
