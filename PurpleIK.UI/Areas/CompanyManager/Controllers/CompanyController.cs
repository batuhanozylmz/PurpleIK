using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PurpleIK.Core.Enums;
using PurpleIK.Entities;
using PurpleIK.Services.Interfaces;
using PurpleIK.UI.Utility;
using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using PurpleIK.Services.Concretes;
using System.Security.Claims;
using PurpleIK.UI.Areas.CompanyManager.Models.VM.CompanyVM;
using PurpleIK.Core;
using PurpleIK.UI.Areas.CompanyManager.Models.VM.EmployeeManagerVM;
using PurpleIK.UI.Areas.Employee.Models.VM.EmployeeVM;

namespace PurpleIK.UI.Areas.CompanyManager.Controllers
{
    [Area("CompanyManager")]
    [Authorize(Roles = "companymanager")]
    public class CompanyController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ICompanyService _companyService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IPersonService _personService;
        private readonly IPersonPermissionService _personPermissionService;
        private readonly IShiftService _shiftService;

        public CompanyController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ICompanyService companyService, IMapper mapper, IConfiguration configuration, IPersonService personService, IPersonPermissionService personPermissionService, IShiftService shiftService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _companyService = companyService;
            _mapper = mapper;
            _configuration = configuration;
            _personService = personService;
            _personPermissionService = personPermissionService;
            _shiftService = shiftService;
        }

        // [Authorize]
        public async Task<IActionResult> Home()
        {
            // Giriş yapmış kullanıcının e-posta id'sini al
            var userEmailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var companyManager = _personService.GetByEntity(x => x.CompanyEmail.ToLower() == userEmailClaim);

            if (companyManager == null)
            {
                // Kullanıcı bulunamadı veya yetkilendirme hatası
                return RedirectToAction("AccessDenied", "Error"); // Veya uygun bir yönlendirme
            }

            var employees = _personService.GetAll().Where(x => x.Company.Id == companyManager.CompanyId);
            var company = _companyService.GetByEntity(x => x.Id == companyManager.CompanyId);

            if (employees == null)
            {
                // Şirkette çalışan bulunamadı
                return View();
            }

            List<IndexVM> employeeListVM = new List<IndexVM>();

            foreach (var employee in employees)
            {
                IndexVM employeeVM = new IndexVM();
                employeeVM.IndexItems = new List<IndexItem>();

                IndexItem indexItem = _mapper.Map<IndexItem>(employee);
                indexItem.BirhtPlace = employee.BirhtPlace;
                indexItem.CompanyName = company.CompanyName;
                indexItem.Department = employee.Department.GetDisplayName();
                indexItem.Gender = employee.Gender.GetDisplayName();
                indexItem.Status = employee.Status.GetDisplayName();
                ViewBag.CompanyName = company.CompanyName;

                // Çalışanın rollerini al
                var user = await _userManager.FindByEmailAsync(employee.CompanyEmail);
                var roles = await _userManager.GetRolesAsync(user);
                // Eğer çalışanın rolleri varsa, ilk rolü al
                indexItem.Role = roles.Any() ? roles.First() : "employee"; // Roller yoksa varsayılan ata

                if (employee.ProfilePhoto != null)
                {
                    indexItem.Picture = $"data:image/png;base64,{Convert.ToBase64String(employee.ProfilePhoto)}";
                }

                // Çalışanın şirketindeki diğer personellerin doğum günleri
                var persons = _personService.GetAll()
                    .Where(x => x.Id != companyManager.Id && x.CompanyId == companyManager.CompanyId && x.Status == Status.Active && x.BirthDate != null)
                    .OrderBy(x => x.BirthDate.HasValue && x.BirthDate.Value.Month == DateTime.Now.Month ? (x.BirthDate.Value.Day >= DateTime.Now.Day ? 0 : 1) : (x.BirthDate.Value.Month > DateTime.Now.Month ? 0 : 1))
                    .ThenBy(x => x.BirthDate)
                    .Take(3)
                    .ToList();
                // Diğer çalışanların izinlerini
                var currentDate = DateTime.Now;

                var otherEmployeesPermissions = _personPermissionService.GetAll()
                    .Where(x => x.PersonId != companyManager.Id
                                && x.Person.CompanyId == companyManager.CompanyId
                                && x.Status == Status.Active)
                    .OrderBy(x => x.StartDate.HasValue && x.StartDate.Value.Month == currentDate.Month ?
                                  (x.StartDate.Value.Day >= currentDate.Day ? 0 : 1) :
                                  (x.StartDate.Value.Month > currentDate.Month ? 0 : 1))
                    .ThenBy(x => x.StartDate)
                    .Take(2)
                    .ToList();

                // Diğer çalışanın vardiyalarını
                // Vardiyalar
                var today = DateTime.Today;
                var startOfWeek = today.AddDays(-(int)today.DayOfWeek); // Bu haftanın başlangıcı (Pazartesi)
                var endOfWeek = startOfWeek.AddDays(7); // Bu haftanın sonu (Pazar)
                var shifts = _shiftService.GetAll()
                    .Where(x => x.PersonId == companyManager.Id && x.Status == Status.Active &&
                                    (x.ShiftDate.HasValue && x.ShiftDate.Value.Date >= startOfWeek && x.ShiftDate.Value.Date < endOfWeek))
                        .OrderBy(x => x.ShiftDate.HasValue && x.ShiftDate.Value.Date == today ? 0 : 1)
                        .ThenBy(x => x.ShiftDate)
                        .Take(7)
                        .ToList();
                employeeVM.Shifts = shifts;
                employeeVM.OtherEmployeesPermissions = otherEmployeesPermissions;
                employeeVM.Persons = persons;
                employeeVM.IndexItems.Add(indexItem);

                if (employee.CompanyEmail != company.SuperManagerEmail)
                {
                    employeeListVM.Add(employeeVM);
                }
            }

            return View(employeeListVM);
        }
        private async Task<List<string>> GetUserRolesAsync(string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToList();
        }

    }
}
