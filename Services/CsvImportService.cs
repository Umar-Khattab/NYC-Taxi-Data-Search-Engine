using NycTaxiSearch.Data.Repositories;
using NycTaxiSearch.Models;
using System.Collections.Generic;
using System.Globalization;

namespace NycTaxiSearch.Services
{
    public class CsvImportService : ICsvImportService
    {
        private readonly ITaxiTripRepository _repository;
        private readonly ICacheService<TaxiTrip> _cacheService;
        private readonly ILogger _logger;
        private const int BatchSize = 1000;

        public CsvImportService(
            ITaxiTripRepository repository,
            ICacheService<TaxiTrip> cacheService,
            ILogger logger)
        {
            _repository = repository;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<(bool, int, string)> ImportCsvAsync(
            Stream fileStream, string fileName)
        {
            var recordsImported = 0;
            var trips = new List<TaxiTrip>();

            try
            {
                using var reader = new StreamReader(fileStream);

                // Read and validate header
                var headerLine = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(headerLine))
                {
                    return (false, 0, "CSV file is empty");
                }

                var headers = headerLine.Split(',').Select(h => h.Trim()).ToArray();

                // Read data rows
                string? line;
                var lineNumber = 1;

                while ((line = await reader.ReadLineAsync()) != null)
                {
                    lineNumber++;

                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    try
                    {
                        var trip = ParseCsvLine(line, headers);
                        if (trip != null)
                        {
                            trips.Add(trip);
                            recordsImported++;

                            // Batch insert for performance
                            if (trips.Count >= BatchSize)
                            {
                                await _repository.BulkInsertAsync(trips);
                                trips.Clear();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error parsing line {LineNumber}: {Line}",
                            lineNumber, line);
                    }
                }

                // Insert remaining records
                if (trips.Any())
                {
                    await _repository.BulkInsertAsync(trips);
                }

                // Clear search cache after import
                await _cacheService.ClearAsync("search_");

                _logger.LogInformation("Successfully imported {Count} records from {FileName}",
                    recordsImported, fileName);

                return (true, recordsImported,
                    $"Successfully imported {recordsImported} taxi trip records");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing CSV file: {FileName}", fileName);
                return (false, recordsImported,
                    $"Error importing file: {ex.Message}. {recordsImported} records were imported before the error.");
            }
        }

        private TaxiTrip? ParseCsvLine(string line, string[] headers)
        {
            var values = ParseCsvValues(line);

            if (values.Length < 10)
                return null;

            try
            {
                var trip = new TaxiTrip();

                for (int i = 0; i < Math.Min(headers.Length, values.Length); i++)
                {
                    var header = headers[i].ToLowerInvariant();
                    var value = values[i];

                    switch (header)
                    {
                        case "pickup_datetime":
                        case "pickupdatetime":
                        case "tpep_pickup_datetime":
                            trip.PickupDateTime = ParseDateTime(value);
                            break;
                        case "dropoff_datetime":
                        case "dropoffdatetime":
                        case "tpep_dropoff_datetime":
                            trip.DropoffDateTime = ParseDateTime(value);
                            break;
                        case "passenger_count":
                        case "passengercount":
                            trip.PassengerCount = ParseInt(value);
                            break;
                        case "trip_distance":
                        case "tripdistance":
                            trip.TripDistance = ParseDecimal(value);
                            break;
                        case "pickup_location":
                        case "pickuplocation":
                        case "pulocationid":
                            trip.PickupLocation = value;
                            break;
                        case "dropoff_location":
                        case "dropofflocation":
                        case "dolocationid":
                            trip.DropoffLocation = value;
                            break;
                        case "fare_amount":
                        case "fareamount":
                            trip.FareAmount = ParseDecimal(value);
                            break;
                        case "tip_amount":
                        case "tipamount":
                            trip.TipAmount = ParseDecimal(value);
                            break;
                        case "tolls_amount":
                        case "tollsamount":
                            trip.TollsAmount = ParseDecimal(value);
                            break;
                        case "total_amount":
                        case "totalamount":
                            trip.TotalAmount = ParseDecimal(value);
                            break;
                        case "payment_type":
                        case "paymenttype":
                            trip.PaymentType = value;
                            break;
                    }
                }

                // Validation
                if (trip.PickupDateTime == default || trip.DropoffDateTime == default)
                    return null;

                if (trip.DropoffDateTime <= trip.PickupDateTime)
                    return null;

                return trip;
            }
            catch
            {
                return null;
            }
        }

        private string[] ParseCsvValues(string line)
        {
            var values = new List<string>();
            var inQuotes = false;
            var currentValue = string.Empty;

            for (int i = 0; i < line.Length; i++)
            {
                var c = line[i];

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    values.Add(currentValue.Trim());
                    currentValue = string.Empty;
                }
                else
                {
                    currentValue += c;
                }
            }

            values.Add(currentValue.Trim());
            return values.ToArray();
        }

        private DateTime ParseDateTime(string value)
        {
            if (DateTime.TryParse(value, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var result))
                return result;
            return default;
        }

        private int ParseInt(string value)
        {
            if (int.TryParse(value, out var result))
                return result;
            return 0;
        }

        private decimal ParseDecimal(string value)
        {
            if (decimal.TryParse(value, NumberStyles.Any,
                CultureInfo.InvariantCulture, out var result))
                return result;
            return 0;
        }
    }
}