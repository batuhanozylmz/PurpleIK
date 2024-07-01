namespace PurpleIK.UI.Areas.CompanyManager.Models.VM.PermissionVM
{
    public class ManagerPermissionIndexVM
    {
        public Guid Id { get; set; }
        public string? Employee { get; set; }
        public string Picture { get; set; }
        public string? Role { get; set; }
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
