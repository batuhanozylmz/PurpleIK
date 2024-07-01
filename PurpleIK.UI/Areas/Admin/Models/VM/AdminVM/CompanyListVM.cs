using PurpleIK.Core.Enums;

namespace PurpleIK.UI.Areas.Admin.Models.VM.AdminVM
{
    public class CompanyListVM
    {
        public Guid CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public string? Email { get; set; }
        public int? NumberOfEmployee { get; set; }
        public Status Status { get; set; }
        public string StatusText { get; set; }
        public Guid PlanId { get; set; }
    }
}
