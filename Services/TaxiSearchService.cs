using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NycTaxiSearch.Data.Repositories;
using NycTaxiSearch.Models;
using NycTaxiSearch.Models.ViewModels;

namespace NycTaxiSearch.Services
{
    public class TaxiSearchService : ITaxiSearchService
    {
        private readonly ITaxiTripRepository _repository;
        private readonly ICacheService<SearchResultViewModel> _cacheService;
        private readonly ILogger<TaxiSearchService> _logger;
        private static readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };

        public TaxiSearchService(
            ITaxiTripRepository repository,
            ICacheService<SearchResultViewModel> cacheService,
            ILogger<TaxiSearchService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<SearchResultViewModel> SearchAsync(SearchViewModel criteria)
        {
            if (criteria == null) throw new ArgumentNullException(nameof(criteria));
            if (criteria.Page <= 0) criteria.Page = 1;
            if (criteria.PageSize <= 0) criteria.PageSize = 25;

            var cacheKey = GenerateCacheKey(criteria);

            try
            {
                SearchResultViewModel cachedResult = await _cacheService.GetAsync(cacheKey);
                if (cachedResult != null)
                {
                    _logger.LogInformation("Returning cached search results for key: {Key}", cacheKey);
                    cachedResult.FromCache = true;
                    return cachedResult;
                }

                // Query database
                var (trips, totalCount) = await _repository.SearchAsync(criteria).ConfigureAwait(false);

                var totalPages = criteria.PageSize > 0
                    ? (int)Math.Ceiling((double)totalCount / criteria.PageSize)
                    : 0;

                var result = new SearchResultViewModel
                {
                    Trips = trips,
                    TotalRecords = totalCount,
                    Page = criteria.Page,
                    PageSize = criteria.PageSize,
                    SearchCriteria = criteria,
                    FromCache = false
                };

                // Cache the result (best-effort; log but don't fail the request on cache errors)
                try
                {
                    await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(15)).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to cache search results for key: {Key}", cacheKey);
                }

                _logger.LogInformation(
                    "Search completed. Found {Count} records (Page {Page} of {TotalPages})",
                    totalCount, criteria.Page, totalPages);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while executing search (Page: {Page}, PageSize: {PageSize})",
                    criteria.Page, criteria.PageSize);
                throw;
            }
        }

        private static string GenerateCacheKey(SearchViewModel criteria)
        {
            // Use stable JSON serialization and base64 to avoid invalid cache key characters
            var json = JsonSerializer.Serialize(criteria, _serializerOptions);
            var bytes = Encoding.UTF8.GetBytes(json);
            return $"search_{Convert.ToBase64String(bytes)}";
        }
    }
}