namespace PurpleIK.UI.Areas.Employee.Models.VM.EmployeePermision
{
    public class EmployeePermissionsEditVM
    {
        public Guid Id { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? NumberOfDays { get; set; }
        public IFormFile? PermissionFile { get; set; }
        public byte[]? PermissionFile_ { get; set; }
        public string? Status { get; set; }
    }
}
