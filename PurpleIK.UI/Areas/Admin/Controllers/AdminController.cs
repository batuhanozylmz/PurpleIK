using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PurpleIK.Core.Enums;
using PurpleIK.Entities;
using PurpleIK.Services.Concretes;
using PurpleIK.Services.Interfaces;
using PurpleIK.UI.Areas.Admin.Models.VM.AdminVM;
using PurpleIK.UI.Areas.Employee.Models.VM.EmployeeVM;
using PurpleIK.UI.Utility;
using System;
using System.ComponentModel.Design;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace PurpleIK.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ICompanyService _companyService;
        private readonly IPersonService _personService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ICompanyMembershipService _companyMembership;
        public AdminController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ICompanyService companyService, IPersonService personService, IMapper mapper, IConfiguration configuration, ICompanyMembershipService companyMembership)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _companyService = companyService;
            _personService = personService;
            _mapper = mapper;
            _configuration = configuration;
            _companyMembership = companyMembership;
        }
        public async Task<IActionResult> Home()
        {
            // Giriş yapmış kullanıcının adını al
            var loggedInUsername = HttpContext.User.Identity.Name;
            var user = await _userManager.FindByNameAsync(loggedInUsername);
            Match match = Regex.Match(user.Email, @"@(\w+)\.");

            AdminIndexVM adminIndexVM = new AdminIndexVM();

            // Aktif ve pasif şirketlerin listelerini al
            var activeCompanys = _companyService.GetAll()
                .Where(x => x.Status == Status.Active)
                .ToList();
            foreach (var company in activeCompanys)
            {
                var activeEmployeesCount = _personService.GetAll()
                    .Count(person => person.CompanyId == company.Id && person.Status == Status.Active);

                // Her bir şirketin NumberOfEmployees özelliğine çalışan sayısını atayın
                company.NumberOfEmployees = activeEmployeesCount;
            }
            adminIndexVM.ActiveCompanys = activeCompanys;

            var deActiveCompanys = _companyService.GetAll()
                .Where(x => x.Status == Status.DeActive)
                .ToList();
            adminIndexVM.DeActiveCompanys = deActiveCompanys;

            var approvalCompanys = _companyService.GetAll()
                .Where(x => x.Status == Status.Approval)
                .ToList();
            adminIndexVM.ApprovalCompanys = approvalCompanys;

            // Şirket yöneticileri rollerini çek
            var companyAdmins = await _userManager.GetUsersInRoleAsync("companymanager");

            // Aktif ve pasif şirket yöneticilerini ayır
            adminIndexVM.ActiveCompanyAdmins = companyAdmins
                                                .Where(u => u.Status == Status.Active && u.Persons.Any(p => p.Status == Status.Active &&
                                    p.Company.Status == Status.Active))
                                                .ToList();
            adminIndexVM.DeactiveCompanyAdmins = companyAdmins
                                                .Where(u => u.Status == Status.DeActive && u.Persons.Any(p => p.Status == Status.DeActive &&
                                    p.Company.Status == Status.DeActive))
                                                .ToList();

            // Şirket yöneticileri rollerini çek
            var employees = await _userManager.GetUsersInRoleAsync("employee");

            // Aktif ve pasif şirket yöneticilerini ayır
            adminIndexVM.ActiveEmployees = employees.Where(u => u.Status == Status.Active && u.Persons.Any(p => p.Status == Status.Active &&
                                    p.Company.Status == Status.Active))
                                                .ToList();
            adminIndexVM.DeactiveEmployees = employees.Where(u => u.Status == Status.DeActive && u.Persons.Any(p => p.Status == Status.DeActive &&
                                    p.Company.Status == Status.DeActive))
                                                .ToList();

            var companyMemberships = _companyMembership.GetAll()
                                    .Where(cm => cm.Status == Status.Active)
                                    .GroupBy(cm => cm.CompanyName) // Şirket adına göre grupla
                                    .Select(group => group.OrderByDescending(cm => cm.ExpiryDate).First()) // Her gruptan en son bitiş tarihine sahip olanı seç
                                    .OrderBy(x => x.ExpiryDate.HasValue && x.ExpiryDate.Value.Month == DateTime.Now.Month ? (x.ExpiryDate.Value.Day >= DateTime.Now.Day ? 0 : 1) : (x.ExpiryDate.Value.Month > DateTime.Now.Month ? 0 : 1))
                                    .ThenBy(x => x.ExpiryDate)
                                    .ToList();

            adminIndexVM.CompanyMemberships = companyMemberships;

            
            // View'a AdminIndexVM modelini gönder
            return View("Home", adminIndexVM);
        }

        public IActionResult ShowAlert()
        {
            ViewBag.Message = TempData["message"];
            ViewBag.AlertType = TempData["alertType"];
            return View("ShowAlert");
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AdminAddVM admin)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    

                    AppUser user = new AppUser { UserName = admin.AdminName+admin.AdminLastName, Email = admin.Email };                   
                    user.TwoFactorEnabled = false;
                    user.EmailConfirmed = true;
                    user.CreatedDate = DateTime.Now;
                    user.Status = Status.Active;
                    var result = await _userManager.CreateAsync(user, admin.Password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "admin");                  
                        // Kullanıcıya şifresini mail olarak gönderme
                        string mailBody = $"Admin Giriş Bilgileriniz:<br>Kullanıcı Adı: {admin.Email}<br>Şifre: {admin.Password}";
                        MailHelper.SendMail(admin.Email, "Kullanıcı Bilgilendirme Maili", mailBody, null, null);
                        
                        TempData["successMessage"] = "Admin başarılı bir şekilde eklendi";
                        return RedirectToAction("Home");
                    }
                }
                catch (Exception ex)
                {
                    TempData["message"] = $"Bir Hata Oluştu: {ex.Message}";
                    TempData["alertType"] = "error";
                }
            }

            TempData["errorMessage"] = "Ekleme sırasında bir hata oluştu";
            return RedirectToAction("Add");
        }

        [HttpGet]
        public IActionResult CompanyEdit(Guid id)
        {
            Company c = _companyService.Find(id);
            CompanyEditVM company = _mapper.Map<CompanyEditVM>(c);
            return View(company);
        }

        [HttpPost]
        public IActionResult CompanyEdit(CompanyEditVM company)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Company c = _companyService.Find(company.Id);
                    c.CompanyName = company.CompanyName;
                    c.MersisNo = company.MersisNo;
                    c.TaxNo = company.TaxNo;
                    c.PhoneNumber = company.PhoneNumber;
                    c.Address = company.Address;
                    c.CompanyEmail = company.CompanyEmail;
                    if (Enum.TryParse<CompanyTypes>(company.CompanyTypes, out var companyType))
                    {
                        c.CompanyTypes = companyType;
                    }
                    if (company._Logo != null && company._Logo.Length > 0)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            company._Logo.CopyTo(ms);
                            c.Logo = ms.ToArray();
                        }
                    }
                    if (Enum.TryParse<Status>(company.Status, out var status))
                    {
                        c.Status = status;
                    }
                    c.ModifiedDate = DateTime.Now;
                    _companyService.Edit(c);

                    return RedirectToAction("GetAllActiveCompanies");
                }
                catch (Exception ex)
                {

                }
            }
            return RedirectToAction("GetAllActiveCompanies");

        }

        public async Task<IActionResult> GetAllActiveCompanies()
        {
            var companies = _companyService.GetAll().Where(x => x.Status == Status.Active);

            List<CompanyListVM> companyListVM = new List<CompanyListVM>();
            if (companies != null)
            {
                foreach (var item in companies)
                {
                    CompanyListVM companyVM = _mapper.Map<CompanyListVM>(item);
                    companyVM.CompanyId = item.Id;
                    companyVM.CompanyName = item.CompanyName;
                    companyVM.Email = item.CompanyEmail;
                    companyVM.NumberOfEmployee = item.NumberOfEmployees;
                    companyVM.Status = item.Status.Value;                

                    companyListVM.Add(companyVM);
                }
                return View(companyListVM);
            }
            else
            {
                TempData["message"] = "Görüntülenecek Aktif Şirket Bulunamadı.";
                TempData["alertType"] = "error";
            }

            return View();
        }
        public async Task<IActionResult> GetAllApprovalCompanies()
        {
            var companies = _companyService.GetAll().Where(x => x.Status == Status.Approval);
           
            List<CompanyListVM> companyListVM = new List<CompanyListVM>();
            if (companies != null)
            {
              foreach (var item in companies)
                {
                    CompanyListVM companyVM = _mapper.Map<CompanyListVM>(item);
                    companyVM.CompanyId = item.Id;                    
                    companyVM.CompanyName = item.CompanyName;
                    companyVM.Email = item.CompanyEmail;
                    companyVM.Status = item.Status.Value;
                    if (item.Status == Status.Approval)
                    {
                        companyVM.StatusText = "Onay Bekliyor";
                    }

                    companyListVM.Add(companyVM);
                }
              return View(companyListVM);
            }
            else
            {
                TempData["message"] = "Onaylanacak Şirket Bulunamadı.";
                TempData["alertType"] = "error";
            }
            
            return View();
        }
        

        [HttpPost]
        public async Task<IActionResult> ApproveMembership(Guid companyId)
        {
            var employee = _personService.GetByEntity(x => x.CompanyId == companyId);
            var company = _companyService.Find(companyId);
            // Üyeliği onayla
            company.Status = Status.Active;                     
            
            _companyService.Edit(company);
            var employees = _personService.GetAll().Where(x => x.CompanyId == employee.CompanyId);
            foreach (var emp in employees)
            {
                var empUser = await _userManager.FindByIdAsync(emp.AppUserId.ToString());
                if (empUser != null)
                {
                    
                    empUser.Status = Status.Active;
                    emp.Status = Status.Active;
                    _personService.Edit(emp);

                    // E-posta gönderme işlemleri
                    MailHelper.SendMail(emp.PersonalEmail, " Kullanici Deactive Maili", $"Kullanıcınız Yönetici Tarafından Aktifleştirildi. KullanıciAdı: {emp.CompanyEmail} Şifre:123", null, null);


                    
                }
            }

            Company c = _mapper.Map<Company>(company);

            // Mail gönderme işlemi
            // E-posta gönderme işlemleri
            string baseAddress = _configuration.GetSection("baseAddress").Value;
            string mailBody = MailBodyTemplate.MailBodyApprove(c.CompanyName, $"{baseAddress}/Admin/Company/AccountConfirm?user={c.SuperManagerEmail}");
            MailHelper.SendMail(c.CompanyEmail, "Aktivasyon Maili", mailBody, null, null);           

            return RedirectToAction("GetAllApprovalCompanies");

           
        }

        [HttpPost]
        public async Task<IActionResult> RejectMembership(Guid companyId)
        {
            var employee = _personService.GetByEntity(x => x.CompanyId == companyId);
            var company = _companyService.Find(companyId);
            // Üyeliği reddet
            company.Status = Status.DeActive;

            _companyService.Edit(company);
            var employees = _personService.GetAll().Where(x => x.CompanyId == employee.CompanyId);
            foreach (var emp in employees)
            {
                var empUser = await _userManager.FindByIdAsync(emp.AppUserId.ToString());
                if (empUser != null)
                {

                    empUser.Status = Status.Active;
                    emp.Status = Status.Active;
                    _personService.Edit(emp);

                    // E-posta gönderme işlemleri
                    MailHelper.SendMail(emp.PersonalEmail, " Kullanici Deactive Maili", $"Kullanıcınız Yönetici Tarafından Pasifleştirildi.", null, null);

                }
            }

            // Mail gönderme işlemi
            Company c = _mapper.Map<Company>(company);

            // Mail gönderme işlemi
            // E-posta gönderme işlemleri
            string baseAddress = _configuration.GetSection("baseAddress").Value;
            string mailBody = MailBodyTemplate.MailBodyReject(c.CompanyName, $"{baseAddress}/Admin/Company/AccountConfirm?user={c.SuperManagerEmail}");
            MailHelper.SendMail(c.CompanyEmail, "Reddetme Maili", mailBody, null, null);

            return RedirectToAction("GetAllApprovalCompanies");
        }

    }
}
