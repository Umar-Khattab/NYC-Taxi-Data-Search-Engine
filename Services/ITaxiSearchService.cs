using NycTaxiSearch.Models.ViewModels;

namespace NycTaxiSearch.Services
{
    public interface ITaxiSearchService
    {
        Task<SearchResultViewModel> SearchAsync(SearchViewModel criteria);
    }
}