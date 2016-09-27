using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shouldly;
using System.Threading.Tasks;

namespace SqlStreamStore.SQLite.Tests
{
    using System.IO;
    using Xunit;


    public class SQLiteAcceptanceTests
    {
        private SQLiteStreamStoreFixture _aStore;
        private string _storeName;

        public SQLiteAcceptanceTests()
        {
            _storeName = SQLiteStreamStoreFixture.GetUniqueStoreName();
            _aStore = new SQLiteStreamStoreFixture(_storeName);
        }

        [Fact]
        public async Task Can_initialize_SQLite_store()
        {
            await _aStore.ThatIsInitialized();

            _aStore.DataSource.ShouldBe(_storeName);
            File.Exists(_aStore.DataSource).ShouldBeTrue();
        }
    }
}
