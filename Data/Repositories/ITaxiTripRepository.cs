using NycTaxiSearch.Models;
using NycTaxiSearch.Models.ViewModels;
using System.Collections.Generic;

namespace NycTaxiSearch.Data.Repositories
{
    public interface ITaxiTripRepository : IRepository<TaxiTrip>
    {
        Task<(List<TaxiTrip> trips, int totalCount)> SearchAsync(SearchViewModel criteria);
        Task BulkInsertAsync(IEnumerable<TaxiTrip> trips);
    }
}
