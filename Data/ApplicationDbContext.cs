using System;
using System.Collections.Generic;
using System.Text;
using FocusHRMS.Areas.Identity.Data;
using FocusHRMS.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FocusHRMS.Data
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

            builder.Entity<PayslipMaster>()
                .HasIndex(u => new { u.Month, u.Year, u.EmployeeCode })
                .IsUnique();
        }

        public DbSet<PayslipMaster> PayslipMasters { get; set; }
        public DbSet<OfferLetter> OfferLetters { get; set; }
        public DbSet<AppoinmentLetter> AppoinmentLetters { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Nominee> Nominees { get; set; }
        public DbSet<MailLibrary> MailLibraries { get; set; }
        public DbSet<DocumentList> DocumentLists { get; set; }
        public DbSet<MyDocument> MyDocuments { get; set; }

    }
}
