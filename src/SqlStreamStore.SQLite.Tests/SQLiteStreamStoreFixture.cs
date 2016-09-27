namespace SqlStreamStore.SQLite.Tests
{
    using System;
    using System.Threading.Tasks;

    public class SQLiteStreamStoreFixture
    {
        public string DataSource;
        public string Schema;

        public SQLiteStreamStoreFixture(string dataSource, string schema = "Fixture")
        {
            DataSource = dataSource;
            Schema = schema;
        }

        public async Task<SQLiteStreamStore> ThatIsInitialized()
        {
            SQLiteStreamStore store = new SQLiteStreamStore(
                new SQLiteStreamStoreSettings($"Data Source={DataSource};Version=3;"));
            await store.InitializeStore();

            return store;
        }

        public static string GetUniqueStoreName()
        {
            var uuid = Guid.NewGuid().ToString().Replace("-", string.Empty);
            return $"SQLiteTestStore-{uuid}.db";
        }
    }
}
