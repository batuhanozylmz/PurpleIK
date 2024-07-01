using Microsoft.AspNetCore.Mvc.Rendering;
using PurpleIK.Core.Enums;
using PurpleIK.Entities;
using PurpleIK.UI.Areas.CompanyManager.Models.VM.PushMoneyVM;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace PurpleIK.UI.Areas.CompanyManager.Models.VM.DebitVM
{
    public class DebitAddVM
    {
        public DebitAddItem DebitAddItem { get; set; }
        public List<Person>? Persons { get; set; }
    }
    public class DebitAddItem
    {
        public string ProductName { get; set; }
        public DateTime ReceiptDate { get; set; }//alma
        public IFormFile? DebitFormFile { get; set; }
        public Guid? PersonId { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid? ManagerId { get; set; }
    }
}
