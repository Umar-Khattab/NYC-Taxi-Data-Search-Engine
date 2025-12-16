using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace NycTaxiSearch.Models.ViewModels
{
    public class UploadViewModel
    {
        [Required(ErrorMessage = "Please select a CSV file")]
        [Display(Name = "CSV File")]
        public IFormFile? CsvFile { get; set; }

        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public int RecordsImported { get; set; }
    }
}