namespace WebApplication7.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Data.Entity.ModelConfiguration.Conventions;

    public class Model1 : DbContext
    {
        public Model1() : base("DefaultConnection")
        {
        }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<Donor> Donors { get; set; }
        public DbSet<Proprietor> Proprietor { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Website> Website { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
