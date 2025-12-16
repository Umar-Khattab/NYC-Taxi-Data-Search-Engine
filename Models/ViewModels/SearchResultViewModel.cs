using NYC_Taxi_Data_Search_Engine;
using System.Collections.Generic;

namespace NycTaxiSearch.Models.ViewModels
{
    public class SearchResultViewModel
    {
        public List<TaxiTrip> Trips { get; set; } = new();
        public int TotalRecords { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalRecords / (double)PageSize);
        public SearchViewModel SearchCriteria { get; set; } = new();
        public bool FromCache { get; set; }
    }
}
