using System;
using System.IO;
using System.Text.Json;

namespace RPG_GAME.App.Configuration
{
    public static class ConfigLoader
    {
        public static GameConfig Load(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Configuration path must be provided.", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Configuration file not found: {filePath}", filePath);

            var json = File.ReadAllText(filePath);
            var config = JsonSerializer.Deserialize<GameConfig>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (config == null)
                throw new InvalidOperationException("Configuration file is empty or invalid.");

            if (string.IsNullOrWhiteSpace(config.PlayerName))
                throw new InvalidOperationException("Configuration field 'playerName' cannot be empty.");

            if (string.IsNullOrWhiteSpace(config.LogDirectory))
                throw new InvalidOperationException("Configuration field 'logDirectory' cannot be empty.");

            return config;
        }
    }
}