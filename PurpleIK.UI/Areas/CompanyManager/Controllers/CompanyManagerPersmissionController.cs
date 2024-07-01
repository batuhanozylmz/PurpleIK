using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PurpleIK.Core.Enums;
using PurpleIK.Entities;
using PurpleIK.Services.Concretes;
using PurpleIK.Services.Interfaces;
using PurpleIK.UI.Areas.CompanyManager.Models.VM.PermissionVM;
using System.Security.Claims;

namespace PurpleIK.UI.Areas.CompanyManager.Controllers
{
    [Area("CompanyManager")]
    [Authorize(Roles = "companymanager")]
    public class CompanyManagerPersmissionController : Controller
    {
        private readonly IPersonService _personService;
        private readonly ICompanyService _companyService;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IPersonPermissionService _personPermissionService;
        private readonly IPermissionService _permissionService;
        public CompanyManagerPersmissionController(UserManager<AppUser> userManager, IPersonService personService, ICompanyService companyService, IMapper mapper, IConfiguration configuration, SignInManager<AppUser> signInManager, IPersonPermissionService personPermissionService, IPermissionService permissionService)
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
            // Giriş yapmış kullanıcının adını al
            var userEmailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var companyManager = _personService.GetByEntity(x => x.CompanyEmail.ToLower() == userEmailClaim);
            List<ManagerPermissionIndexVM> vm = new();

            var personPermissionsList = _personPermissionService.GetAll().Where(x => x.Person.CompanyId == companyManager.CompanyId).ToList();

            foreach (var item in personPermissionsList)
            {
                ManagerPermissionIndexVM personPermission = _mapper.Map<ManagerPermissionIndexVM>(item);
                personPermission.Status = item.Status.Value.ToString();

                //kaydının sahibi olan çalışanı bul
                var employee = _personService.GetAll()
                    .FirstOrDefault(x => x.Id == item.PersonId && x.Company.Id == companyManager.CompanyId && x.Status == Status.Active);

                personPermission.Employee = employee != null ? $"{employee.FirstName} {employee.LastName}" : "";
                personPermission.Status = item.Status.ToString();

                // Çalışanın rollerini al
                var user = await _userManager.FindByEmailAsync(employee.CompanyEmail);
                var roles = await _userManager.GetRolesAsync(user);
                // Eğer çalışanın rolleri varsa, ilk rolü al
                if (roles.Any())
                {
                    personPermission.Role = roles.First();
                }
                else
                {
                    personPermission.Role = "employee"; // Eğer rolleri yoksa varsayılan olarak "employee" rolünü ata
                }
                if (employee.ProfilePhoto != null)
                {
                    personPermission.Picture = $"data:image/png;base64,{Convert.ToBase64String(employee.ProfilePhoto)}";
                }
                vm.Add(personPermission);
            }
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var vm = new ManagerPermissionAddVM();
            {
                vm.Permissions = _permissionService.GetAll();
            };
            // Giriş yapmış kullanıcının adını al
            var userEmailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var companyManager = _personService.GetByEntity(x => x.CompanyEmail.ToLower() == userEmailClaim);

            var persons = _personService.GetAll()
                .Where(x => x.Company.Id == companyManager.CompanyId && x.Status == Status.Active && x.AppUserId != null).ToList();
            vm.Persons = persons;
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ManagerPermissionAddVM vm)
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
                PersonPermission p = new();
                p.StartDate = vm.ManagerPermissionAddItem.StartDate;
                p.EndDate = vm.ManagerPermissionAddItem.EndDate;
                p.NumberOfDays = vm.ManagerPermissionAddItem.NumberOfDays;
                p.CreatedDate = DateTime.Now;
                p.Status = Status.Active;
                p.CompanyManagerName = $"{companyManager.FirstName} {companyManager.LastName}";
                p.CompanyManagerEmail = companyManager.CompanyEmail;
                p.DateOfReply = DateTime.Now;

                if (vm.ManagerPermissionAddItem.PermissionFile?.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await vm.ManagerPermissionAddItem.PermissionFile.CopyToAsync(ms);
                        p.PermissionFile = ms.ToArray();
                    }
                }


                var person = _personService.GetByEntity(x => x.Id == vm.ManagerPermissionAddItem.PersonId);

                p.PersonId = person.Id;
                p.Person = person;

                var permissiontype = _permissionService.GetByEntity(x => x.Id == vm.ManagerPermissionAddItem.PermissionId);
                p.Permission = permissiontype;
                p.PermissionId = permissiontype.Id;
                _personPermissionService.Add(p);

                TempData["SuccessMessage"] = "İzin Başarıyla Oluşturuldu.";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "İzin İşlemi Başarısız Oldu";
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

            ManagerPermissionEditVM vm = new ManagerPermissionEditVM();
            if (vm.ManagerPermissionEditItem == null)
            {
                vm.ManagerPermissionEditItem = new ManagerPermissionEditItem(); // ManagerPermissionEditItem null ise yeni bir nesne oluştur
            }
            vm.ManagerPermissionEditItem.Id = personPermission.Id;
            vm.ManagerPermissionEditItem.StartDate = personPermission.StartDate;
            vm.ManagerPermissionEditItem.EndDate = personPermission.EndDate;
            vm.ManagerPermissionEditItem.NumberOfDays = personPermission.NumberOfDays;
            vm.ManagerPermissionEditItem.ReasonOfRejection = personPermission.ReasonOfRejection;
            if (personPermission.Status == Status.Active)
            {
                vm.ManagerPermissionEditItem.Status = "Onaylı";
            }
            if (personPermission.Status == Status.DeActive)
            {
                vm.ManagerPermissionEditItem.Status = "Red Edilmiş";
            }
            if (personPermission.Status == Status.Approval)
            {
                vm.ManagerPermissionEditItem.Status = "Onay Bekliyor";
            }
            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(ManagerPermissionEditVM vm)
        {

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "İzin Talebi Değişikliği ile ilgili tüm alanlara veri giriniz";
                return View(vm);
            }

            try
            {
                // İzin bul bul
                var personPermission = _personPermissionService.Find(vm.ManagerPermissionEditItem.Id);
                if (personPermission == null)
                {
                    TempData["ErrorMessage"] = "İzin Bulunamadı";
                    return RedirectToAction("Index");
                }
                personPermission.StartDate = vm.ManagerPermissionEditItem.StartDate;
                personPermission.EndDate = vm.ManagerPermissionEditItem.EndDate;
                personPermission.NumberOfDays = vm.ManagerPermissionEditItem.NumberOfDays;
                personPermission.ModifiedDate = DateTime.Now;
                if (vm.ManagerPermissionEditItem.PermissionFile != null && vm.ManagerPermissionEditItem.PermissionFile.Length > 0)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        vm.ManagerPermissionEditItem.PermissionFile.CopyTo(ms);
                        personPermission.PermissionFile = ms.ToArray();
                    }
                }
                personPermission.ReasonOfRejection = vm.ManagerPermissionEditItem.ReasonOfRejection;
                personPermission.DateOfReply = DateTime.Now;
                if (vm.ManagerPermissionEditItem.ApprovalStatus != null)
                {
                    if (vm.ManagerPermissionEditItem.ApprovalStatus == "Onay Bekliyor")
                    {
                        personPermission.Status = Status.Approval;
                    }
                    else if (vm.ManagerPermissionEditItem.ApprovalStatus == "Onayla")
                    {
                        personPermission.Status = Status.Active;
                    }
                    else if (vm.ManagerPermissionEditItem.ApprovalStatus == "Red Et")
                    {
                        personPermission.Status = Status.DeActive;
                    }
                }
                // Giriş yapmış kullanıcının adını al
                var userEmailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var companyManager = _personService.GetByEntity(x => x.CompanyEmail.ToLower() == userEmailClaim);
                personPermission.CompanyManagerName = $"{companyManager.FirstName} {companyManager.LastName}";
                personPermission.CompanyManagerEmail = companyManager.CompanyEmail;
                _personPermissionService.Edit(personPermission);

                TempData["SuccessMessage"] = "İzin Başarıyla Güncellendi.";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "İzin İşlemi Başarısız Oldu";
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
            ManagerPermissionIndexVM vm = _mapper.Map<ManagerPermissionIndexVM>(personPermission);


            // Giriş yapmış kullanıcının adını al
            var userEmailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var companyManager = _personService.GetByEntity(x => x.CompanyEmail.ToLower() == userEmailClaim);

            var employee = _personService.GetAll()
                   .FirstOrDefault(x => x.Id == personPermission.PersonId && x.Company.Id == companyManager.CompanyId && x.Status == Status.Active);

            vm.Employee = employee != null ? $"{employee.FirstName} {employee.LastName}" : "";

            // Çalışanın rollerini al
            var user = await _userManager.FindByEmailAsync(employee.CompanyEmail);
            var roles = await _userManager.GetRolesAsync(user);
            // Eğer çalışanın rolleri varsa, ilk rolü al
            if (roles.Any())
            {
                vm.Role = roles.First();
            }
            else
            {
                vm.Role = "employee"; // Eğer rolleri yoksa varsayılan olarak "employee" rolünü ata
            }
            if (vm.Role != null && vm.Role == "companymanager")
            {
                vm.Role = "Şirket Yöntici";
            }
            else
            {
                vm.Role = "Çalışan";
            }
            return View(vm);
        }
    }
}
