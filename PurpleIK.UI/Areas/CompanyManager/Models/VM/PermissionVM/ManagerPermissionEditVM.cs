using PurpleIK.Core.Enums;
using PurpleIK.Entities;

namespace PurpleIK.UI.Areas.CompanyManager.Models.VM.PermissionVM
{
    public class ManagerPermissionEditVM
    {
        public List<Permission>? Permissions { get; set; }
        public ManagerPermissionEditItem ManagerPermissionEditItem { get; set; }
        public List<Person>? Persons { get; set; }
    }
    public class ManagerPermissionEditItem
    {
        public Guid Id { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? NumberOfDays { get; set; }
        public IFormFile? PermissionFile { get; set; }
        public byte[]? PermissionFile_ { get; set; }
        public string? ReasonOfRejection { get; set; }
        public Guid? PermissionId { get; set; }
        public Guid? PersonId { get; set; }
        public string? Status { get; set; }
        public string? ApprovalStatus { get; set; }
    }
}