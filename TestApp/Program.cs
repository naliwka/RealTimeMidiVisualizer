using System;
using Infrastructure.MIDIInput;
using Core.MIDIProcessing;
using NAudio.Midi;

namespace TestApp
{
    static class Program
    {
        static void Main()
        {
            Console.WriteLine("Available MIDI devices:");
            for (int i = 0; i < MidiIn.NumberOfDevices; i++)
            {
                Console.WriteLine($"{i}: {MidiIn.DeviceInfo(i).ProductName}");
            }

            Console.Write("Enter device index to listen: ");
            if (!int.TryParse(Console.ReadLine(), out int deviceIndex) || deviceIndex < 0 || deviceIndex >= MidiIn.NumberOfDevices)
            {
                Console.WriteLine("Invalid device index. Exiting...");
                return;
            }

            var midiListener = new MidiListener();
            midiListener.OnMidiEventReceived += midiEvent =>
            {
                Console.WriteLine($"MIDI Event: Note {midiEvent.Note}, Velocity {midiEvent.Velocity}, Duration {midiEvent.Duration}s");
            };

            midiListener.StartListening(deviceIndex);

            Console.WriteLine("Listening... Press ENTER to exit.");
            Console.ReadLine();

            midiListener.StopListening();
        }
    }
}

