using PurpleIK.Entities;

namespace PurpleIK.UI.Areas.CompanyManager.Models.VM.PushMoneyVM
{
    public class PushMoneyEditVM
    {
        public PushMoneyEditItem PushMoneyEditItem { get; set; }
        public List<Person>? Persons { get; set; }

    }

    public class PushMoneyEditItem
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public int Amount { get; set; }
        public string? CurrencyUnit { get; set; }
        public string? Description { get; set; }
        public Guid? PersonId { get; set; }
    }
}
