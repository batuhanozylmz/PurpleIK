using PurpleIK.Entities;
using PurpleIK.UI.Areas.CompanyManager.Models.VM.DebitVM;

namespace PurpleIK.UI.Areas.CompanyManager.Models.VM.ExpenseVM
{
    public class ExpenseAddVM
    {
        public ExpenseAddItem ExpenseAddItem { get; set; }
        public List<Person>? Persons { get; set; }
    }

    public class ExpenseAddItem
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? ExpenseDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? Status { get; set; }
        public decimal? Price { get; set; }
        public IFormFile? ExpenseFormFile { get; set; }
        public Guid? PersonId { get; set; }
        public Guid? ManagerId { get; set; }
    }
}
