using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurpleIK.Core.Enums
{
    public enum Status
    {
        [Display(Name = "Aktif")]
        Active = 1,
        [Display(Name = "Pasif")]
        DeActive = 2,
        [Display(Name = "Onaylı")]
        Approved = 3,
        [Display(Name = "Silinmiş")]
        Deleted = 4,
        [Display(Name = "Onay Bekliyor")]
        Approval = 5
    }
}
