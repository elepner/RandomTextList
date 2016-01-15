using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RandomTextList.Models;

namespace RandomTextList.DAL
{
    public class RandomRecordsInitializer : System.Data.Entity. DropCreateDatabaseIfModelChanges<RandomRecordsContext>
    {
        protected override void Seed(RandomRecordsContext context)
        {
            context.SaveChanges();
        }
    }
}