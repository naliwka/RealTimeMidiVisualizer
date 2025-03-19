namespace Core.MIDIProcessing.Models
{
    public class MidiEventData
    {
        public long Timestamp { get; set; }
        public string? Note { get; set; }
        public int NoteNumber { get; set; }
        public int Velocity { get; set; }
        public double Duration { get; set; }
        public string? Source { get; set; }
    }
}
