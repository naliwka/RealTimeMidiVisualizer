using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.MIDIProcessing
{
    public class MidiEventData
    {
        public long Timestamp { get; set; }
        public string? Note { get; set; }
        public int Velocity { get; set; }   
        public double Duration { get; set; }
        public string? Source { get; set; }   

    }
}
