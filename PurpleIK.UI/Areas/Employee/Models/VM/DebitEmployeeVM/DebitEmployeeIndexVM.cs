using PurpleIK.Core.Enums;
using PurpleIK.Entities;

namespace PurpleIK.UI.Areas.Employee.Models.VM.DebitEmployeeVM
{
    public class DebitEmployeeIndexVM
    {
        public List<DebitEmployeeItem> DebitEmployeeItems { get; set; }

    }
    public class DebitEmployeeItem
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; }
        public DateTime ReceiptDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public byte[]? DebitForm { get; set; }
        public string? Status { get; set; }
        public Guid? PersonId { get; set; }
        public Guid? ManagerId { get; set; }
        public string? ManagerName { get; set; }

    }

}
