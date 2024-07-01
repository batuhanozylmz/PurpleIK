using PurpleIK.Core.Enums;
using PurpleIK.Entities;

namespace PurpleIK.UI.Areas.CompanyManager.Models.VM.CommentVM
{
    public class CommentEditVM
    {
        public Guid Id { get; set; }
        public string CommentText { get; set; }
        public string? Title { get; set; }
        public string? Summary { get; set; }
        public IFormFile? _Photo { get; set; }
        public string? PictureBase64 { get; set; }
        public Status? Status { get; set; }
        public Guid? PersonId { get; set; }
        public Department? Department { get; set; }
    }
}
