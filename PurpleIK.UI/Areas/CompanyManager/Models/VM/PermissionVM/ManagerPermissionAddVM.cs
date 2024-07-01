using PurpleIK.Entities;
using PurpleIK.UI.Areas.Employee.Models.VM.EmployeePermision;

namespace PurpleIK.UI.Areas.CompanyManager.Models.VM.PermissionVM
{
    public class ManagerPermissionAddVM
    {

        public List<Permission>? Permissions { get; set; }
        public ManagerPermissionAddItem ManagerPermissionAddItem { get; set; }
        public List<Person>? Persons { get; set; }
    }
    public class ManagerPermissionAddItem
    {
        public Guid Id { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? NumberOfDays { get; set; }
        public IFormFile? PermissionFile { get; set; }
        public byte[]? PermissionFile_ { get; set; }
        public Guid? PermissionId { get; set; }
        public Guid? PersonId { get; set; }
        public string? Status { get; set; }
    }
}
