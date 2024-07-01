using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PurpleIK.Core.Enums;
using PurpleIK.Entities;
using PurpleIK.Services.Concretes;
using PurpleIK.Services.Interfaces;
using PurpleIK.UI.Areas.CompanyManager.Models.VM.DebitVM;
using PurpleIK.UI.Areas.CompanyManager.Models.VM.PersonalInformationVM;
using PurpleIK.UI.Areas.CompanyManager.Models.VM.PushMoneyVM;
using System.Collections.Generic;
using System.IO.Compression;
using System.Security.Claims;

namespace PurpleIK.UI.Areas.CompanyManager.Controllers
{
    [Area("CompanyManager")]
    [Authorize(Roles = "companymanager")]
    public class PersonalInformationController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ICompanyService _companyService;
        private readonly IMapper _mapper;
        private readonly IPersonService _personService;
        private readonly IPersonalInformationService _personalInformationService;
        public PersonalInformationController(UserManager<AppUser> userManager,  ICompanyService companyService, IMapper mapper, IPersonService personService,IPersonalInformationService personalInformationService)
        {
            _userManager = userManager;
            _companyService = companyService;
            _mapper = mapper;
            _personService = personService;
            _personalInformationService = personalInformationService;
        }

        public IActionResult Index()
        {
            // Giriş yapmış kullanıcının adını al
            var userEmailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var companyManager = _personService.GetByEntity(x => x.CompanyEmail.ToLower() == userEmailClaim);

            List<PersonalInformationIndexVM> vm = new();

            var personalInformationList = _personalInformationService.GetAll().Where(comment => comment.Person.CompanyId == companyManager.CompanyId).ToList();

            foreach (var item in personalInformationList)
            {
                var personalInformation = new PersonalInformationIndexVM();
                personalInformation.Id = item.Id;
                personalInformation.FormName = item.FormName;
                personalInformation.PersonalInformationForm = item.PersonalInformationForm;
                personalInformation.PersonId = (Guid)item.PersonId;

                var employee = _personService.GetAll()
                   .FirstOrDefault(x => x.Id == item.PersonId && x.Company.Id == companyManager.CompanyId && x.Status == Status.Active);

                if (employee.ProfilePhoto != null)
                {
                    personalInformation.Picture = $"data:image/png;base64,{Convert.ToBase64String(employee.ProfilePhoto)}";
                }

                personalInformation.Employee = employee != null ? $"{employee.FirstName} {employee.LastName}" : "";
                vm.Add(personalInformation);
            }
            var piVm = _mapper.Map<List<PersonalInformationIndexVM>>(vm);
            return View(piVm);
        }
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            // Giriş yapmış kullanıcının adını al
            var userEmailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var companyManager = _personService.GetByEntity(x => x.CompanyEmail.ToLower() == userEmailClaim);

            var persons = _personService.GetAll()
                .Where(x => x.Company.Id == companyManager.CompanyId && x.Status == Status.Active && x.AppUserId != null).ToList();

            var vm = new PersonalInformationAddVM
            {
                Persons = persons
            };

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Add(PersonalInformationAddVM vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Özlük Belgesi ile ilgili tüm alanlara veri giriniz.";
                return View(vm);
            }
                try
                {
                    var userEmailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                    var person = _personService.GetByEntity(x => x.CompanyEmail.ToLower() == userEmailClaim);

                    PersonalInformation p = _mapper.Map<PersonalInformation>(vm.PersonalInformationAddItem);

                    p.CreatedDate = DateTime.Now;
                    p.ModifiedDate = DateTime.Now;
                    p.Status = Status.Approval;
                    p.ManagerId = person.Id;

                    if (vm.PersonalInformationAddItem.PersonalInformationFormFile != null && vm.PersonalInformationAddItem.PersonalInformationFormFile.Length > 0)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            vm.PersonalInformationAddItem.PersonalInformationFormFile.CopyTo(ms);
                            p.PersonalInformationForm = ms.ToArray();
                        }
                    }

                    _personalInformationService.Add(p);
                    TempData["SuccessInformationMessage"] = "Özlük Belgesi Başarıyla Eklendi.";
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    TempData["ErrorInformationMessage"] = $"Özlük Belgesi Eklenemedi!";
                    return View(vm);
                }
            
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var personalInformation = _personalInformationService.Find(id);
            if (personalInformation == null)
            {
                return NotFound();
            }

            var userEmailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var companyManager = _personService.GetByEntity(x => x.CompanyEmail.ToLower() == userEmailClaim);

            var persons = _personService.GetAll()
                .Where(x => x.Company.Id == companyManager.CompanyId && x.Status == Status.Active && x.AppUserId != null).ToList();

            var vm = new PersonalInformationEditVM
            {
                PersonalInformationEditItem = new PersonalInformationEditItem
                {
                    Id = personalInformation.Id, 
                    PersonId = personalInformation.PersonId,
                    FormName = personalInformation.FormName,
                    PersonalInformationForm = personalInformation.PersonalInformationForm,
                    ManagerId = personalInformation.ManagerId,

                },
                Persons = persons
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PersonalInformationEditVM vm)
        {
                if (!ModelState.IsValid)
                {
                     TempData["ErrorInformationMessage"] = "Özlük Belgesi Değişikliği ile ilgili tüm alanlara veri giriniz";
                     return View(vm);
                }
                try
                {
                    var userEmailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                    var person = _personService.GetByEntity(x => x.CompanyEmail.ToLower() == userEmailClaim);

                    // Özlük Bilgilerini bul
                    var personalInformation = _personalInformationService.Find(vm.PersonalInformationEditItem.Id);
                    if (personalInformation == null)
                    {
                        return NotFound();
                    }

                    personalInformation.FormName = vm.PersonalInformationEditItem.FormName;
                    personalInformation.PersonId = vm.PersonalInformationEditItem.PersonId;
                    personalInformation.ModifiedDate = DateTime.Now;
                    personalInformation.Status = Status.Active;
                    personalInformation.ManagerId = person.Id;
                    if (vm.PersonalInformationEditItem.PersonalInformationFormFile != null && vm.PersonalInformationEditItem.PersonalInformationFormFile.Length > 0)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            vm.PersonalInformationEditItem.PersonalInformationFormFile.CopyTo(ms);
                            personalInformation.PersonalInformationForm = ms.ToArray();
                        }
                    }

                    // Değişiklikleri kaydet
                    _personalInformationService.Edit(personalInformation);
                    TempData["SuccessInformationMessage"] = "Özlük Belgesi Başarıyla Güncellendi.";
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    TempData["ErrorInformationMessage"] = "Özlük Belgesi Güncellenemedi!";
                    return View(vm);
                }
            
        }

        public IActionResult GetDocument(Guid id, string layoutName)
        {
            var info = _personalInformationService.Find(id);
            if (info == null || info.PersonalInformationForm == null)
            {
                TempData["ErrorMessage"] = "Dosya Bulunamadı";
                if (layoutName == "Edit")
                {
                    return RedirectToAction(layoutName, new { id });
                }
                return RedirectToAction(layoutName);
            }

            return File(info.PersonalInformationForm, "application/pdf");
        }

        [HttpGet]
        public async Task<IActionResult> Remove(Guid id)
        {
            try
            {
                PersonalInformation p = _personalInformationService.Find(id);
                _personalInformationService.Remove(p);
                TempData["SuccessInformationMessage"] = "Özlük Belgesi Başarıyla Silindi.";
            }
            catch (Exception)
            {
                TempData["ErrorInformationMessage"] = "Özlük Belgesi Bulunamadı";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

    }
}
