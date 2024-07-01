using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PurpleIK.Core.Enums;
using PurpleIK.Entities;
using PurpleIK.Services.Interfaces;
using PurpleIK.UI.Areas.Employee.Models.VM.EmployeePermision;
using System.Security.Claims;

namespace PurpleIK.UI.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize(Roles = "employee")]
    public class EmployeePermissionController : Controller
    {
        private readonly IPersonService _personService;
        private readonly ICompanyService _companyService;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IPersonPermissionService _personPermissionService;
        private readonly IPermissionService _permissionService;
        public EmployeePermissionController(UserManager<AppUser> userManager, IPersonService personService, ICompanyService companyService, IMapper mapper, IConfiguration configuration, SignInManager<AppUser> signInManager, IPersonPermissionService personPermissionService, IPermissionService permissionService)
        {
            _personService = personService;
            _companyService = companyService;
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;
            _signInManager = signInManager;
            _personPermissionService = personPermissionService;
            _permissionService = permissionService;
        }
        public async Task<IActionResult> Index()
        {
            var userEmailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var person = _personService.GetByEntity(x => x.CompanyEmail == userEmailClaim);
            List<EmployeePermissionsListVM> vm = new List<EmployeePermissionsListVM>();

            var personPermissionList = _personPermissionService.GetAll().Where(x => x.PersonId == person.Id);
            if (personPermissionList != null)
            {
                foreach (var item in personPermissionList)
                {
                    EmployeePermissionsListVM personPermissions = _mapper.Map<EmployeePermissionsListVM>(item);
                    personPermissions.Status = item.Status.Value.ToString();

                    vm.Add(personPermissions);
                }
                return View(vm);
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var vm = new EmployeePermissionsAddVM();
            {
                vm.Permissions = _permissionService.GetAll();
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Add(EmployeePermissionsAddVM vm)
        {

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "İzin Talebi ile ilgili tüm alanlara veri giriniz";
                return View(vm);
            }

            try
            {
                PersonPermission p = new();
                p.StartDate = vm.EmployeePermissionsAddItem.StartDate;
                p.EndDate = vm.EmployeePermissionsAddItem.EndDate;
                p.NumberOfDays = vm.EmployeePermissionsAddItem.NumberOfDays;
                p.CreatedDate = DateTime.Now;
                p.Status = Status.Approval;

                if (vm.EmployeePermissionsAddItem.PermissionFile?.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await vm.EmployeePermissionsAddItem.PermissionFile.CopyToAsync(ms);
                        p.PermissionFile = ms.ToArray();
                    }
                }

                var userEmailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var person = _personService.GetByEntity(x => x.CompanyEmail == userEmailClaim);

                p.PersonId = person.Id;
                p.Person = person;

                var permissiontype = _permissionService.GetByEntity(x => x.Id == vm.EmployeePermissionsAddItem.PermissionId);
                p.Permission = permissiontype;
                p.PermissionId = permissiontype.Id;
                _personPermissionService.Add(p);

                TempData["SuccessMessage"] = "İzin Talebi Başarıyla Oluşturuldu.";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "İzin Talebi İşlemi Başarısız Oldu";
                return View(vm);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var personPermission = _personPermissionService.Find(id);
            if (personPermission == null)
            {
                TempData["ErrorMessage"] = "İzin Bulunamadı";
                return RedirectToAction("Index");
            }
            EmployeePermissionsEditVM vm = new EmployeePermissionsEditVM();
            vm.Id = personPermission.Id;
            vm.StartDate = personPermission.StartDate;
            vm.EndDate = personPermission.EndDate;
            vm.NumberOfDays = personPermission.NumberOfDays;
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EmployeePermissionsEditVM vm)
        {

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "İzin Talebi Değişikliği ile ilgili tüm alanlara veri giriniz";
                return View(vm);
            }

            try
            {
                // İzin bul bul
                var personPermission = _personPermissionService.Find(vm.Id);
                if (personPermission == null)
                {
                    TempData["ErrorMessage"] = "İzin Bulunamadı";
                    return RedirectToAction("Index");
                }
                personPermission.StartDate = vm.StartDate;
                personPermission.EndDate = vm.EndDate;
                personPermission.NumberOfDays = vm.NumberOfDays;
                personPermission.ModifiedDate = DateTime.Now;
                if (vm.PermissionFile != null && vm.PermissionFile.Length > 0)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        vm.PermissionFile.CopyTo(ms);
                        personPermission.PermissionFile = ms.ToArray();
                    }
                }
                _personPermissionService.Edit(personPermission);

                TempData["SuccessMessage"] = "İzin Talebi Başarıyla Güncellendi.";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "İzin Talebi İşlemi Başarısız Oldu";
                return View(vm);
            }
        }
        public IActionResult GetDocument(Guid id, string layoutName)
        {
            var personPermission = _personPermissionService.Find(id);
            if (personPermission == null || personPermission.PermissionFile == null)
            {
                TempData["ErrorMessage"] = "Dosya Bulunamadı";
                if (layoutName == "Edit")
                {
                    return RedirectToAction(layoutName, new { id });
                }
                return RedirectToAction(layoutName);
            }

            return File(personPermission.PermissionFile, "application/pdf");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var personPermission = _personPermissionService.Find(id);
            if (personPermission == null)
            {
                TempData["ErrorMessage"] = "İzin Bulunamadı";
                return RedirectToAction("Index");
            }
            _personPermissionService.Remove(personPermission);
            TempData["SuccessMessage"] = "İzin Talebi Başarıyla Silindi.";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            var personPermission = _personPermissionService.Find(id);
            if (personPermission == null)
            {
                TempData["ErrorMessage"] = "İzin Bulunamadı";
                return RedirectToAction("Index");
            }
            EmployeePermissionsListVM vm = _mapper.Map<EmployeePermissionsListVM>(personPermission);

            return View(vm);
        }
    }
}
