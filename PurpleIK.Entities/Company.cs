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
    public class Company : BaseEntity<Guid>, IEntity<Guid>
    {
        public string? CompanyName { get; set; }
        public string? MersisNo { get; set; }
        public string? TaxNo { get; set; }
        public byte[]? Logo { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? CompanyEmail { get; set; }
        public string? SuperManagerFirstName { get; set; }
        public string? SuperManagerLastName { get; set; }
        public string? SuperManagerEmail { get; set; }
        public string? SuperManagerPassword { get; set; }
        public CompanyTypes? CompanyTypes { get; set; }
        public int? NumberOfEmployees { get; set; }

        public virtual ICollection<Person> Persons { get; set; }
        public virtual CompanyMembership CompanyMemberships { get; set; }

    }
}
