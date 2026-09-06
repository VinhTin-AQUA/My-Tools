using System.Collections.Concurrent;
using JsonFlatFileDataStore;


namespace QuickTools.Services.LocalStorages
{
    public interface IJsonStorage : IDisposable
    {
        // Collection operations
        IDocumentCollection<T> GetCollection<T>(string collectionName) where T : class;
        IDocumentCollection<dynamic> GetDynamicCollection(string collectionName);
        
        // Single item operations
        Task<T> GetItemAsync<T>(string key);
        Task<bool> InsertItemAsync<T>(string key, T item);
        Task<bool> ReplaceItemAsync<T>(string key, T item, bool upsert = false);
        Task<bool> UpdateItemAsync<T>(string key, object updates);
        Task<bool> DeleteItemAsync(string key);
        
        // Query helpers
        IEnumerable<T> QueryCollection<T>(string collectionName) where T : class;
        IEnumerable<dynamic> QueryDynamicCollection(string collectionName);
        
        // Full-text search
        IEnumerable<T> FullTextSearch<T>(string searchTerm, bool caseSensitive = false, string collectionName = "") where T : class;
        IEnumerable<dynamic> FullTextSearchDynamic(string collectionName, string searchTerm, bool caseSensitive = false);
        
        // Utility
        Task ReloadAsync();
        Task CommitAsync();
        
        // Get next id value
        object GetNextIdValue(string collectionName);
        T GetNextIdValue<T>(string collectionName);
        
        // Check if collection exists
        bool CollectionExists(string collectionName);
    }
    
    public class JsonStorage : IJsonStorage
    {
        private readonly DataStore _dataStore;
        private readonly string _filePath;
        private readonly bool _useLowerCamelCase;
        private readonly bool _minifyJson;
        private readonly string? _encryptionKey;
        private readonly bool _reloadBeforeGetCollection;
        private readonly string _keyProperty;
        
        // Cache collections để tránh tạo lại
        private readonly ConcurrentDictionary<string, object> _collectionCache;
        private readonly ConcurrentDictionary<string, object> _dynamicCollectionCache;
        
        private bool _disposed;

        public JsonStorage(
            string filePath = "data.json",
            bool useLowerCamelCase = true,
            bool minifyJson = false,
            string? encryptionKey = null,
            bool reloadBeforeGetCollection = false,
            string keyProperty = "Id")
        {
            _filePath = filePath;
            _useLowerCamelCase = useLowerCamelCase;
            _minifyJson = minifyJson;
            _encryptionKey = encryptionKey;
            _reloadBeforeGetCollection = reloadBeforeGetCollection;
            _keyProperty = keyProperty;
            
            _collectionCache = new ConcurrentDictionary<string, object>();
            _dynamicCollectionCache = new ConcurrentDictionary<string, object>();
            
            // Khởi tạo DataStore
            if (!string.IsNullOrEmpty(encryptionKey))
            {
                _dataStore = new DataStore(filePath, useLowerCamelCase, keyProperty,reloadBeforeGetCollection, encryptionKey, minifyJson);
            }
            else
            {
                _dataStore = new DataStore(filePath, useLowerCamelCase, keyProperty,reloadBeforeGetCollection, null, minifyJson);
            }
        }

        #region Collection Operations

        /// <summary>
        /// Lấy typed collection với caching
        /// </summary>
        public IDocumentCollection<T> GetCollection<T>(string collectionName) where T : class
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(JsonStorage));

            var name = collectionName ?? typeof(T).Name;
            var cacheKey = $"{name}_{typeof(T).FullName}";
            
            return (IDocumentCollection<T>)_collectionCache.GetOrAdd(cacheKey, _ =>
            {
                return _dataStore.GetCollection<T>(collectionName);
            });
        }

        /// <summary>
        /// Lấy dynamic collection với caching
        /// </summary>
        public IDocumentCollection<dynamic> GetDynamicCollection(string collectionName)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(JsonStorage));

            if (string.IsNullOrEmpty(collectionName))
                throw new ArgumentException("Collection name is required", nameof(collectionName));

            return (IDocumentCollection<dynamic>)_dynamicCollectionCache.GetOrAdd(collectionName, _ =>
            {
                return _dataStore.GetCollection(collectionName);
            });
        }

        #endregion

        #region Query Operations

        /// <summary>
        /// Query typed collection với LINQ
        /// </summary>
        public IEnumerable<T> QueryCollection<T>(string collectionName) where T : class
        {
            var collection = GetCollection<T>(collectionName);
            return collection.AsQueryable();
        }

        /// <summary>
        /// Query dynamic collection với LINQ
        /// </summary>
        public IEnumerable<dynamic> QueryDynamicCollection(string collectionName)
        {
            var collection = GetDynamicCollection(collectionName);
            return collection.AsQueryable();
        }

        #endregion

        #region Single Item Operations

        public async Task<T> GetItemAsync<T>(string key)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(JsonStorage));

            try
            {
                return _dataStore.GetItem<T>(key);
            }
            catch (KeyNotFoundException)
            {
                return default;
            }
        }

        public async Task<bool> InsertItemAsync<T>(string key, T item)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(JsonStorage));

            return await _dataStore.InsertItemAsync(key, item);
        }

        public async Task<bool> ReplaceItemAsync<T>(string key, T item, bool upsert = false)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(JsonStorage));

            return await _dataStore.ReplaceItemAsync(key, item, upsert);
        }

        public async Task<bool> UpdateItemAsync<T>(string key, object updates)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(JsonStorage));

            return await _dataStore.UpdateItemAsync(key, updates);
        }

        public async Task<bool> DeleteItemAsync(string key)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(JsonStorage));

            return await _dataStore.DeleteItemAsync(key);
        }

        #endregion

        #region Full-Text Search

        public IEnumerable<T> FullTextSearch<T>(string searchTerm, bool caseSensitive = false, string collectionName = null) where T : class
        {
            var collection = GetCollection<T>(collectionName);
            return collection.Find(searchTerm, caseSensitive).Cast<T>();
        }

        public IEnumerable<dynamic> FullTextSearchDynamic(string collectionName, string searchTerm, bool caseSensitive = false)
        {
            var collection = GetDynamicCollection(collectionName);
            return collection.Find(searchTerm, caseSensitive);
        }

        #endregion

        #region Utility Methods

        public async Task ReloadAsync()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(JsonStorage));

            await Task.Run(() => _dataStore.Reload());
            // Clear cache để reload từ file
            _collectionCache.Clear();
            _dynamicCollectionCache.Clear();
        }

        public async Task CommitAsync()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(JsonStorage));

            // DataStore tự động commit, nhưng có thể force nếu cần
            await Task.CompletedTask;
        }

        public object GetNextIdValue(string collectionName)
        {
            var collection = GetDynamicCollection(collectionName);
            return collection.GetNextIdValue();
        }

        public T GetNextIdValue<T>(string collectionName)
        {
            var value = GetNextIdValue(collectionName);
            return (T)Convert.ChangeType(value, typeof(T));
        }

        public bool CollectionExists(string collectionName)
        {
            // Kiểm tra collection tồn tại trong DataStore
            try
            {
                var collection = GetDynamicCollection(collectionName);
                return collection.AsQueryable().Any();
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region IDisposable Implementation

        public void Dispose()
        {
            if (!_disposed)
            {
                _dataStore.Dispose();
                _collectionCache.Clear();
                _dynamicCollectionCache.Clear();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}