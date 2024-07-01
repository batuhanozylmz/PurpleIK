using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PurpleIK.Entities;
using PurpleIK.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace PurpleIK.Context
{
    public class PurpleDbContext : IdentityDbContext<AppUser,AppRole,Guid>
    {
        public PurpleDbContext(DbContextOptions option) : base(option)
        {

        }
        //TODO: Db setler tanımlancak
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<PushMoney> PushMoneys { get; set; }
        public DbSet<PersonalInformation> PersonalInformations { get; set; }
        public DbSet<Debit> Debit { get; set; }
        public DbSet<PersonPermission> PersonPermissions { get; set; }
        public DbSet<PublicHolidays> PublicHolidays { get; set; }
        public DbSet<CompanyMembership> CompanyMemberships { get; set; }
        public DbSet<Shift> Shift { get; set; }
        public DbSet<Expense> Expense { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //TODO: Mapping cllasları eklenecek
            builder.ApplyConfiguration(new PushMoneyMapping());
            builder.ApplyConfiguration(new CommentMapping());
            builder.ApplyConfiguration(new CompanyMapping());
            builder.ApplyConfiguration(new PersonMapping());
            builder.ApplyConfiguration(new AppUserMapping());
            builder.ApplyConfiguration(new AppRoleMApping());
            builder.ApplyConfiguration(new PermissionMapping());
            builder.ApplyConfiguration(new MembershipMapping());
            builder.ApplyConfiguration(new DebitMapping());
            builder.ApplyConfiguration(new PersonalInformationMapping());
            builder.ApplyConfiguration(new PersonPermissionMapping());
            builder.ApplyConfiguration(new CompanyMembershipMapping());
            builder.ApplyConfiguration(new ShiftMapping());
            builder.ApplyConfiguration(new ExpenseMapping());
            base.OnModelCreating(builder);
        }
    }
}
