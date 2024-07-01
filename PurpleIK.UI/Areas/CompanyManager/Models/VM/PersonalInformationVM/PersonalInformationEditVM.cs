using PurpleIK.Entities;

namespace PurpleIK.UI.Areas.CompanyManager.Models.VM.PersonalInformationVM
{
    public class PersonalInformationEditVM
    {
        public PersonalInformationEditItem PersonalInformationEditItem { get; set; }
        public List<Person>? Persons { get; set; }
    }
    public class PersonalInformationEditItem
    {
        public Guid Id { get; set; }
        public string FormName { get; set; }
        public IFormFile? PersonalInformationFormFile { get; set; }
        public byte[]? PersonalInformationForm { get; set; }
        public Guid? PersonId { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid? ManagerId { get; set; }
    }
}
