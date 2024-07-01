using PurpleIK.Core.Enums;
using PurpleIK.Entities;

namespace PurpleIK.UI.Areas.CompanyManager.Models.VM.DebitVM
{
    public class DebitIndexVM
    {
        public Guid Id { get; set; }
        public string Employee { get; set; }
        public string Picture { get; set; }
        public string ProductName { get; set; }
        public DateTime? ReceiptDate { get; set; }//alma
        public DateTime? DeliveryDate { get; set; }//verme
        public DateTime CreatedDate { get; set; }
        public byte[]? DebitForm { get; set; }
        public Guid? PersonId { get; set; }
        public string? Status { get; set; }
        public Guid? ManagerId { get; set; }
    }
}
