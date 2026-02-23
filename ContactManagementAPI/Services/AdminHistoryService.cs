using System.Text.Json;
using ContactManagementAPI.Models;

namespace ContactManagementAPI.Services
{
    public class AdminHistoryService
    {
        private readonly string _historyFilePath;
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = false
        };

        public AdminHistoryService(IWebHostEnvironment environment)
        {
            var historyDirectory = Path.Combine(environment.ContentRootPath, "App_Data");
            Directory.CreateDirectory(historyDirectory);
            _historyFilePath = Path.Combine(historyDirectory, "admin-history.jsonl");
        }

        public void Log(string actionType, string entityType, int? entityId, string performedBy, string details)
        {
            var entry = new AdminHistoryEntry
            {
                ActionType = actionType,
                EntityType = entityType,
                EntityId = entityId,
                PerformedBy = string.IsNullOrWhiteSpace(performedBy) ? "Unknown" : performedBy,
                Details = details,
                PerformedAt = DateTime.Now
            };

            var line = JsonSerializer.Serialize(entry, JsonOptions);
            File.AppendAllText(_historyFilePath, line + Environment.NewLine);
        }

        public IReadOnlyList<AdminHistoryEntry> GetLatest(int take = 200)
        {
            if (!File.Exists(_historyFilePath))
            {
                return Array.Empty<AdminHistoryEntry>();
            }

            var entries = new List<AdminHistoryEntry>();
            var lines = File.ReadLines(_historyFilePath);
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                try
                {
                    var entry = JsonSerializer.Deserialize<AdminHistoryEntry>(line);
                    if (entry != null)
                    {
                        entries.Add(entry);
                    }
                }
                catch
                {
                    // Ignore malformed history lines and continue
                }
            }

            return entries
                .OrderByDescending(e => e.PerformedAt)
                .Take(take)
                .ToList();
        }
    }
}