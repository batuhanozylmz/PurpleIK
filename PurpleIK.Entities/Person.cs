using PurpleIK.Core.Entity.Abstract;
using PurpleIK.Core.Entity.Contract;
using PurpleIK.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurpleIK.Entities
{
    public class Person : BaseEntity<Guid>, IEntity<Guid>
    {
        public Gender? Gender { get; set; }
        public byte[]? ProfilePhoto { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? BirhtPlace { get; set; }
        public string? CitizenId { get; set; }
        public DateTime? StartDate { get; set; }
        public string? PersonalEmail { get; set; }
        public string? CompanyEmail { get; set; }
        public string? Password { get; set; }
        public string? Address { get; set; }
        public string? PersonalPhoneNumber { get; set; }
        public Department? Department { get; set; }

        //NAV
        public Guid? CompanyId { get; set; }
        public virtual Company? Company { get; set; }

        public Guid? AppUserId { get; set; }
        public virtual AppUser? AppUser { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<PushMoney> PushMoneys { get; set; }
        public virtual ICollection<Debit> Debits { get; set; }
        public virtual ICollection<PersonalInformation> PersonalInformations { get; set; }
        public virtual ICollection<PersonPermission> PersonPermissions { get; set; }
        public virtual ICollection<Expense> Expenses { get; set; }
        public virtual ICollection<Shift> Shift { get; set; }


    }
}
