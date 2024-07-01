using PurpleIK.Core.Entity.Abstract;
using PurpleIK.Core.Entity.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurpleIK.Entities
{
    public class CompanyMembership : BaseEntity<Guid>, IEntity<Guid>
    {
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public decimal? Price { get; set; }
        public int? Duration { get; set; }
        public string? SubscriptionPeriod { get; set; }        
        public int? NumberOfEmployee { get; set; }
        public DateTime? ApplicationDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        //Nav
        public virtual Company? Companies { get; set; }
        public virtual Membership? Memberships { get; set; }
        public Guid? MembershipId { get; set; }
        public Guid? CompanyId { get; set; }
    }
}
