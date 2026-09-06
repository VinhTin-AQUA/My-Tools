using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
using QuickTools.Core.DTOs;
using QuickTools.Core.DTOs.Icons;
using QuickTools.Core.Models;
using QuickTools.Services.MongoDB;

namespace QuickTools.Services.Icons
{
    public interface IIconService
    {
        Task<IconModel?> GetByIdAsync(string id);
        Task<List<IconModel>> GetAllAsync();
        Task<List<IconModel>> SearchAsync(string keyword);
        Task<IconModel> CreateAsync(IconModel icon);
        Task<List<IconModel>> CreateManyAsync(List<IconModel> icons);
        Task<bool> UpdateAsync(string id, IconModel icon);
        Task<bool> DeleteAsync(string id);

        // 🔥 NEW: Search with Pagination
        Task<SearchIconResponse> SearchPaginationAsync(SearchIconRequest request);

        // 🔥 NEW: Search with Pagination và Projection (chỉ lấy các field cần thiết)
        Task<PaginationResult<T>> SearchPaginationProjectionAsync<T>(
            SearchIconRequest request,
            Expression<Func<IconModel, T>> projection);
    }

    public class IconService : IIconService
    {
        private readonly IMongoCollection<IconModel> _collection;

        public IconService(MongoDbContext context)
        {
            _collection = context.Icons;
        }

        public async Task<IconModel> CreateAsync(IconModel icon)
        {
            // Để MongoDB tự tạo Id
            icon.Id = string.Empty;
            icon.CreatedAt = DateTime.UtcNow;

            await _collection.InsertOneAsync(icon);
            return icon;
        }
        
        public async Task<List<IconModel>> CreateManyAsync(List<IconModel> icons)
        {
            if (icons == null || icons.Count == 0)
                return new List<IconModel>();

            // Chuẩn bị dữ liệu cho từng icon
            foreach (var icon in icons)
            {
                icon.Id = string.Empty; // Để MongoDB tự tạo Id
                icon.CreatedAt = DateTime.UtcNow;
                icon.UpdatedAt = DateTime.UtcNow;
            }

            // Thêm nhiều document cùng lúc
            await _collection.InsertManyAsync(icons);
        
            return icons;
        }

        public async Task<IconModel?> GetByIdAsync(string id)
        {
            var filter = Builders<IconModel>.Filter.Eq(x => x.Id, id);
            return await _collection
                .Find(filter)
                .FirstOrDefaultAsync();
        }

        public async Task<List<IconModel>> GetAllAsync()
        {
            return await _collection
                .Find(_ => true)
                .ToListAsync();
        }

        public async Task<List<IconModel>> SearchAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return await GetAllAsync();

            // Tìm kiếm không phân biệt hoa thường
            var filter = Builders<IconModel>.Filter.Regex(
                x => x.Name,
                new BsonRegularExpression(keyword, "i")
            );

            return await _collection
                .Find(filter)
                .ToListAsync();
        }

        public async Task<bool> UpdateAsync(string id, IconModel icon)
        {
            var update = Builders<IconModel>.Update
                .Set(x => x.Name, icon.Name)
                .Set(x => x.Url, icon.Url)
                .Set(x => x.UpdatedAt, DateTime.UtcNow); // Tự động cập nhật thời gian sửa

            var result = await _collection.UpdateOneAsync(
                x => x.Id == id,
                update
            );

            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _collection.DeleteOneAsync(x => x.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<SearchIconResponse> SearchPaginationAsync(SearchIconRequest request)
        {
            try
            {
                // Validate request
                if (request.Page < 1) request.Page = 1;
                if (request.PageSize < 1) request.PageSize = 10;
                if (request.PageSize > 100) request.PageSize = 100;

                // Build filter
                var filter = Builders<IconModel>.Filter.Empty;

                if (!string.IsNullOrWhiteSpace(request.Keyword))
                {
                    var regex = new BsonRegularExpression(request.Keyword, "i");
                    filter = Builders<IconModel>.Filter.Or(
                        Builders<IconModel>.Filter.Regex(x => x.Name, regex),
                        Builders<IconModel>.Filter.Regex(x => x.Url, regex)
                    );
                }

                // Count total
                var totalCount = await _collection.CountDocumentsAsync(filter);

                // Get items with pagination
                var items = await _collection
                    .Find(filter)
                    .Skip((request.Page - 1) * request.PageSize)
                    .Limit(request.PageSize)
                    .ToListAsync();

                // Tạo response với dữ liệu đã được clone để tránh lỗi context
                var response = new SearchIconResponse
                {
                    Items = items?.Select(item => new IconModel
                    {
                        Id = item.Id ?? string.Empty,
                        Name = item.Name ?? string.Empty,
                        Url = item.Url ?? string.Empty,
                        IconType = item.IconType,
                        CreatedAt = item.CreatedAt,
                        UpdatedAt = item.UpdatedAt
                    }).ToList() ?? new List<IconModel>(),

                    TotalCount = (int)totalCount,
                    Page = request.Page,
                    PageSize = request.PageSize
                };

                Console.WriteLine(
                    $"✅ SearchPaginationAsync success: {response.Items.Count} items, Total: {response.TotalCount}");
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ SearchPaginationAsync error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);

                // Trả về response rỗng thay vì throw exception
                return new SearchIconResponse
                {
                    Items = new List<IconModel>(),
                    TotalCount = 0,
                    Page = request.Page,
                    PageSize = request.PageSize
                };
            }
        }

        public async Task<PaginationResult<T>> SearchPaginationProjectionAsync<T>(
            SearchIconRequest request,
            Expression<Func<IconModel, T>> projection)
        {
            // 1. Tạo filter
            var filter = BuildFilter(request.Keyword);

            // 3. Đếm tổng số records
            var totalCount = await _collection.CountDocumentsAsync(filter);

            // 4. Lấy dữ liệu với Projection (chỉ lấy các field cần thiết)
            var items = await _collection
                .Find(filter)
                .Skip((request.Page - 1) * request.PageSize)
                .Limit(request.PageSize)
                .Project(projection)
                .ToListAsync();

            return new PaginationResult<T>
            {
                Items = items,
                TotalCount = (int)totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }

        // ========== HELPER METHODS ==========

        /// <summary>
        ///     Xây dựng filter cho tìm kiếm
        /// </summary>
        private FilterDefinition<IconModel> BuildFilter(string? keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return Builders<IconModel>.Filter.Empty;

            // Tìm kiếm trên nhiều field
            var regex = new BsonRegularExpression(keyword, "i");
            return Builders<IconModel>.Filter.Or(
                Builders<IconModel>.Filter.Regex(x => x.Name, regex),
                Builders<IconModel>.Filter.Regex(x => x.Url, regex)
            );
        }

        /// <summary>
        ///     Xây dựng sort
        /// </summary>
        private SortDefinition<IconModel> BuildSort(string sortBy, bool ascending)
        {
            var sortDefinition = sortBy.ToLower() switch
            {
                "name" => ascending
                    ? Builders<IconModel>.Sort.Ascending(x => x.Name)
                    : Builders<IconModel>.Sort.Descending(x => x.Name),
                "createdat" => ascending
                    ? Builders<IconModel>.Sort.Ascending(x => x.CreatedAt)
                    : Builders<IconModel>.Sort.Descending(x => x.CreatedAt),
                "updatedat" => ascending
                    ? Builders<IconModel>.Sort.Ascending(x => x.UpdatedAt)
                    : Builders<IconModel>.Sort.Descending(x => x.UpdatedAt),
                _ => ascending
                    ? Builders<IconModel>.Sort.Ascending(x => x.Name)
                    : Builders<IconModel>.Sort.Descending(x => x.Name)
            };

            return sortDefinition;
        }
    }
}