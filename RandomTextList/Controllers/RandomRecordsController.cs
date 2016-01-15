using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using RandomTextList.Models;

namespace RandomTextList.Controllers
{
    
    public class RandomRecordsController : ApiController
    {
        [Route("get_records")]
        public Record[] GetRecords(int start, int count)
        {
            return new[]{new Record(){Text = "Random Text", Header = "Random Header"} };
        }
    }

}