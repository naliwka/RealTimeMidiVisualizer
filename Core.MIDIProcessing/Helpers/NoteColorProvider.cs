namespace Core.MIDIProcessing.Helpers
{
    public class NoteColorProvider
    {
        private readonly Dictionary<string, string> _colorMap;

        public NoteColorProvider(Dictionary<string, string> customMap = null)
        {
            _colorMap = customMap ?? new Dictionary<string, string>
            {
                { "C", "#FF0000" },    // Red
                { "C#", "#FF4500" },  // OrangeRed
                { "D", "#FFA500" },   // Orange
                { "D#", "#FFD700" },  // Gold
                { "E", "#FFFF00" },   // Yellow
                { "F", "#ADFF2F" },   // GreenYellow
                { "F#", "#00FF00" },  // Lime
                { "G", "#00CED1" },   // DarkTurquoise
                { "G#", "#1E90FF" },  // DodgerBlue
                { "A", "#0000FF" },   // Blue
                { "A#", "#8A2BE2" },  // BlueViolet
                { "B", "#EE82EE" }    // Violet
            };
        }

        public string GetColorForNote(string? note)
        {
            if (string.IsNullOrEmpty(note)) return "#CCCCCC";

            string noteName = ExtractNoteName(note);

            if (_colorMap.TryGetValue(noteName, out var color))
                return color;

            return "#CCCCCC";
        }

        private string ExtractNoteName(string note)
        {
            if (note.Length >= 2 && note[1] == '#')
                return note.Substring(0, 2);
            else
                return note.Substring(0, 1);
        }
    }
}
