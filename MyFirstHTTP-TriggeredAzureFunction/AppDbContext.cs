using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;


namespace MyFirstHTTP_TriggeredAzureFunction
{

    public class AppDBContext : DbContext
    {
        // Create database name SalesData
        public DbSet<Sales> SalesData { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sales>().Property(e => e.Amount).HasPrecision(10, 2);
        }

    }


}