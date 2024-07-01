using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurpleIK.Core.Enums
{
    public enum Department
    {
        [Display(Name = "Satış")]
        Sales = 1,
        [Display(Name = "Pazarlama")]
        Marketing = 2,

        [Display(Name = "İnsan kaynakları")]
        HumanResources = 3,

        [Display(Name = "Finans")]
        Finance = 4,

        [Display(Name = "BT")]
        IT = 5,

        [Display(Name = "Operasyonlar")]
        Operations = 6,

        [Display(Name = "Müşteri servisi")]
        CustomerService = 7,

        [Display(Name = "Araştırma ve Geliştirme")]
        ResearchAndDevelopment = 8,

        [Display(Name = "Yasal")]
        Legal = 9,

        [Display(Name = "Yönetim")]
        Administration = 10
    }
}
