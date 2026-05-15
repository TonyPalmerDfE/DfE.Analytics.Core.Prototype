using DfE.Analytics.Core.Abstractions;
using DfE.Analytics.Core.Consumer.Mvc.Analytics;
using DfE.Analytics.Core.Events;
using Microsoft.AspNetCore.Mvc;

namespace DfE.Analytics.Core.Consumer.Mvc.Controllers
{

    public class SchoolsController : Controller
    {
        private readonly IAnalyticsTracker _tracker;

        public SchoolsController(IAnalyticsTracker tracker)
        {
            _tracker = tracker;
        }

        // -----------------------------
        // 1. SEARCH
        // -----------------------------
        [HttpGet]
        public IActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Search(string searchTerm)
        {
            // Dummy results
            var results = DummySchools
                .Where(s => s.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();

            await _tracker.TrackAsync(
                new AnalyticsEvent("school_search_performed",
                    new SchoolSearchData(searchTerm, results.Count)
                )
            );

            return View("SearchResults", results);
        }

        // -----------------------------
        // 2. FILTER
        // -----------------------------
        [HttpPost]
        public async Task<IActionResult> ApplyFilter(string filterName, string selectedValue)
        {
            // Dummy filter logic
            var results = DummySchools
                .Where(s => s.Phase == selectedValue)
                .ToList();

            await _tracker.TrackAsync(
                new AnalyticsEvent("school_filter_performed",
                    new FilterAppliedData(filterName, selectedValue, results.Count)
                )
            );

            return View("SearchResults", results);
        }

        // -----------------------------
        // 3. DETAILS
        // -----------------------------
        public async Task<IActionResult> Details(string urn)
        {
            var school = DummySchools.FirstOrDefault(s => s.Urn == urn);

            if (school == null)
                return NotFound();

            await _tracker.TrackAsync(
                new AnalyticsEvent("school_details_viewed",
                    new SchoolDetailsViewedData(school.Urn, school.Name, school.Phase)
                )
                .WithMetadata("journey", "parent_discovery")
            );

            return View(school);
        }

        // -----------------------------
        // 4. EXTERNAL WEBSITE CLICK
        // -----------------------------
        public async Task<IActionResult> External(string urn)
        {
            var school = DummySchools.FirstOrDefault(s => s.Urn == urn);

            if (school == null)
                return NotFound();

            var url = $"https://www.google.com/search?q={school.Name.Replace(" ", "+")}";

            await _tracker.TrackAsync(
                new AnalyticsEvent("external_website_clicked",
                    new ExternalWebsiteClickedData(school.Urn, url)
                )
            );

            return Redirect(url);
        }

        // -----------------------------
        // Dummy data
        // -----------------------------
        private static readonly List<SchoolVm> DummySchools = new()
    {
        new SchoolVm("100001", "St Mary's Primary", "Primary"),
        new SchoolVm("100002", "Greenfield Academy", "Secondary"),
        new SchoolVm("100003", "Riverside School", "Primary"),
        new SchoolVm("100004", "Hilltop High", "Secondary")
    };
    }

    public record SchoolVm(string Urn, string Name, string Phase);
}
