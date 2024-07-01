using PurpleIK.Core.Enums;
using PurpleIK.Entities;

namespace PurpleIK.UI.Areas.CompanyManager.Models.VM.PushMoneyVM
{
    public class PushMoneyIndexVM
    {
        public Guid Id { get; set; }
        public string Picture { get; set; }
        public string Employee { get; set; }
        public string Type { get; set; }
        public int Amount { get; set; }
        public string? CurrencyUnit { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid PersonId { get; set; }

    }
}
