using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PurpleIK.Core.Enums;
using PurpleIK.Entities;
using PurpleIK.Services.Interfaces;
using PurpleIK.UI.Areas.CompanyManager.Models.VM.DebitVM;
using PurpleIK.UI.Areas.Employee.Models.VM.DebitEmployeeVM;
using PurpleIK.UI.Utility;
using System.Security.Claims;

namespace PurpleIK.UI.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize(Roles = "employee")]
    public class DebitEmployeeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ICompanyService _companyService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IPersonService _personService;
        private readonly IDebitService _debitService;


        public DebitEmployeeController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ICompanyService companyService, IMapper mapper, IConfiguration configuration, IPersonService personService, IDebitService debitService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _companyService = companyService;
            _mapper = mapper;
            _configuration = configuration;
            _personService = personService;
            _debitService = debitService;
        }
        public IActionResult DebitEmployeeList()
        {
            // Giriş yapmış kullanıcının AppUser kimliğini al
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            // Kullanıcının kimlik bilgisini kullanarak personel kaydını bul
            var person = _personService.GetByEntity(x => x.AppUserId == Guid.Parse(userId));

            // Çalışanın tüm zimmetlerini getirir
            var debitsForEmployee = _debitService.GetAll()
                .Where(debit => debit.PersonId == person.Id)
                .Select(debit => new DebitEmployeeItem
                {
                    Id = debit.Id,
                    ProductName = debit.ProductName,
                    ReceiptDate = debit.ReceiptDate.Value,
                    DeliveryDate = debit.DeliveryDate,
                    DebitForm = debit.DebitForm,
                    PersonId = debit.PersonId,
                    ManagerId = debit.ManagerId,
                    Status= debit.Status.ToString()
                })
                .ToList();

            // Yönetici adı ve soyadını almak için ManagerId kullanarak ilgili personel kaydını bulun
            foreach (var debit in debitsForEmployee)
            {
                var manager = _personService.GetByEntity(x => x.Id == debit.ManagerId);
                debit.ManagerName = $"{manager.FirstName} {manager.LastName}"; 
            }

            var vm = new DebitEmployeeIndexVM
            {
                DebitEmployeeItems = debitsForEmployee
            };
            return View(vm);
        }
        public async Task<IActionResult> ActiveDebit(Guid id)
        {
            var debit = _debitService.Find(id);

            // Zimmetin durumunu "Onaylı" olarak güncelle
            debit.Status = Status.Active;
            _debitService.Edit(debit);

            TempData["SuccessMessage"] = "Zimmet başarıyla onaylandı.";
            return RedirectToAction("DebitEmployeeList");
        }
        [HttpGet]
        public async Task<IActionResult> DeActiveDebit(Guid id)
        {
            var vm = new RejectVM { Id = id };
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> DeActiveDebit(RejectVM vm)
        {
            if (ModelState.IsValid)
            {
                var debit = _debitService.Find(vm.Id);
                
                // Zimmetin durumunu "Reddedilen" olarak güncelle
                debit.Status = Status.DeActive;
                _debitService.Edit(debit);

                // E-posta gönderme işlemleri
                string mailBody = $"Zimmet reddedildi. Reddetme nedeni: {vm.Reject}";
                var manager = _personService.GetByEntity(x => x.Id == debit.ManagerId);
                MailHelper.SendMail(manager.PersonalEmail, " Zimmet Reddetme Maili", mailBody, null, null);
                TempData["SuccessMessage"] = "Zimmet reddedildi";
                return RedirectToAction("DebitEmployeeList");
            }
            return View(vm);
        }

        public IActionResult GetDocument(Guid id, string layoutName)
        {
            var debit = _debitService.Find(id);
            if (debit == null || debit.DebitForm == null)
            {
                TempData["ErrorMessage"] = "Dosya Bulunamadı";
                if (layoutName == "Edit")
                {
                    return RedirectToAction(layoutName, new { id });
                }
                return RedirectToAction(layoutName);
            }

            return File(debit.DebitForm, "application/pdf");
        }
    }
}
