using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using RandomTextList.Models;

namespace RandomTextList.DAL
{
    public class RandomRecordsContext : DbContext
    {

        public DbSet<Record> RandomRecords { get; set; } 

        public RandomRecordsContext() : base("RandomRecordsContext")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}