using PurpleIK.Core.Enums;
using PurpleIK.Entities;

namespace PurpleIK.UI.Areas.Admin.Models.VM.PermissionVM
{
    public class IndexPermissionVM
    { 
        public string IzinAdi { get; set; }
        public List<IndexPermissionItem> Permissions { get; set; }
    }
    public class IndexPermissionItem
    {
        public Guid Id { get; set; }
        public string PermissionType { get; set; }
    }
}
