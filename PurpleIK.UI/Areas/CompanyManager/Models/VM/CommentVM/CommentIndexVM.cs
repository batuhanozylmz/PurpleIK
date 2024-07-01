using PurpleIK.Core.Enums;

namespace PurpleIK.UI.Areas.CompanyManager.Models.VM.CommentVM
{
    public class CommentIndexVM
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; }
        public string CompanyManagerName { get; set; }
        public string? Department { get; set; }
        public string Picture { get; set; }
        public Guid? ActiveId { get; set; }
        public Status? Status { get; set; }

        public string CommentText { get; set; }
        public string? Title { get; set; }
        public string? Summary { get; set; }

        public Guid? PersonId { get; set; }
        public string? CreatedDate { get; set; }
    }
}
