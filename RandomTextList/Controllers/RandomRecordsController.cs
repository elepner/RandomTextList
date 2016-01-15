using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using RandomTextList.Models;

namespace RandomTextList.Controllers
{
    
    public class RandomRecordsController : ApiController
    {
        private readonly DbContext _dbContext;

        public RandomRecordsController(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Route("get_records")]
        public Record[] GetRecords(int start, int count)
        {
            var record = _dbContext.Set<Record>().FirstOrDefault();
            return new[]{new Record {Text = "Random Text", Header = "Random Header"} };
        }

    }

}