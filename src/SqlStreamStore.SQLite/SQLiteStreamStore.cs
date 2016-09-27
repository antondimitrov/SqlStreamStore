using System;
using System.Threading.Tasks;

namespace SqlStreamStore.SQLite
{
    using System.Data.SQLite;
    using System.IO;
    using System.Threading;
    using EnsureThat;
    
    using SqlStreamStore.Infrastructure;
    using SqlStreamStore.SQLite.SQLiteScripts;
    using SqlStreamStore.Streams;
    using SqlStreamStore.Subscriptions;

    public sealed class SQLiteStreamStore : StreamStoreBase
    {

        private readonly Func<SQLiteConnection> _createConnection;
        private readonly AsyncLazy<IStreamStoreNotifier> _streamStoreNotifier;
        private readonly Scripts _scripts;
        private readonly string _dataStore;

        public SQLiteStreamStore(SQLiteStreamStoreSettings settings)
            : base(
                settings.MetadataMaxAgeCacheExpire,
                settings.MetadataMaxAgeCacheMaxSize,
                settings.GetUtcNow,
                settings.LogName)
        {
            Ensure.That(settings, nameof(settings)).IsNotNull();

            _createConnection = () => new SQLiteConnection(settings.ConnectionString);
            _streamStoreNotifier = new AsyncLazy<IStreamStoreNotifier>(
                async () =>
                    {
                         if (settings.CreateStreamStoreNotifier == null)
                         {
                             throw new InvalidOperationException(
                                 "Cannot create notifier because supplied createStreamStoreNotifier was null");
                         }
                         return await settings.CreateStreamStoreNotifier(this).NotOnCapturedContext();
                     });
            _scripts = new Scripts(settings.Schema);
            _dataStore = settings.DataSource;
        }

        public async Task InitializeStore(bool ignoreErrors = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckIfDisposed();

            if (!File.Exists(_dataStore))
            {
                SQLiteConnection.CreateFile(_dataStore);
            }

            using (var connection = _createConnection())
            {
                await connection.OpenAsync(cancellationToken).NotOnCapturedContext();

                using(var command = new SQLiteCommand(_scripts.InitializeStore, connection))
                {
                    if (ignoreErrors)
                    {
                        await ExecuteAndIgnoreErrors(() => command.ExecuteNonQueryAsync(cancellationToken))
                            .NotOnCapturedContext();
                    }
                    else
                    {
                        await command.ExecuteNonQueryAsync(cancellationToken)
                            .NotOnCapturedContext();
                    }
                }
            }
            

        }

        protected override Task<AllMessagesPage> ReadAllForwardsInternal(long fromPositionExlusive, int maxCount, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override Task<AllMessagesPage> ReadAllBackwardsInternal(long fromPositionExclusive, int maxCount, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override Task<StreamMessagesPage> ReadStreamForwardsInternal(string streamId, int start, int count, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override Task<StreamMessagesPage> ReadStreamBackwardsInternal(
            string streamId,
            int fromVersionInclusive,
            int count,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override Task<long> ReadHeadPositionInternal(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override Task<IStreamSubscription> SubscribeToStreamInternal(
            string streamId,
            int startVersion,
            StreamMessageReceived streamMessageReceived,
            SubscriptionDropped subscriptionDropped,
            string name,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override Task<IAllStreamSubscription> SubscribeToAllInternal(
            long? fromPosition,
            StreamMessageReceived streamMessageReceived,
            SubscriptionDropped subscriptionDropped,
            string name,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override Task<StreamMetadataResult> GetStreamMetadataInternal(string streamId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Task<int> GetmessageCount(string streamId, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        protected override Task AppendToStreamInternal(
            string streamId,
            int expectedVersion,
            NewStreamMessage[] messages,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override Task DeleteStreamInternal(string streamId, int expectedVersion, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override Task DeleteEventInternal(string streamId, Guid eventId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override Task SetStreamMetadataInternal(
            string streamId,
            int expectedStreamMetadataVersion,
            int? maxAge,
            int? maxCount,
            string metadataJson,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_streamStoreNotifier.IsValueCreated)
                {
                    _streamStoreNotifier.Value.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private IObservable<Unit> GetStoreObservable => _streamStoreNotifier.Value.Result;

        private static async Task<T> ExecuteAndIgnoreErrors<T>(Func<Task<T>> operation)
        {
            try
            {
                return await operation().NotOnCapturedContext();
            }
            catch
            {
                return default(T);
            }
        }
    }
}
