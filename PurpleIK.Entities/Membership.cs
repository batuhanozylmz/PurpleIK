using PurpleIK.Core.Entity.Abstract;
using PurpleIK.Core.Entity.Contract;
namespace PurpleIK.Entities;

public class Membership : BaseEntity<Guid>, IEntity<Guid>
{
    public string Name { get; set; }
    public decimal? Price { get; set; }
    public string? SubscriptionPeriod { get; set; }
    public string? Description { get; set; }
    public int? NumberOfEmployee { get; set; }   
    //Nav
    public virtual ICollection<CompanyMembership> CompanyMemberships { get; set; }

}
