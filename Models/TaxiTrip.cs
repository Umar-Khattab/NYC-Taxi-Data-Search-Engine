using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NycTaxiSearch.Models
{
    public class TaxiTrip
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime PickupDateTime { get; set; }

        [Required]
        public DateTime DropoffDateTime { get; set; }

        [Required]
        [Range(0, 9)]
        public int PassengerCount { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TripDistance { get; set; }

        [MaxLength(100)]
        public string? PickupLocation { get; set; }

        [MaxLength(100)]
        public string? DropoffLocation { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal FareAmount { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TipAmount { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TollsAmount { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        [MaxLength(50)]
        public string? PaymentType { get; set; }

        public DateTime ImportedAt { get; set; } = DateTime.UtcNow;

        // Computed property
        [NotMapped]
        public TimeSpan TripDuration => DropoffDateTime - PickupDateTime;
    }
}
