using PurpleIK.Core.Enums;

namespace PurpleIK.UI.Models.VM.CommentVM
{
    public class CommentDetailVM
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; }
        public string CompanyManagerName { get; set; }
        public Guid? PersonId { get; set; }
        public string? Department { get; set; }
        public string? Status { get; set; }
       
        public string? Summary { get; set; }
        public string CommentText { get; set; }
        public string? Title { get; set; }
        public byte[]? Photo { get; set; }
        public string Picture { get; set; }
        public byte[]? Logo_ { get; set; }
        public string LogoPicture { get; set; }
    }
}
