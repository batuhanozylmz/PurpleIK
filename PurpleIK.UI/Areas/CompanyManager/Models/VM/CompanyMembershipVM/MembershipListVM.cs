using PurpleIK.Core.Enums;

namespace PurpleIK.UI.Areas.CompanyManager.Models.VM.CompanyMembershipVM
{
    public class MembershipListVM
    {
        public Guid PlanId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int NumberOfEmployee { get; set; }        
        public string? SubscriptionPeriod { get; set; }        
    }
}
