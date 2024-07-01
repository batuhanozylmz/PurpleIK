using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using PurpleIK.Core.Enums;
using PurpleIK.Entities;
using PurpleIK.Services.Concretes;
using PurpleIK.Services.Interfaces;
using PurpleIK.UI.Areas.Employee.Models.VM.EmployeeVM;
using PurpleIK.UI.Utility;
using System.Security.Claims;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace PurpleIK.UI.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize(Roles = "employee")]
    public class EmployeeController : Controller
    {
        private readonly IPersonService _personService;
        private readonly ICompanyService _companyService;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IPersonPermissionService _personPermissionService;
        private readonly IPublicHolidaysService _publicHolidaysService;
        private readonly IShiftService _shiftService;

        public EmployeeController(UserManager<AppUser> userManager, IPersonService personService, ICompanyService companyService, IMapper mapper, IConfiguration configuration, IPersonPermissionService personPermissionService, IPublicHolidaysService publicHolidaysService, IShiftService shiftService)
        {
            _personService = personService;
            _companyService = companyService;
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;
            _personPermissionService = personPermissionService;
            _publicHolidaysService = publicHolidaysService;
            _shiftService = shiftService;
        }

        public IActionResult Home()
        {
            // Giriş yapmış kullanıcının adını al
            var userEmailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            EmployeeIndexVM employeeIndexVM = new EmployeeIndexVM();
            if (!string.IsNullOrEmpty(userEmailClaim))
            {
                var person = _personService.GetByEntity(x => x.CompanyEmail.ToLower() == userEmailClaim);

                if (person != null)
                {
                    //Giriş yapan personelin izinleri
                    DateTime TotalDays = DateTime.Now;
                    var personPermissions = _personPermissionService.GetAll()
                     .Where(x => x.PersonId == person.Id && x.Status == Status.Active)
                     .OrderBy(x => x.StartDate.HasValue && x.StartDate.Value.Month == DateTime.Now.Month ? (x.StartDate.Value.Day >= DateTime.Now.Day ? 0 : 1) : (x.StartDate.Value.Month > DateTime.Now.Month ? 0 : 1))
                        .ThenBy(x => x.StartDate)
                     .Take(4)
                     .ToList();

                    //Çalışanın şirketindeki diğer personellerin doğum günleri
                    var persons = _personService.GetAll()
                     .Where(x => x.Id != person.Id && x.CompanyId == person.CompanyId && x.Status == Status.Active && x.BirthDate != null)
                      .OrderBy(x => x.BirthDate.HasValue && x.BirthDate.Value.Month == DateTime.Now.Month ? (x.BirthDate.Value.Day >= DateTime.Now.Day ? 0 : 1) : (x.BirthDate.Value.Month > DateTime.Now.Month ? 0 : 1))
                        .ThenBy(x => x.BirthDate)
                     .Take(2)
                     .ToList();

                    // Diğer çalışanların izinlerini de al
                    var otherEmployeesPermissions = _personPermissionService.GetAll()
                        .Where(x => x.PersonId != person.Id && x.Person.CompanyId == person.CompanyId && x.Status == Status.Active)
                        .OrderBy(x => x.StartDate.HasValue && x.StartDate.Value.Month == DateTime.Now.Month ? (x.StartDate.Value.Day >= DateTime.Now.Day ? 0 : 1) : (x.StartDate.Value.Month > DateTime.Now.Month ? 0 : 1))
                        .ThenBy(x => x.StartDate)
                        .Take(2)
                    .ToList();

                    // Resmi Tatiller
                    var publicHolidays = _publicHolidaysService.GetAll()
                        .OrderBy(x => x.Date.HasValue && x.Date.Value.Month == DateTime.Now.Month ? (x.Date.Value.Day >= DateTime.Now.Day ? 0 : 1) : (x.Date.Value.Month > DateTime.Now.Month ? 0 : 1))
                        .ThenBy(x => x.Date)
                        .Take(7)
                    .ToList();

                    // Vardiyalar
                    var today = DateTime.Today;
                    var startOfWeek = today.AddDays(-(int)today.DayOfWeek); // Bu haftanın başlangıcı (Pazartesi)
                    var endOfWeek = startOfWeek.AddDays(7); // Bu haftanın sonu (Pazar)

                    var shifts = _shiftService.GetAll()
                        .Where(x => x.PersonId == person.Id && x.Status == Status.Active &&
                                    (x.ShiftDate.HasValue && x.ShiftDate.Value.Date >= startOfWeek && x.ShiftDate.Value.Date < endOfWeek))
                        .OrderBy(x => x.ShiftDate.HasValue && x.ShiftDate.Value.Date == today ? 0 : 1)
                        .ThenBy(x => x.ShiftDate)
                        .Take(7)
                        .ToList();

                    employeeIndexVM.Shifts = shifts;
                    employeeIndexVM.PublicHolidays = publicHolidays;
                    employeeIndexVM.Person = person;
                    employeeIndexVM.Persons = persons;
                    employeeIndexVM.PersonPermission = personPermissions;
                    employeeIndexVM.OtherEmployeesPermissions = otherEmployeesPermissions;
                }

                return View(employeeIndexVM);
            }

            return View();
        }

    }
}
