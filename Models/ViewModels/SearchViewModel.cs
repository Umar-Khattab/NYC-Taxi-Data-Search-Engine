using System.ComponentModel.DataAnnotations;

namespace NycTaxiSearch.Models.ViewModels
{
    public class SearchViewModel
    {
        [Display(Name = "From Date")]
        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }

        [Display(Name = "To Date")]
        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        [Display(Name = "Min Passenger Count")]
        [Range(0, 9)]
        public int? MinPassengerCount { get; set; }

        [Display(Name = "Max Passenger Count")]
        [Range(0, 9)]
        public int? MaxPassengerCount { get; set; }

        [Display(Name = "Min Trip Distance (miles)")]
        [Range(0, 999)]
        public decimal? MinTripDistance { get; set; }

        [Display(Name = "Max Trip Distance (miles)")]
        [Range(0, 999)]
        public decimal? MaxTripDistance { get; set; }

        [Display(Name = "Min Fare Amount")]
        [Range(0, 99999)]
        public decimal? MinFareAmount { get; set; }

        [Display(Name = "Max Fare Amount")]
        [Range(0, 99999)]
        public decimal? MaxFareAmount { get; set; }

        [Display(Name = "Payment Type")]
        public string? PaymentType { get; set; }

        [Display(Name = "Pickup Location")]
        public string? PickupLocation { get; set; }

        [Display(Name = "Dropoff Location")]
        public string? DropoffLocation { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 25;
    }
}