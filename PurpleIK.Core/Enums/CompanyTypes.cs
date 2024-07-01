using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurpleIK.Core.Enums
{
    public enum CompanyTypes
    {
        [Display(Name = "Kamu Limited Şirket")]
        PublicLimitedCompany,
        [Display(Name = "Özel Limited şirket")]
        PrivateLimitedCompany,
        [Display(Name = "Genel Ortaklık")]
        GeneralPartnership,
        [Display(Name = "Sınırlı Ortaklık")]
        LimitedPartnership,
        [Display(Name = "Kooperatif")]
        Cooperative,
        [Display(Name = "Şahıs Mülkiyeti")]
        SoleProprietorship
    }
}
