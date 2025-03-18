using System.Diagnostics;
using Core.MIDIProcessing;
using NAudio.Midi;
namespace Infrastructure.MIDIInput
{
    public class MidiListener
    {
        private MidiIn? _midiIn;
        public event Action<MidiEventData>? OnNoteOnReceived;
        public event Action<MidiEventData>? OnMidiEventReceived;
        private readonly Dictionary<int, long> _noteStartTimes = new();

        public void StartListening(int midiInDevice)
        {
            if (_midiIn != null)
            {
                throw new InvalidOperationException("Already listening to MIDI input.");
            }

            _midiIn = new MidiIn(midiInDevice);
            _midiIn.MessageReceived += MidiIn_MessageReceived;
            _midiIn.ErrorReceived += (s, e) => Debug.WriteLine($"MIDI Error: {e}");

            _midiIn.Start();
            Debug.WriteLine($"Listening on {MidiIn.DeviceInfo(midiInDevice).ProductName}...");
        }

        private void MidiIn_MessageReceived(object? sender, MidiInMessageEventArgs e)
        {
            if (e.MidiEvent != null && e.MidiEvent.CommandCode == MidiCommandCode.AutoSensing)
            {
                return;
            }
            if (e.MidiEvent is NoteOnEvent noteOn && noteOn.Velocity > 0)
            {
                long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var midiEvent = CreateMidiEvent(noteOn.NoteName, noteOn.NoteNumber, noteOn.Velocity, timestamp);

                OnNoteOnReceived?.Invoke(midiEvent);
                _noteStartTimes[noteOn.NoteNumber] = timestamp;
            }
            else if(e.MidiEvent is NoteEvent noteEvent && (noteEvent.CommandCode == MidiCommandCode.NoteOff ||
             (noteEvent.CommandCode == MidiCommandCode.NoteOn && noteEvent.Velocity == 0)))
            {
                if (_noteStartTimes.ContainsKey(noteEvent.NoteNumber))
                {
                    long startTime = _noteStartTimes[noteEvent.NoteNumber];
                    long endTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    double duration = (endTime - startTime) / 1000.0;

                    var midiEvent = CreateMidiEvent(noteEvent.NoteName,noteEvent.NoteNumber, noteEvent.Velocity, startTime, duration);

                    _noteStartTimes.Remove(noteEvent.NoteNumber);
                    OnMidiEventReceived?.Invoke(midiEvent);
                }
            }           
        }

        private MidiEventData CreateMidiEvent(string noteName, int noteNumber, int velocity, long timestamp, double duration = 0)
        {
            return new MidiEventData
            {
                Timestamp = timestamp,
                Note = noteName,
                NoteNumber = noteNumber,
                Velocity = velocity,
                Duration = duration,
                Source = "Device"
            };
        }

        public void StopListening()
        {
            if (_midiIn != null)
            {
                _midiIn.Stop();
                _midiIn.MessageReceived -= MidiIn_MessageReceived;
                _midiIn.Dispose();
                _midiIn = null;
            }
        }        
    }
}
