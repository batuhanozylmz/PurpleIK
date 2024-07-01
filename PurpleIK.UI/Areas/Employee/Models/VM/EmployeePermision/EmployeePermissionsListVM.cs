namespace PurpleIK.UI.Areas.Employee.Models.VM.EmployeePermision
{
    public class EmployeePermissionsListVM
    {
        public Guid Id { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? NumberOfDays { get; set; }
        public DateTime? DateOfReply { get; set; }
        public byte[]? PermissionFile { get; set; }
        public string? ReasonOfRejection { get; set; }
        public string? CompanyManagerName { get; set; }
        public string? CompanyManagerEmail { get; set; }
        public string? Status { get; set; }
    }
}
