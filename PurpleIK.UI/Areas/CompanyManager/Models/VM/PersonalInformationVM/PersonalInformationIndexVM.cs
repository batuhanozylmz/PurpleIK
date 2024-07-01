namespace PurpleIK.UI.Areas.CompanyManager.Models.VM.PersonalInformationVM
{
    public class PersonalInformationIndexVM
    {
        public Guid Id { get; set; }
        public string FormName { get; set; }
        public string Employee { get; set; }
        public string Picture { get; set; }
        public byte[]? PersonalInformationForm { get; set; }
        public Guid? PersonId { get; set; }
        public Guid? ManagerId { get; set; }
    }
}
