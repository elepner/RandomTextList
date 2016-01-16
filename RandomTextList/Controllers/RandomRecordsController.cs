using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Castle.Core.Internal;
using RandomTextList.Models;

namespace RandomTextList.Controllers
{
    
    [RoutePrefix("api")]
    public class RandomRecordsController : ApiController
    {
        private readonly DbContext _dbContext;
        private static readonly Random Random = new Random();

        public RandomRecordsController(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Route("get_records")]
        public async Task<IHttpActionResult> GetRecords(int start, int count)
        {
            if (start < 0 || count <= 0)
            {
                Ok("error");
            }
            int recordCount = _dbContext.Set<Record>().Count();
            if (recordCount < start + count)
            {
                var results = Enumerable.Range(0, count - recordCount + start).Select(idx => new Record {Header = RandomString(Random.Next(4, 8)), Text = RandomString(Random.Next(50, 1000))});
                _dbContext.Set<Record>().AddRange(results);
                await _dbContext.SaveChangesAsync();
            }
            
            return Ok(_dbContext.Set<Record>().OrderBy(x => x.Id).Skip(0).Take(count).ToArray());
        }

        [Route("cleanup")]
        [HttpGet]
        public async Task<IHttpActionResult> CleanUp()
        {
            await _dbContext.Database.ExecuteSqlCommandAsync("delete from Record");
            return Ok();
        }
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789     ";
            
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[Random.Next(s.Length)]).ToArray());
        }
    }


}