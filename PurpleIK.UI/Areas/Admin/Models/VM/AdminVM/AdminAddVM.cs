using PurpleIK.Core.Enums;

namespace PurpleIK.UI.Areas.Admin.Models.VM.AdminVM
{
    public class AdminAddVM
    {
        public string? AdminName { get; set; }
        public string AdminLastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public Status Status { get; set; } = Status.Active;
    }
}
