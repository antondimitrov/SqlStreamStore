namespace SqlStreamStore.SQLite
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EnsureThat;
    using SqlStreamStore.Infrastructure;
    using SqlStreamStore.Subscriptions;

    public class SQLiteStreamStoreSettings
    {
        private string _schema = "dbo";
        public string ConnectionString;
        private readonly Dictionary<string, string> _connection;
        public string DataSource => Get("Data Source");
        public string Version => Get("Version");
        public string PageSize => Get("Page Size");
        public bool BinaryGuid => false;
        public CreateStreamStoreNotifier CreateStreamStoreNotifier { get; set; } =
           PollingStreamStoreNotifier.CreateStreamStoreNotifier();
        public GetUtcNow GetUtcNow { get; set; }
        public TimeSpan MetadataMaxAgeCacheExpire { get; set; } = TimeSpan.FromMinutes(1);
        public int MetadataMaxAgeCacheMaxSize { get; set; } = 10000;
        public string LogName { get; set; } = "SQLiteStreamStore";

        public SQLiteStreamStoreSettings(string connectionString)
        {
            Ensure.That(connectionString, nameof(connectionString)).IsNotNullOrWhiteSpace();

            //todo add BinaryGUID=False as the default to the connection string to avoid GUIDS to be stored as binary
            ConnectionString = connectionString;
            _connection = ParseConnection(connectionString);
        }

        private Dictionary<string, string> ParseConnection(string connectionString)
        {
            return connectionString.Split(';')
                .Where(pair => pair.Contains('='))
                .Select(pair => pair.Split('='))
                .ToDictionary(pair => pair[0].Trim(), pair => pair[1].Trim());
        }
        public string Schema
        {
            get { return _schema; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    Ensure.That(value, nameof(Schema)).IsNotNullOrWhiteSpace();
                }
                _schema = value;
            }
        }

        public string Get(string key)
        {
            return _connection.ContainsKey(key)
                ? _connection[key]
                : "";
        }

        
    }
}
