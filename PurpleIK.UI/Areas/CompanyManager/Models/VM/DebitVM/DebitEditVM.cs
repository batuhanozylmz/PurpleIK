using PurpleIK.Entities;

namespace PurpleIK.UI.Areas.CompanyManager.Models.VM.DebitVM
{
    public class DebitEditVM
    {
        public DebitEditItem DebitEditItem { get; set; }
        public List<Person>? Persons { get; set; }

    }

    public class DebitEditItem
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; }
        public DateTime? ReceiptDate { get; set; }//alma
        public DateTime? DeliveryDate { get; set; }//verme
        public IFormFile? DebitFormFile { get; set; }
        public byte[]? DebitFormFile_ { get; set; }
        public Guid? PersonId { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid? ManagerId { get; set; }
    }
}
