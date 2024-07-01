namespace PurpleIK.UI.Areas.CompanyManager.Models.VM.CompanyMembershipVM
{
    public class MembershipVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int? NumberOfEmployees { get; set; }
        public string? SubscriptionPeriod { get; set; }
        public int? Duration { get; set; }
        public Guid PlanId { get; set; }
    }
}
