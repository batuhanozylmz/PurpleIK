using PurpleIK.Core.Enums;

namespace PurpleIK.UI.Areas.CompanyManager.Models.VM.EmployeeManagerVM
{
    public class EmployeeEditVM
    {
        public Guid Id { get; set; }
        public string? CompanyName { get; set; }
        public Gender? Gender { get; set; }
        public IFormFile? Picture_ { get; set; }
        public string? PictureBase64 { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? BirthPlace { get; set; }
        public string? CitizenId { get; set; }
        public DateTime? StartDate { get; set; }
        public string? PersonalEmail { get; set; }
        public string? CompanyEmail { get; set; }
        public string? Address { get; set; }
        public string? PersonalPhoneNumber { get; set; }
        public Department? Department { get; set; }
        public string? SelectedRole { get; set; }
    }
}
