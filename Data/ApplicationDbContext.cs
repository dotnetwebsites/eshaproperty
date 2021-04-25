using System;
using System.Collections.Generic;
using System.Text;
using EeshaProperty.Areas.Identity.Data;
using EeshaProperty.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EeshaProperty.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Enquiry>()
                .HasIndex(u => new { u.ApplicationId })
                .IsUnique();
        }

        public DbSet<MailLibrary> MailLibraries { get; set; }
        public DbSet<Enquiry> Enquiries { get; set; }
        public DbSet<DocumentList> DocumentLists { get; set; }
        public DbSet<MyDocument> MyDocuments { get; set; }

        public DbSet<Country> Countries { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<City> Cities { get; set; }
    }
}
