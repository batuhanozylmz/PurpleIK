using PurpleIK.Core.Enums;

namespace PurpleIK.UI.Models.VM.CommentVM
{
    public class CommentListVM
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; }
        public string CompanyManagerName { get; set; }
        public Guid? PersonId { get; set; }
        public string? Department { get; set; }
        public Status? Status { get; set; }
        public string Picture { get; set; }
        public string? Summary { get; set; }
    }
}
