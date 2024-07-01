using PurpleIK.Entities;

namespace PurpleIK.UI.Areas.CompanyManager.Models.VM.PushMoneyVM
{
    public class PushMoneyAddVM
    {
        public PushMoneyAddItem PushMoneyAddItem { get; set; }
        public List<Person>? Persons { get; set; }


    }

    public class PushMoneyAddItem
    {
        public string Type { get; set; }
        public int Amount { get; set; }
        public string? CurrencyUnit { get; set; }
        public string? Description { get; set; }
        public Guid? PersonId { get; set; }
        public Guid? CompanyId { get; set; }
    }

}
