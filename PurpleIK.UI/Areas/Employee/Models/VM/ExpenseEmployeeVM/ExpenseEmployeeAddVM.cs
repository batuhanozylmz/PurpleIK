using PurpleIK.Entities;
using PurpleIK.UI.Areas.CompanyManager.Models.VM.DebitVM;

namespace PurpleIK.UI.Areas.Employee.Models.VM.ExpenseEmployeeVM
{
    public class ExpenseEmployeeAddVM
    {
        public ExpenseEmployeeAddItem ExpenseEmployeeAddItem { get; set; }
        public List<Person>? Persons { get; set; }
    }

    public class ExpenseEmployeeAddItem
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
