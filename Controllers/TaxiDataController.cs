using Microsoft.AspNetCore.Mvc;
using NycTaxiSearch.Models.ViewModels;
using NycTaxiSearch.Services;

namespace NycTaxiSearch.Controllers
{
    public class TaxiDataController : Controller
    {
        private readonly ICsvImportService _importService;
        private readonly ITaxiSearchService _searchService;
        private readonly ILogger _logger;

        public TaxiDataController(
            ICsvImportService importService,
            ITaxiSearchService searchService,
            ILogger logger)
        {
            _importService = importService;
            _searchService = searchService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View(new UploadViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(UploadViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.CsvFile == null || model.CsvFile.Length == 0)
            {
                ModelState.AddModelError("CsvFile", "Please select a valid CSV file");
                return View(model);
            }

            if (!model.CsvFile.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("CsvFile", "Only CSV files are allowed");
                return View(model);
            }

            try
            {
                using var stream = model.CsvFile.OpenReadStream();
                var (success, recordsImported, message) = await _importService.ImportCsvAsync(
                    stream, model.CsvFile.FileName);

                model.IsSuccess = success;
                model.Message = message;
                model.RecordsImported = recordsImported;

                if (success)
                {
                    TempData["SuccessMessage"] = message;
                    return RedirectToAction(nameof(Search));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file");
                model.IsSuccess = false;
                model.Message = "An error occurred while uploading the file";
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Search()
        {
            return View(new SearchViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(SearchViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var results = await _searchService.SearchAsync(model);
                return View("Results", results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching taxi data");
                ModelState.AddModelError("", "An error occurred while searching. Please try again.");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Results(SearchViewModel model)
        {
            var results = await _searchService.SearchAsync(model);
            return View(results);
        }
    }
}