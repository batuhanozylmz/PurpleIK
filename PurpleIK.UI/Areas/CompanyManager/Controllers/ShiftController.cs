using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PurpleIK.Core.Enums;
using PurpleIK.Entities;
using PurpleIK.Services.Concretes;
using PurpleIK.Services.Interfaces;
using PurpleIK.UI.Areas.CompanyManager.Models.VM.PermissionVM;
using PurpleIK.UI.Areas.CompanyManager.Models.VM.ShiftVM;
using System.Security.Claims;

namespace PurpleIK.UI.Areas.CompanyManager.Controllers
{
    [Area("CompanyManager")]
    [Authorize(Roles = "companymanager")]
    public class ShiftController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ICompanyService _companyService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IPersonService _personService;
        private readonly IShiftService _shiftService;




        public ShiftController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ICompanyService companyService, IMapper mapper, IConfiguration configuration, IPersonService personService, IShiftService shiftService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _companyService = companyService;
            _mapper = mapper;
            _configuration = configuration;
            _personService = personService;
            _shiftService = shiftService;
        }
        public IActionResult Index()
        {
            // Giriş yapmış kullanıcının adını al
            var userEmailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var companyManager = _personService.GetByEntity(x => x.CompanyEmail.ToLower() == userEmailClaim);
            List<ShiftIndexVM> vm = new();

            var shiftList = _shiftService.GetAll().Where(x => x.Person.CompanyId == companyManager.CompanyId).ToList();

            foreach (var item in shiftList)
            {
                ShiftIndexVM shiftIndex = _mapper.Map<ShiftIndexVM>(item);

                //kaydının sahibi olan çalışanı bul
                var employee = _personService.GetAll()
                    .FirstOrDefault(x => x.Id == item.PersonId && x.Company.Id == companyManager.CompanyId && x.Status == Status.Active);

                if (employee.ProfilePhoto != null)
                {
                    shiftIndex.Picture = $"data:image/png;base64,{Convert.ToBase64String(employee.ProfilePhoto)}";
                }

                shiftIndex.Employee = employee != null ? $"{employee.FirstName} {employee.LastName}" : "";

                shiftIndex.TotalShiftTime = item.EndTime - item.StartTime;
                shiftIndex.TotalBreakTimeOne = item.BreakTimeOneEnd - item.BreakTimeOneStart;
                shiftIndex.TotalBreakTimeSecond = item.BreakTimeSecondEnd - item.BreakTimeSecondStart;
                shiftIndex.ShiftDate = item.ShiftDate;
                vm.Add(shiftIndex);
            }
            return View(vm);
        }
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var vm = new ShiftAddVM();

            // Giriş yapmış kullanıcının adını al
            var userEmailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var companyManager = _personService.GetByEntity(x => x.CompanyEmail.ToLower() == userEmailClaim);

            var persons = _personService.GetAll()
                .Where(x => x.Company.Id == companyManager.CompanyId && x.Status == Status.Active && x.AppUserId != null).ToList();
            vm.Persons = persons;
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Add(ShiftAddVM vm)
        {

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "İzin Talebi ile ilgili tüm alanlara veri giriniz";
                return View(vm);
            }
            // Giriş yapmış kullanıcının adını al
            var userEmailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var companyManager = _personService.GetByEntity(x => x.CompanyEmail.ToLower() == userEmailClaim);
            try
            {
                Shift s = new();
                s.Name = vm.ShiftAddItem.Name;
                s.StartTime = vm.ShiftAddItem.StartTime;
                s.EndTime = vm.ShiftAddItem.EndTime;
                s.BreakTimeOneStart = vm.ShiftAddItem.BreakTimeOneStart;
                s.BreakTimeOneEnd = vm.ShiftAddItem.BreakTimeOneEnd;
                s.BreakTimeSecondStart = vm.ShiftAddItem.BreakTimeSecondStart;
                s.BreakTimeSecondEnd = vm.ShiftAddItem.BreakTimeSecondEnd;
                s.ShiftDate = vm.ShiftAddItem.ShiftDate;
                s.CreatedDate = DateTime.Now;
                s.Status = Status.Active;

                var person = _personService.GetByEntity(x => x.Id == vm.ShiftAddItem.PersonId);

                s.PersonId = person.Id;
                s.Person = person;

                _shiftService.Add(s);

                TempData["SuccessMessage"] = "Vardiya Başarıyla Oluşturuldu.";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Vardiya İşlemi Başarısız Oldu";
                return View(vm);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var shift = _shiftService.Find(id);
            if (shift == null)
            {
                TempData["ErrorMessage"] = "İzin Bulunamadı";
                return RedirectToAction("Index");
            }

            ShiftEditVM vm = new ShiftEditVM();
            if (vm.ShiftEditItem == null)
            {
                vm.ShiftEditItem = new ShiftEditItem();
            }
            vm.ShiftEditItem.Id = shift.Id;
            vm.ShiftEditItem.ShiftDate = shift.ShiftDate;
            vm.ShiftEditItem.StartTime = shift.StartTime;
            vm.ShiftEditItem.EndTime = shift.EndTime;
            vm.ShiftEditItem.BreakTimeOneStart = shift.BreakTimeOneStart;
            vm.ShiftEditItem.BreakTimeOneEnd = shift.BreakTimeOneEnd;
            vm.ShiftEditItem.BreakTimeSecondStart = shift.BreakTimeSecondStart;
            vm.ShiftEditItem.BreakTimeSecondEnd = shift.BreakTimeSecondEnd;
            vm.ShiftEditItem.Name = shift.Name;

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(ShiftEditVM vm)
        {

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Vardiya Değişikliği ile ilgili tüm alanlara veri giriniz";
                return View(vm);
            }

            try
            {
                var s = _shiftService.Find(vm.ShiftEditItem.Id);
                if (s == null)
                {
                    TempData["ErrorMessage"] = "Vardiya Bulunamadı";
                    return RedirectToAction("Index");
                }
                s.Name = vm.ShiftEditItem.Name;
                s.StartTime = vm.ShiftEditItem.StartTime;
                s.EndTime = vm.ShiftEditItem.EndTime;
                s.BreakTimeOneStart = vm.ShiftEditItem.BreakTimeOneStart;
                s.BreakTimeOneEnd = vm.ShiftEditItem.BreakTimeOneEnd;
                s.BreakTimeSecondStart = vm.ShiftEditItem.BreakTimeSecondStart;
                s.BreakTimeSecondEnd = vm.ShiftEditItem.BreakTimeSecondEnd;
                s.ShiftDate = vm.ShiftEditItem.ShiftDate;
                s.ModifiedDate = DateTime.Now;

                _shiftService.Edit(s);

                TempData["SuccessMessage"] = "Vardiya Başarıyla Güncellendi.";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Vardiya İşlemi Başarısız Oldu";
                return View(vm);
            }
        }
        public async Task<IActionResult> Delete(Guid id)
        {
            var shift = _shiftService.Find(id);
            if (shift == null)
            {
                TempData["ErrorMessage"] = "Vardiya Bulunamadı";
                return RedirectToAction("Index");
            }
            _shiftService.Remove(shift);
            TempData["SuccessMessage"] = "Vardiya Başarıyla Silindi.";
            return RedirectToAction("Index");
        }
    }
}
