﻿using EasyCashIdentityProject.EntityLayer.Concrete;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCashIdentityProject.DataAccessLayer.Concrete
{
    public class Context : IdentityDbContext<AppUser, AppRole, int>
	{
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("server=DESKTOP-J9H0RUJ\\SQLEXPRESS;initial catalog=EasyCashDb;integrated Security=true");
        }
        public DbSet<CustomerAccount> CustomerAccounts { get; set; }
        public DbSet<CustomerAccountProcess> CustomerAccountProcesses { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<CustomerAccountProcess>()
                .HasOne(x => x.SenderCustomer)
                .WithMany(y => y.CustomerSender)
                .HasForeignKey(x => x.SenderID)
                .OnDelete(DeleteBehavior.ClientSetNull);
            
            builder.Entity<CustomerAccountProcess>()
                .HasOne(x => x.ReceiverCustomer)
                .WithMany(y => y.CustomerReceiver)
                .HasForeignKey(x => x.ReceiverID    )
                .OnDelete(DeleteBehavior.ClientSetNull);
            base.OnModelCreating(builder);
        }


    }
}
