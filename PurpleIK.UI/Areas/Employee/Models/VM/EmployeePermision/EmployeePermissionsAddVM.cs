using PurpleIK.Entities;

namespace PurpleIK.UI.Areas.Employee.Models.VM.EmployeePermision
{
    public class EmployeePermissionsAddVM
    {
        public List<Permission>? Permissions { get; set; }
        public EmployeePermissionsAddItem EmployeePermissionsAddItem { get; set; }
    }
    public class EmployeePermissionsAddItem
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
