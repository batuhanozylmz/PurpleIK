using PurpleIK.Core.Enums;
using PurpleIK.Entities;
using System.ComponentModel.DataAnnotations;

namespace PurpleIK.UI.Areas.CompanyManager.Models.VM.PersonalInformationVM
{
    public class PersonalInformationAddVM
    {
        public PersonalInformationAddItem PersonalInformationAddItem { get; set; }
        public List<Person>? Persons { get; set; }
    }
    public class PersonalInformationAddItem
    {
        public string FormName { get; set; }
        public IFormFile? PersonalInformationFormFile { get; set; }
        public Guid? PersonId { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid? ManagerId { get; set; }
    }
}
