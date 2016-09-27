namespace SqlStreamStore.SQLite.SQLiteScripts
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;

    internal class Scripts
    {
        internal readonly string Schema;
        private readonly ConcurrentDictionary<string, string> _scripts 
            = new ConcurrentDictionary<string, string>();

        internal Scripts(string schema)
        {
            Schema = schema;
        }

        internal string InitializeStore => GetScript(nameof(InitializeStore));
        internal string AppendStream => GetScript(nameof(AppendStream));

        private string GetScript(string name)
        {
            return _scripts.GetOrAdd(name,
                key =>
                {
                    using(Stream stream = typeof(Scripts)
                        .Assembly
                        .GetManifestResourceStream("SqlStreamStore.SQLite.SQLiteScripts." + key + ".sql"))
                    {
                        if(stream == null)
                        {
                            throw new Exception($"Embedded resource, SqlStreamStore.SQLite.SQLiteScripts.{key}.sql -  {name}, not found. BUG!");
                        }
                        using(StreamReader reader = new StreamReader(stream))
                        {
                            return reader
                                .ReadToEnd()
                                .Replace("dbo.", Schema + ".");
                        }
                    }
                });
        }
    }
}