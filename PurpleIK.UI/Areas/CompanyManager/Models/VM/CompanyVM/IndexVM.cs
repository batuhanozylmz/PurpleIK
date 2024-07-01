using PurpleIK.Core.Enums;
using PurpleIK.Entities;

namespace PurpleIK.UI.Areas.CompanyManager.Models.VM.CompanyVM
{
    public class IndexVM
    {
        public List<IndexItem> IndexItems { get; set; }
        public List<Person> Persons { get; set; }
        public List<Shift> Shifts { get; set; }
        public List<PersonPermission> OtherEmployeesPermissions { get; set; }
    }

    public class IndexItem
    {
        public Guid Id { get; set; }
        public string Picture { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? CompanyName { get; set; }
        public string? Role { get; set; }
        public string Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? BirhtPlace { get; set; }
        public string? CitizenId { get; set; }
        public DateTime? StartDate { get; set; }
        public string? PersonalEmail { get; set; }
        public string? Address { get; set; }
        public string? PersonalPhoneNumber { get; set; }
        public string? Department { get; set; }
        public string? Status { get; set; }
    }
}
