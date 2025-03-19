using StackExchange.Redis;
using System.Text.Json;
using Core.MIDIProcessing.Logging;
using Core.MIDIProcessing.Models;

namespace Infrastructure.Logging
{
    public class RedisLogger : IEventLogger
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _db;


        public RedisLogger(string connectionString = "localhost:6379")
        {
            _redis = ConnectionMultiplexer.Connect(connectionString);
            _db = _redis.GetDatabase();
        }

        public void Log(MidiEventData midiEvent)
        {
            string json = JsonSerializer.Serialize(midiEvent);
            _db.ListLeftPush("midi_logs", json);
        }
    }
}

