using PurpleIK.Core.Enums;
using PurpleIK.Entities;
using System.ComponentModel.DataAnnotations;

namespace PurpleIK.UI.Areas.Admin.Models.VM.PermissionVM
{
    public class EditPermissonVM
    {
        public Guid Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string PermissionType { get; set; }
    }
}
