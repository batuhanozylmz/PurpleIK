using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using PurpleIK.Core.Enums;
using PurpleIK.Entities;
using PurpleIK.Services.Concretes;
using PurpleIK.Services.Interfaces;
using PurpleIK.UI.Areas.CompanyManager.Models.VM.PushMoneyVM;
using System.ComponentModel.Design;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace PurpleIK.UI.Areas.CompanyManager.Controllers
{
    [Area("CompanyManager")]
    [Authorize(Roles = "companymanager")]
    public class PushMoneyController : Controller
    {
        IPushMoneyService _pushMoneyService;
        UserManager<AppUser> _userManager;
        ICompanyService _companyService;
        IPersonService _personService;
        IMapper _mapper;
        public PushMoneyController(IPushMoneyService service,IPersonService personService, ICompanyService companyService, IMapper mapper, UserManager<AppUser> userManager)
        {
            _companyService = companyService;
            _pushMoneyService = service;
            _personService = personService;
            _mapper = mapper;
            _userManager = userManager;

        }
        public IActionResult List()
        {
            // Giriş yapmış kullanıcının adını al
            var userEmailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var companyManager = _personService.GetByEntity(x => x.CompanyEmail.ToLower() == userEmailClaim);

            List<PushMoneyIndexVM> vm = new();

            var pushMoneyList = _pushMoneyService.GetAll().Where(comment => comment.Person.CompanyId == companyManager.CompanyId).ToList();

            foreach (var item in pushMoneyList)
            {
                var pushMoney = new PushMoneyIndexVM();
                pushMoney.Id = item.Id;
                pushMoney.Type = item.Type;
                pushMoney.Amount = item.Amount;
                pushMoney.CurrencyUnit = item.CurrencyUnit;
                pushMoney.Description = item.Description;
                pushMoney.PersonId = (Guid)item.PersonId;

                // Prim kaydının sahibi olan çalışanı bul
                var employee = _personService.GetAll()
                    .FirstOrDefault(x => x.Id == item.PersonId && x.Company.Id == companyManager.CompanyId && x.Status == Status.Active);

                if (item.Person.ProfilePhoto != null)
                {
                    pushMoney.Picture = $"data:image/png;base64,{Convert.ToBase64String(item.Person.ProfilePhoto)}";
                }

                pushMoney.Employee = employee != null ? $"{employee.FirstName} {employee.LastName}" : "";
                vm.Add(pushMoney);
            }
            var pmVm = _mapper.Map<List<PushMoneyIndexVM>>(vm);
            return View(pmVm);

        }
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            // Giriş yapmış kullanıcının adını al
            var userEmailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var companyManager = _personService.GetByEntity(x => x.CompanyEmail.ToLower() == userEmailClaim);

            var persons = _personService.GetAll()
                .Where(x => x.Company.Id == companyManager.CompanyId && x.Status == Status.Active && x.AppUserId != null).ToList();

            var vm = new PushMoneyAddVM
            {
                Persons = persons
            };

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Add(PushMoneyAddVM vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    PushMoney p = _mapper.Map<PushMoney>(vm.PushMoneyAddItem);

                    p.CreatedDate = DateTime.Now;
                    p.ModifiedDate = DateTime.Now;
                    p.Status = Status.Active;


                    _pushMoneyService.Add(p);
                    return RedirectToAction("List");
                }
                catch (Exception ex)
                {

                }
            }
            TempData["message"] = $"Bir hata oluştu!";
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var pushMoney = _pushMoneyService.Find(id);
            if (pushMoney == null)
            {
                return NotFound();
            }

            var userEmailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var companyManager = _personService.GetByEntity(x => x.CompanyEmail.ToLower() == userEmailClaim);

            var persons = _personService.GetAll()
                .Where(x => x.Company.Id == companyManager.CompanyId && x.Status == Status.Active && x.AppUserId != null).ToList();

            var vm = new PushMoneyEditVM
            {
                PushMoneyEditItem = new PushMoneyEditItem
                {
                    Id = pushMoney.Id, // Id değerinin atanması
                    Type = pushMoney.Type,
                    Amount = pushMoney.Amount,
                    CurrencyUnit = pushMoney.CurrencyUnit,
                    Description = pushMoney.Description,
                    PersonId = pushMoney.PersonId
                },
                Persons = persons
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PushMoneyEditVM vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var pushMoney = _pushMoneyService.Find(vm.PushMoneyEditItem.Id);
                    if (pushMoney == null)
                    {
                        return NotFound();
                    }

                    // Gerekli güncellemeleri yap
                    pushMoney.Type = vm.PushMoneyEditItem.Type;
                    pushMoney.Amount = vm.PushMoneyEditItem.Amount;
                    pushMoney.CurrencyUnit = vm.PushMoneyEditItem.CurrencyUnit;
                    pushMoney.Description = vm.PushMoneyEditItem.Description;
                    pushMoney.ModifiedDate = DateTime.Now;

                    _pushMoneyService.Edit(pushMoney);
                    return RedirectToAction("List");
                }
                catch (Exception ex)
                {
                    // Hata yönetimi
                }
            }

            // ModelState geçerli değilse, formu tekrar görüntüle
            TempData["message"] = $"Bir hata oluştu!";
            return View(vm);
        }
        [HttpGet]
        public IActionResult Remove(Guid id)
        {
            try
            {
                PushMoney p = _pushMoneyService.Find(id);
                _pushMoneyService.Remove(p);
            }
            catch (Exception)
            {

            }
            return RedirectToAction("List");
        }
    }
}
