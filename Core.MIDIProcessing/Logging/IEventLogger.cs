using Core.MIDIProcessing.Models;

namespace Core.MIDIProcessing.Logging
{
    public interface IEventLogger
    {
        void Log(MidiEventData midiEvent);
    }
}
