using StackExchange.Redis;
using System.Text.Json;
using Core.MIDIProcessing.Logging;
using Core.MIDIProcessing.Models;

namespace Infrastructure.Logging
{
    public class RedisLogger : IEventLogger
    {
        private ConnectionMultiplexer? _redis;
        private IDatabase? _db;
        private bool _isConnected = false;
        private readonly string _connectionString;

        public bool IsAvailable => _isConnected && _db != null;

        public RedisLogger(string connectionString = "localhost:6379")
        {
            _connectionString = connectionString;
            TryToConnect();
        }

        private void TryToConnect()
        {
            try
            {
                _redis = ConnectionMultiplexer.Connect(_connectionString);
                _db = _redis.GetDatabase();
                _isConnected = true;
            }
            catch (RedisConnectionException)
            {
                _isConnected = false;
            }
        }

        async public void Log(MidiEventData midiEvent)
        {
            if (!_isConnected || _db == null)
            {
                TryToConnect();
                if (!_isConnected || _db == null) return;
            }
            if (_db != null)
            {
                string json = JsonSerializer.Serialize(midiEvent);
                await _db.ListLeftPushAsync("midi_logs", json);
            }           
        }
    }
}

