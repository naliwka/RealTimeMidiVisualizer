using NAudio.Midi;

namespace Infrastructure.MIDIDevices
{
    public class MidiDeviceManager
    {
        public List<string> GetMidiDevices()
        {
            List<string> devices = new List<string>();
            for (int i = 0; i < MidiIn.NumberOfDevices; i++)
            {
                devices.Add(MidiIn.DeviceInfo(i).ProductName);
            }
            return devices;
        }
    }
}
