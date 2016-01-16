using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Castle.Core.Internal;
using RandomTextList.Code;
using RandomTextList.Models;

namespace RandomTextList.Controllers
{
    
    [RoutePrefix("api")]
    public class RecordsController : ApiController
    {
        private readonly DbContext _dbContext;
        private readonly DatabaseWriter<Record> _databaseWriter;

        public RecordsController(DbContext dbContext, DatabaseWriter<Record> databaseWriter)
        {
            _dbContext = dbContext;
            _databaseWriter = databaseWriter;
        }

        [Route("get_records")]
        public async Task<IHttpActionResult> GetRecords(int start, int count)
        {
            if (start < 0 || count <= 0)
            {
                Ok("error");
            }
            
            return Ok(_dbContext.Set<Record>().OrderBy(x => x.Id).Skip(0).Take(count).ToArray());
        }

        [Route("cleanup")]
        [HttpGet]
        public async Task<IHttpActionResult> CleanUp()
        {
            await _dbContext.Database.ExecuteSqlCommandAsync("delete from Record");
            _databaseWriter.RecordsCount = 0;
            return Ok();
        }

        [Route("start_writing")]
        [HttpGet]
        public IHttpActionResult StartWriting()
        {
            _databaseWriter.StartDataGeneration();
            return Ok(_databaseWriter.WriterStatus);
        }

        [Route("status")]
        [HttpGet]
        public IHttpActionResult GetWriterStatus()
        {
            return Ok(_databaseWriter.WriterStatus);
        }

        [Route("stop_writing")]
        [HttpGet]
        public IHttpActionResult StopWriting()
        {
            _databaseWriter.StopDataGeneration();
            return Ok(_databaseWriter.WriterStatus);
        }
        
    }


}