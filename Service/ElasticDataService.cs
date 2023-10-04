using automationTest.Context;
using automationTest.Models;
namespace automationTest.Service
{
    public class ElasticDataService
    {
        private readonly ApplicationDbContext _context;
        public ElasticDataService(ApplicationDbContext context)
        {
            _context = context;
        }
        private IQueryable<tblElasticData> ProjectElasticDataProperties(IQueryable<tblElasticData> dataQuery)
        {
            return dataQuery.Select(data => new tblElasticData
            {
                Subject = data.Subject,
                To = data.To,
                From = data.From,
                EventType = data.EventType,
                EventDate = data.EventDate,
                Channel = data.Channel,
                MessageCategory = data.MessageCategory
            });
        }
        public List<tblElasticData> GetElasticDataBySubject(string searchSubject)
        {
            if (searchSubject != null)
            {
                searchSubject = searchSubject.Replace(" ", ""); // Remove whitespace from the search subject

                return ProjectElasticDataProperties(_context.tblElasticData
                .Where(data => data.Subject != null && data.Subject.Replace(" ", "").Equals(searchSubject)))
                .ToList();

            }
            else
            {
                // Handle the case where searchSubject is null (optional)
                return new List<tblElasticData>();
            }
        }

        public List<tblElasticData> GetElasticDataByDate(DateTime? startDate, DateTime? endDate)
        {
            return ProjectElasticDataProperties(_context.tblElasticData
                .Where(data => data.EventDate.Date >= startDate && data.EventDate.Date <= endDate))
                .ToList();
        }

    }
}
