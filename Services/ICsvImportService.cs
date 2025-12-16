using NycTaxiSearch.Models;

namespace NycTaxiSearch.Services
{
    public interface ICsvImportService
    {
        Task<(bool, int, string)> ImportCsvAsync(
            Stream fileStream, string fileName);
    }
}