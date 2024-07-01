using PurpleIK.Core.Enums;
using PurpleIK.UI.Areas.Employee.Models.VM.DebitEmployeeVM;

namespace PurpleIK.UI.Areas.Employee.Models.VM.ExpenseEmployeeVM
{
    public class ExpenseEmployeeIndexVM
    {
        public List<ExpenseEmployeeItem> ExpenseEmployeeItems { get; set; }
    }

    public class ExpenseEmployeeItem
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? ExpenseDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? Status { get; set; }
        public decimal? Price { get; set; }
        public byte[]? ExpenseForm { get; set; }
        public Guid? PersonId { get; set; }
        public Guid? ManagerId { get; set; }
        public string? ManagerName { get; set; }
    }
}
