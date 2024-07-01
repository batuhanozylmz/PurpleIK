using PurpleIK.Entities;

namespace PurpleIK.UI.Areas.CompanyManager.Models.VM.ExpenseVM
{
    public class ExpenseIndexVM
    {
        public Guid? Id { get; set; }
        public string? Employee { get; set; }
        public string? Picture { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? ExpenseDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? Status { get; set; }
        public decimal? Price { get; set; }
        public byte[]? ExpenseForm { get; set; }
        public Guid? PersonId { get; set; }
        public Guid? ManagerId { get; set; }
    }
}
