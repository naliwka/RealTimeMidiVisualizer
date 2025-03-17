using System.Diagnostics;
using Core.MIDIProcessing;
using NAudio.Midi;
namespace Infrastructure.MIDIInput
{
    public class MidiListener
    {
        private MidiIn? _midiIn;
        public event Action<MidiEventData> OnMidiEventReceived = delegate { };
        private readonly Dictionary<int, long> _noteStartTimes = new();
        private readonly Dictionary<int, int> _noteVelocities = new();

        public void StartListening(int midiInDevice)
        {
            if (_midiIn == null)
            {
                _midiIn = new MidiIn(midiInDevice);
                _midiIn.MessageReceived += MidiIn_MessageReceived;
                _midiIn.ErrorReceived += (s, e) => Debug.WriteLine($"MIDI Error: {e}");
            }
            _midiIn.Start();
            Debug.WriteLine($"Listening on {MidiIn.DeviceInfo(midiInDevice).ProductName}...");
        }
        private void MidiIn_MessageReceived(object? sender, MidiInMessageEventArgs e)
        {
            if (e.MidiEvent != null && e.MidiEvent.CommandCode == MidiCommandCode.AutoSensing)
            {
                return;
            }

            if (e.MidiEvent is NoteOnEvent noteOn)
            {
                Debug.WriteLine($"Note On: {noteOn.NoteNumber}, Velocity: {noteOn.Velocity}");
                if (noteOn.Velocity > 0)
                {
                    _noteStartTimes[noteOn.NoteNumber] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    _noteVelocities[noteOn.NoteNumber] = noteOn.Velocity;
                }
                else
                {
                    HandleNoteEvent(noteOn.NoteNumber, noteOn.NoteName);
                }               
            }
            else if (e.MidiEvent is NoteEvent noteEvent && noteEvent.CommandCode == MidiCommandCode.NoteOff)
            {
                Debug.WriteLine($"Note Off: {noteEvent.NoteNumber}");
                HandleNoteEvent(noteEvent.NoteNumber, noteEvent.NoteName);
            }
        }
        private void HandleNoteEvent(int noteNumber, string noteName)
        {
            if (_noteStartTimes.ContainsKey(noteNumber))
            {
                long startTime = _noteStartTimes[noteNumber];
                long endTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                double duration = (endTime - startTime) / 1000.0;
                int velocity = _noteVelocities.ContainsKey(noteNumber) ? _noteVelocities[noteNumber] : 0;

                var midiEvent = new MidiEventData
                {
                    Timestamp = startTime,
                    Note = noteName,
                    NoteNumber = noteNumber,
                    Velocity = velocity,
                    Duration = duration,
                    Source = "Device"
                };

                _noteStartTimes.Remove(noteNumber);
                _noteVelocities.Remove(noteNumber);
                OnMidiEventReceived?.Invoke(midiEvent);
            }
            else
            {
                Debug.WriteLine($"Warning: NoteOff received for unknown Note {noteNumber}");
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
