using PurpleIK.Entities;
using System.Drawing;

namespace PurpleIK.UI.Areas.Employee.Models.VM.EmployeeVM
{
    public class EmployeeIndexVM
    {
        public Person Person { get; set; }
        public List<Person> Persons { get; set; }
        public List<PersonPermission> PersonPermission { get; set; }
        public List<PersonPermission> OtherEmployeesPermissions { get; set; }
        public List<PublicHolidays> PublicHolidays { get; set; }
        public List<Shift> Shifts { get; set; }
    }
}
