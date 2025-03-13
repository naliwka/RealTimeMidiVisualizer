using System.Diagnostics;
using Core.MIDIProcessing;
using NAudio.Midi;
namespace Infrastructure.MIDIInput
{
    public class MidiListener
    {
        private MidiIn? _midiIn;
        public event Action<MidiEventData> OnMidiEventReceived = delegate { };
        private readonly Dictionary<int, long> _noteStartTimes = new(); // Press time of each note

        public void StartListening(int midiInDevice)
        {
            if (_midiIn == null)
            {
                _midiIn = new MidiIn(midiInDevice);
            }
            _midiIn.Start();
            Debug.WriteLine($"Listening on {MidiIn.DeviceInfo(midiInDevice).ProductName}...");
        }
        private void midiIn_MessageReceived(object sender, MidiInMessageEventArgs e)
        {
            // Check if MIDI event is not null and check connection (AutoSensing)
            if (e.MidiEvent != null && e.MidiEvent.CommandCode == MidiCommandCode.AutoSensing)
            {
                return;
            }

            if (e.MidiEvent is NoteOnEvent noteOn && noteOn.Velocity > 0)
            {
                _noteStartTimes[noteOn.NoteNumber] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }
            else if (e.MidiEvent is NoteEvent noteEvent &&
                    (noteEvent.CommandCode == MidiCommandCode.NoteOff ||
                     (noteEvent.CommandCode == MidiCommandCode.NoteOn && noteEvent.Velocity == 0)))
            {
                if (_noteStartTimes.ContainsKey(noteEvent.NoteNumber))
                {
                    long startTime = _noteStartTimes[noteEvent.NoteNumber];
                    long endTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    double duration = (endTime - startTime) / 1000.0;

                    var midiEvent = new MidiEventData
                    {
                        Timestamp = startTime,
                        Note = noteEvent.NoteName,
                        Velocity = noteEvent.Velocity,
                        Duration = duration,
                        Source = "Device"
                    };
                    Debug.WriteLine($"Note: {midiEvent.Note}, Velocity: {midiEvent.Velocity}, Duration: {midiEvent.Duration}");

                    _noteStartTimes.Remove(noteEvent.NoteNumber); // Видаляємо запис про ноту
                    OnMidiEventReceived(midiEvent); // Відправляємо в GUI
                }
            }
        }
        public void StopListening()
        {
            if (_midiIn != null)
            {
                _midiIn.Stop();
                _midiIn.Dispose();
                _midiIn = null;
            }
        }
    }
}
