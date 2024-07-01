using PurpleIK.Entities;
using System.ComponentModel.DataAnnotations;

namespace PurpleIK.UI.Areas.Admin.Models.VM.PermissionVM
{
    public class AddPermissionVM
    {
        [Required]
        [MaxLength(50)]
        public string PermissionType { get; set; }
        
    }
}
