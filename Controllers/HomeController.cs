using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Data;
using automationTest.Models;
using automationTest.Service;

namespace automationTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ElasticDataService _tblElasticData;
        private readonly EventDataService _tblEventData;
        private readonly ILogger _logger;

        public HomeController(ElasticDataService elasticDataServices, EventDataService eventDataServices, ILogger<HomeController> logger)
        {
            _tblElasticData = elasticDataServices;
            _tblEventData = eventDataServices;
            _logger = logger;
        }
        public IActionResult Index(string searchMailNumbers)
        {
            ViewBag.SearchMailNumbers = searchMailNumbers;
            List<tblEvent> events = new List<tblEvent>();

            if (!string.IsNullOrWhiteSpace(searchMailNumbers))
            {
                // Split the input by whitespace to get individual mail numbers
                string[] mailNumbers = searchMailNumbers.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var mailNumber in mailNumbers)
                {
                    List<tblEvent> eventsForMailNumber = _tblEventData.GetMailNumber(mailNumber.Trim());
                    events.AddRange(eventsForMailNumber);
                }
            }
            return View(events);
        }

        [HttpGet]
        public IActionResult DisplayElasticData(string searchMailNumbers, string searchSubject, DateTime? startDate, DateTime? endDate)
        {
            ViewBag.SearchSubject = searchSubject;
            ViewBag.SearchMailNumber = searchMailNumbers;
            ViewBag.SearchSubjectDateStart = startDate;
            ViewBag.SearchSubjectDateEnd = endDate;
            if (string.IsNullOrEmpty(searchSubject))
            {
                ModelState.AddModelError("searchSubject", "Please enter a valid search subject.");
            }
            // Fetch the data based on your search criteria
            if (startDate != null && endDate != null)
            {
                List<tblElasticData> elasticData = _tblElasticData.GetElasticDataByDate(startDate, endDate);
                // Now, you can perform detailed aggregation on the data using LINQ
                var aggregatedData = elasticData
                    .GroupBy(data => new { data.To, data.From, data.Subject, data.EventType, data.EventDate, data.Channel, data.MessageCategory })
                    .Select(group => new tblElasticData
                    {
                        To = group.Key.To,
                        From = group.Key.From,
                        Subject = group.Key.Subject,
                        EventType = group.Key.EventType,
                        EventDate = group.Key.EventDate,
                        Channel = group.Key.Channel,
                        MessageCategory = group.Key.MessageCategory,
                        Quantity = group.Count()
                    })
                    .ToList();
                return View(aggregatedData);
            }

            if (!ModelState.IsValid)
            {
                return View(); // Return the view with error messages
            }

            else
            {
                List<tblElasticData> elasticData = _tblElasticData.GetElasticDataBySubject(searchSubject);
                if (!string.IsNullOrEmpty(searchSubject))
                {
                    List<tblElasticData> mailNumberData = _tblElasticData.GetElasticDataBySubject(searchMailNumbers);
                    elasticData = mailNumberData.Any() ? mailNumberData : elasticData;
                }
                return View (elasticData);
            }
    
        }

        [HttpPost]
        public IActionResult DisplayElasticData(DateTime? startDate, DateTime? endDate)
        {
            List<tblElasticData> elasticData = _tblElasticData.GetElasticDataByDate(startDate, endDate);
          
            if (startDate != null && endDate != null)
            {
                elasticData = elasticData.Where(data => data.EventDate.Date >= startDate && data.EventDate.Date <= endDate).ToList();
            }
          
            ViewBag.SearchSubject = null; // Clear search subject when filtering by dates
            ViewBag.SearchSubjectDateStart = startDate;
            ViewBag.SearchSubjectDateEnd = endDate;
    
            return View(elasticData);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}   
