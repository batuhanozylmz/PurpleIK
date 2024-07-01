using PurpleIK.Entities;

namespace PurpleIK.UI.Areas.Admin.Models.VM.AdminVM
{
    public class AdminIndexVM
    {
        public Person Person { get; set; }
        public Company Company { get; set; }
        public List<Person> Persons { get; set; }
        public List<Company> ActiveCompanys { get; set; }
        public List<Company> DeActiveCompanys { get; set; }
        public List<Company> ApprovalCompanys { get; set; }
        public List<AppUser> ActiveCompanyAdmins { get; set; } // Aktif şirket yöneticileri
        public List<AppUser> DeactiveCompanyAdmins { get; set; } // Pasif şirket yöneticileri
        public List<AppUser> ActiveEmployees { get; set; } // Aktif şirket yöneticileri
        public List<AppUser> DeactiveEmployees { get; set; } // Pasif şirket yöneticileri
        public List<CompanyMembership> CompanyMemberships { get; set; }

    }
}
