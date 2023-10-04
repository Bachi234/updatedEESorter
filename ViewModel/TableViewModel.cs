using automationTest.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace automationTest.ViewModel
{
    public class TableViewModel
    {
        public List<tblElasticData> TblElasticDatas { get; set; }
        public List<tblEvent> TblEvents { get; set; }
    }
}
