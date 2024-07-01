using PurpleIK.Core.Enums;

namespace PurpleIK.UI.Models.VM.AccountVM
{
    public class CompanyRegisterVM
    {
        public string? CompanyName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? CompanyEmail { get; set; }
        public string? SuperManagerFirstName { get; set; }
        public string? SuperManagerLastName { get; set; }
        public string? SuperManagerEmail { get; set; }
        public string? SuperManagerPassword { get; set; }
        public int? NumberOfEmployees { get; set; }
        public Guid PlanId { get; set; }
        public Status Status { get; set; } = Status.DeActive;
    }
}
