using Microsoft.EntityFrameworkCore;
using NycTaxiSearch.Models;
using NycTaxiSearch.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;

namespace NycTaxiSearch.Data.Repositories
{
    public class TaxiTripRepository : Repository<TaxiTrip>, ITaxiTripRepository
    {
        public TaxiTripRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<(List<TaxiTrip> trips, int totalCount)> SearchAsync(SearchViewModel criteria)
        {
            var query = _dbSet.AsQueryable();

            // Apply filters
            if (criteria.FromDate.HasValue)
                query = query.Where(t => t.PickupDateTime >= criteria.FromDate.Value);

            if (criteria.ToDate.HasValue)
                query = query.Where(t => t.PickupDateTime <= criteria.ToDate.Value.AddDays(1).AddSeconds(-1));

            if (criteria.MinPassengerCount.HasValue)
                query = query.Where(t => t.PassengerCount >= criteria.MinPassengerCount.Value);

            if (criteria.MaxPassengerCount.HasValue)
                query = query.Where(t => t.PassengerCount <= criteria.MaxPassengerCount.Value);

            if (criteria.MinTripDistance.HasValue)
                query = query.Where(t => t.TripDistance >= criteria.MinTripDistance.Value);

            if (criteria.MaxTripDistance.HasValue)
                query = query.Where(t => t.TripDistance <= criteria.MaxTripDistance.Value);

            if (criteria.MinFareAmount.HasValue)
                query = query.Where(t => t.FareAmount >= criteria.MinFareAmount.Value);

            if (criteria.MaxFareAmount.HasValue)
                query = query.Where(t => t.FareAmount <= criteria.MaxFareAmount.Value);

            if (!string.IsNullOrWhiteSpace(criteria.PaymentType))
                query = query.Where(t => t.PaymentType == criteria.PaymentType);

            if (!string.IsNullOrWhiteSpace(criteria.PickupLocation))
                query = query.Where(t => t.PickupLocation != null &&
                    t.PickupLocation.Contains(criteria.PickupLocation));

            if (!string.IsNullOrWhiteSpace(criteria.DropoffLocation))
                query = query.Where(t => t.DropoffLocation != null &&
                    t.DropoffLocation.Contains(criteria.DropoffLocation));

            var totalCount = await query.CountAsync();

            // Apply pagination and ordering
            var trips = await query
                .OrderByDescending(t => t.PickupDateTime)
                .Skip((criteria.Page - 1) * criteria.PageSize)
                .Take(criteria.PageSize)
                .ToListAsync();

            return (trips, totalCount);
        }

        public async Task BulkInsertAsync(IEnumerable<TaxiTrip> trips)
        {
            await _dbSet.AddRangeAsync(trips);
            await _context.SaveChangesAsync();
        }
    }
}