using PurpleIK.Core.Enums;

namespace PurpleIK.UI.Areas.Admin.Models.VM.AdminVM
{
    public class CompanyEditVM
    {
        public Guid Id { get; set; }
        public string? CompanyName { get; set; }
        public string? MersisNo { get; set; }
        public string? TaxNo { get; set; }
        public IFormFile? _Logo { get; set; }
        public byte[]? Logo_ { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? CompanyEmail { get; set; }       
        public string? CompanyTypes { get; set; }
        public int? NumberOfEmployees { get; set; }
        public string? Status { get; set; }
    }
}
