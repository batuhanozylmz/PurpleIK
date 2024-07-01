using PurpleIK.Core.Enums;

namespace PurpleIK.UI.Areas.Admin.Models.VM.MembershipVM
{
    public class MembershipIndexVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }  
        public int NumberOfEmployee { get; set; }
        public string? Description { get; set; }
        public string? SubscriptionPeriod { get; set; }
        public Status Status { get; set; }

    }
}
