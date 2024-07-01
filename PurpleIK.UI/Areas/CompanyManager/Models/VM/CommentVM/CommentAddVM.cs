using PurpleIK.Core.Enums;
using PurpleIK.Entities;

namespace PurpleIK.UI.Areas.CompanyManager.Models.VM.CommentVM
{
    public class CommentAddVM
    {
        public string CommentText { get; set; }
        public string? Title { get; set; }
        public string? Summary { get; set; }   
        public IFormFile? _Photo { get; set; }
        public Status? Status { get; set; }
        public Guid PersonId { get; set; }





    }
}
