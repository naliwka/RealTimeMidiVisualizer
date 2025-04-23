using Core.MIDIProcessing.Models;

namespace Infrastructure.Logging
{
    public interface IEventLogger
    {
        void Log(MidiEventData midiEvent);
    }
}
