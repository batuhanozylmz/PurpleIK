using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using PurpleIK.Core;
using PurpleIK.Core.Enums;
using PurpleIK.Entities;
using PurpleIK.Services.Interfaces;
using PurpleIK.UI.Areas.CompanyManager.Models.VM.EmployeeManagerVM;
using PurpleIK.UI.Utility;
using System;
using System.ComponentModel.Design;
using System.Net;
using System.Security.Claims;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace PurpleIK.UI.Areas.CompanyManager.Controllers
{
    [Area("CompanyManager")]
    [Authorize(Roles = "companymanager")]
    public class EmployeeManagerController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IPersonService _personService;
        private readonly ICompanyService _companyService;
        private readonly UserManager<AppUser> _userManager;

        public EmployeeManagerController(IMapper mapper, IConfiguration configuration, IPersonService personService, ICompanyService companyService, UserManager<AppUser> userManager)
        {
            _mapper = mapper;
            _configuration = configuration;
            _personService = personService;
            _companyService = companyService;
            _userManager = userManager;            
        }
        public async Task<IActionResult> ListEmployee()
        {

            // Giriş yapmış kullanıcının adını al
            var userEmailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var companyManager = _personService.GetByEntity(x => x.CompanyEmail.ToLower() == userEmailClaim);

            var employees = _personService.GetAll().Where(x => x.Company.Id == companyManager.CompanyId);
            var company = _companyService.GetByEntity(x => x.Id == companyManager.CompanyId);

            List<EmployeeListVM> employeeListVM = new List<EmployeeListVM>();

            if (employees != null)
            {
                foreach (var item in employees)
                {
                    EmployeeListVM employeeVM = _mapper.Map<EmployeeListVM>(item);
                    employeeVM.BirhtPlace = item.BirhtPlace;
                    employeeVM.CompanyName = item.Company.CompanyName;
                    employeeVM.Department = item.Department.GetDisplayName();
                    employeeVM.Gender = item.Gender.GetDisplayName();
                    employeeVM.Status = item.Status.GetDisplayName();
                    ViewBag.CompanyName = item.Company.CompanyName;
                    // Çalışanın rollerini al
                    var user = await _userManager.FindByEmailAsync(item.CompanyEmail);
                    var roles = await _userManager.GetRolesAsync(user);
                    // Eğer çalışanın rolleri varsa, ilk rolü al
                    if (roles.Any())
                    {
                        employeeVM.Role = roles.First();
                    }
                    else
                    {
                        employeeVM.Role = "employee"; // Eğer rolleri yoksa varsayılan olarak "employee" rolünü ata
                    }

                    if (item.ProfilePhoto != null)
                    {
                        employeeVM.Picture = $"data:image/png;base64,{Convert.ToBase64String(item.ProfilePhoto)}";
                    }
                    if (item.CompanyEmail==company.SuperManagerEmail)
                    {
                        
                    }
                    else
                    {
                        employeeListVM.Add(employeeVM);
                    }                  
                }
                return View(employeeListVM);
            }
            return View();
        }
        public async Task<IActionResult> Detail(Guid id)
        {
            // Çalışanı id'ye göre al
            var employee = _personService.GetByEntity(x => x.Id == id);

            if (employee == null)
            {
                return NotFound(); // Çalışan bulunamazsa 404 hatası döndür
            }

            // Görünüm modelini oluştur
            var employeeDetailVM = _mapper.Map<EmployeeListVM>(employee);
            employeeDetailVM.CompanyName = employee.Company.CompanyName;
            employeeDetailVM.Department = employee.Department.GetDisplayName();
            employeeDetailVM.Gender = employee.Gender.GetDisplayName();
            employeeDetailVM.Status = employee.Status.GetDisplayName();
            
            // Profil resmi varsa, base64 formatına dönüştürerek görünüm modeline ekle
            if (employee.ProfilePhoto != null)
            {
                employeeDetailVM.Picture = $"data:image/png;base64,{Convert.ToBase64String(employee.ProfilePhoto)}";
            }

            return View(employeeDetailVM); // Görünümü döndür
        }

        [HttpGet]
        public IActionResult EmployeeAdd()
        {
            // Boş bir PersonAddVM oluşturup view'a gönder
            var model = new EmployeeAddVM();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmployeeAdd(EmployeeAddVM model)
        {
            if (ModelState.IsValid)
            {
                // Model valid, Person entity'sini oluştur
                Person person = _mapper.Map<Person>(model);
                person.BirhtPlace = model.BirthPlace;
                using (var ms = new MemoryStream())
                {
                    model.Picture_.CopyTo(ms);
                    person.ProfilePhoto = ms.ToArray();
                }
                person.Status = Status.Active;

                // Company bilgisini veritabanından al
                var userEmailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var compamyManager = _personService.GetByEntity(x => x.CompanyEmail.ToLower() == userEmailClaim);
                var company = _companyService.GetByEntity(x => x.Id == compamyManager.CompanyId);
                if (company != null)
                {
                    person.Company = company;
                    person.CompanyId = company.Id;
                }

                // AppUser oluştur
                AppUser user = new AppUser();

                string email = CreateUserInformation.CompanyMailNameCreate(company.CompanyName, person.FirstName, person.LastName);
                string userName = CreateUserInformation.UserNameCreate(person.FirstName, person.LastName);
                var findUser = await _userManager.FindByEmailAsync(email);

                if (findUser != null)
                {
                    email = CreateUserInformation.CompanyMailNameCreate(company.CompanyName, person.LastName, person.FirstName);
                    userName = CreateUserInformation.UserNameCreate(person.LastName, person.FirstName);
                }
                person.CompanyEmail = email;
                user.UserName = userName;
                user.Email = person.CompanyEmail;
                user.PhoneNumber = person.PersonalPhoneNumber;
                user.TwoFactorEnabled = false;
                user.EmailConfirmed = true;
                user.CreatedDate = DateTime.Now;
                user.Status = Status.Active;
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result != null)
                {
                    // Seçilen role göre işlem yap
                    switch (model.SelectedRole)
                    {
                        case "admin":
                            await _userManager.AddToRoleAsync(user, "admin");
                            break;
                        case "companymanager":
                            await _userManager.AddToRoleAsync(user, "companymanager");
                            break;
                        case "employee":
                            await _userManager.AddToRoleAsync(user, "employee");
                            break;
                        default:
                            break;
                    }

                    person.AppUser = user;
                    person.AppUserId = user.Id;
                }

                // Person nesnesini veritabanına ekle
                _personService.Add(person);

                var resultMembership = CheckMembershipLimits(compamyManager.CompanyEmail, "companymanager");
                if(resultMembership == false)
                {
                    TempData["MembershipErrorMessage"] = "Çalışan sayınız aktif limitinizin üstüne çıkmıştır. Lütfen çalışan sayınıza uygun üyelik seçiniz";
                    return RedirectToAction("MembershipList", "CompanyMembership", new { area = "CompanyManager" });
                }

                // E-posta gönderme işlemleri
                string mailBody = $"KullanıciAdı: {person.CompanyEmail} Şifre:123";
                MailHelper.SendMail(person.PersonalEmail, " Kullanici Ekleme  Maili", mailBody, null, null);

                // Ekleme başarılıysa başka bir ekrana yönlendir veya işlemi tamamla

                TempData["SuccessMessage"] = "Personel Başarıyla Eklendi.";
                return RedirectToAction("ListEmployee");
            }

            // ModelState geçersiz, view'a aynı modeli geri gönder
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> EmployeeEdit(Guid id)
        {
            var employee = _personService.Find(id);
            if (employee == null)
            {
                TempData["ErrorMessage"] = "Personel Bulunamadı.";
                return RedirectToAction("ListEmployee");
            }
            EmployeeEditVM model = _mapper.Map<EmployeeEditVM>(employee);
            if (employee.ProfilePhoto != null)
            {
                model.PictureBase64 = $"data:image/png;base64,{Convert.ToBase64String(employee.ProfilePhoto)}";
            }
            model.CompanyName = employee.Company.CompanyName;
            model.BirthPlace = employee.BirhtPlace;
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EmployeeEdit(EmployeeEditVM vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var employee = _personService.Find(vm.Id);

                    // Include metodu kullanarak ilişkili verileri alın
                    var user = _userManager.Users.FirstOrDefault(u => u.Id == employee.AppUserId);

                    // Person özelliklerini güncelle
                    employee.Gender = vm.Gender;
                    employee.FirstName = vm.FirstName ?? employee.FirstName;
                    employee.LastName = vm.LastName ?? employee.LastName;
                    employee.BirthDate = vm.BirthDate ?? employee.BirthDate;
                    employee.BirhtPlace = vm.BirthPlace ?? employee.BirhtPlace;
                    employee.CitizenId = vm.CitizenId ?? employee.CitizenId;
                    employee.StartDate = vm.StartDate ?? employee.StartDate;
                    employee.PersonalEmail = vm.PersonalEmail ?? employee.PersonalEmail;
                    employee.Address = vm.Address ?? employee.Address;
                    employee.PersonalPhoneNumber = vm.PersonalPhoneNumber ?? employee.PersonalPhoneNumber;
                    employee.Department = vm.Department ?? employee.Department;
                    employee.ModifiedDate = DateTime.Now;

                    // Eğer yeni resim seçilmemişse, mevcut resmi kullan
                    if (vm.Picture_ != null)
                    {
                        using (var ms = new MemoryStream())
                        {
                            vm.Picture_.CopyTo(ms);
                            employee.ProfilePhoto = ms.ToArray();
                        }
                    }
                    else
                    {
                        employee.ProfilePhoto = employee.ProfilePhoto;
                    }

                    // Company bilgisini veritabanından al
                    var company = _companyService.GetAll().FirstOrDefault(x => x.Id == employee.CompanyId);
                    if (company != null)
                    {
                        employee.Company = company;
                        employee.CompanyId = company.Id;
                    }

                    // AppUser oluştur ve güncelle
                    if (user != null)
                    {
                        string email = CreateUserInformation.CompanyMailNameCreate(company.CompanyName, employee.FirstName, employee.LastName);
                        string userName = CreateUserInformation.UserNameCreate(employee.FirstName, employee.LastName);
                        var findUser = await _userManager.FindByEmailAsync(email);
                        if (employee.CompanyEmail == email)
                        {

                            employee.CompanyEmail = employee.CompanyEmail;
                            user.UserName = user.UserName;
                            user.Email = employee.CompanyEmail;
                        }
                        else
                        {
                            if (findUser!=null)
                            {
                                email = CreateUserInformation.CompanyMailNameCreate(company.CompanyName, employee.LastName, employee.FirstName);
                                userName = CreateUserInformation.UserNameCreate(employee.LastName, employee.FirstName);
                            }
                            employee.CompanyEmail = email;
                            user.UserName = userName;
                            user.Email = employee.CompanyEmail;
                        }
                  
                        user.PhoneNumber = employee.PersonalPhoneNumber;
                        user.TwoFactorEnabled = false;
                        user.EmailConfirmed = true;
                        user.ModifiedDate = DateTime.Now;
                        // Önceki rolleri al
                        var currentRoles = await _userManager.GetRolesAsync(user);

                        // Önceki rolleri güncelle
                        foreach (var role in currentRoles)
                        {
                            await _userManager.RemoveFromRoleAsync(user, role);
                        }

                        // Yeni rolü ekle
                        var newRole = string.IsNullOrEmpty(vm.SelectedRole) ? "employee" : vm.SelectedRole.ToLower();
                        await _userManager.AddToRoleAsync(user, newRole);

                        employee.AppUser = user;
                        employee.AppUserId = user.Id;
                        _personService.Edit(employee);
                    }

                    // E-posta gönderme işlemleri
                    string mailBody = $"KullanıcıAdı: {employee.CompanyEmail} Şifre:{employee.Password}";
                    MailHelper.SendMail(employee.PersonalEmail, " Kullanici Güncelleştirme  Maili", mailBody, null, null);

                    // Başarılıysa başka bir ekrana yönlendir veya işlemi tamamla
                    TempData["SuccessMessage"] = "Personbel Başarıyla Guncellendi.";
                    return RedirectToAction("ListEmployee");
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "Personel Edit İşlemi Başarısız";
                    return View(vm);
                }
            }

            TempData["ErrorMessage"] = "Personel ile ilgili tüm elemanlara veri giriniz";
            return View(vm);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                // Person verisini bul
                var employee = _personService.Find(id);

                // Eğer person verisi bulunamazsa hata mesajı gönder
                if (employee == null)
                {
                    TempData["ErrorMessage"] = "Personel Bulunamadı.";
                    return RedirectToAction("ListEmployee");
                }

                // İlgili person verisine bağlı kullanıcıyı bul
                var user = await _userManager.FindByIdAsync(employee.AppUserId.ToString());

                // Eğer kullanıcı bulunamazsa hata mesajı gönder
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Kullanıcı Bulunamadı.";
                    return RedirectToAction("ListEmployee");
                }
                var company = _companyService.GetByEntity(x => x.Id == employee.CompanyId);
                
                // Süper yönetici kontrolü
                if (IsSuperManager(employee, company))
                {
                    TempData["ErrorMessage"] = "Süper Şirket yönetici olduğu için Silinemez.";
                    return RedirectToAction("ListEmployee");
                }
                else
                {
                    // Person ve ilgili kullanıcıyı sil
                    _personService.Remove(employee);
                    await _userManager.DeleteAsync(user);

                    // Başarı mesajı gönder ve listeleme sayfasına yönlendir
                    TempData["SuccessMessage"] = "Personel Başarıyla Silindi.";
                    return RedirectToAction("ListEmployee");

                }

            }
            catch (Exception ex)
            {
                // Hata durumunda hata mesajı gönder ve listeleme sayfasına yönlendir
                TempData["ErrorMessage"] = "Personel Silinirken Hata Verdi." + ex.Message;
                return RedirectToAction("ListEmployee");
            }
        }
        public async Task<IActionResult> ChangeEmployeeStatus(Guid id, Status newStatus)
        {
            try
            {
                // Person verisini bul
                var employee = _personService.Find(id);

                // Eğer person verisi bulunamazsa hata mesajı gönder
                if (employee == null)
                {
                    TempData["ErrorMessage"] = "Personel Bulunamadı.";
                    return RedirectToAction("ListEmployee");
                }

                // İlgili person verisine bağlı kullanıcıyı bul
                var user = await _userManager.FindByIdAsync(employee.AppUserId.ToString());

                // Eğer kullanıcı bulunamazsa hata mesajı gönder
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Kullanıcı Bulunamadı.";
                    return RedirectToAction("ListEmployee");
                }
                var company = _companyService.GetByEntity(x => x.Id == employee.CompanyId);
                
                // Süper yönetici kontrolü
                if (IsSuperManager(employee, company))
                {
                    TempData["ErrorMessage"] = "Süper Şirket yönetici olduğu için işlem yapılamaz.";
                    return RedirectToAction("ListEmployee");
                }
                else
                {
                    // Person ve ilgili kullanıcıyı belirtilen duruma getir
                    user.Status = newStatus;
                    employee.Status = newStatus;
                    _personService.Edit(employee);

                    // E-posta gönderme işlemleri
                    string mailBody = "";
                    if (newStatus == Status.Active)
                    {
                        mailBody = $"Kullanıcınız Yönetici Tarafından Aktifleştirildi";
                    }
                    else if (newStatus == Status.DeActive)
                    {
                        mailBody = $"Kullanıcınız Yönetici Tarafından Pasifleştirildi";
                    }
                    MailHelper.SendMail(employee.PersonalEmail, " Kullanici " + newStatus + " Maili", mailBody, null, null);

                    // Başarılıysa başka bir ekrana yönlendir veya işlemi tamamla
                    TempData["SuccessMessage"] = "İşlem Başarıyla gerçekleşti.";
                    return RedirectToAction("ListEmployee");

                }

            }
            catch (Exception ex)
            {
                // Hata durumunda hata mesajı gönder ve listeleme sayfasına yönlendir
                TempData["ErrorMessage"] = "İşlem Başarısız oldu.." + " " + ex.Message;
                return RedirectToAction("ListEmployee");
            }
        }
        private bool IsSuperManager(Person employee, Company company)
        {
            return company.SuperManagerEmail == employee.CompanyEmail &&
                   company.SuperManagerFirstName.ToLower() == employee.FirstName.ToLower() &&
                   company.SuperManagerLastName.ToLower() == employee.LastName.ToLower();
        }
        public bool CheckMembershipLimits(string email, string role)
        {
            // Email'e göre ilgili şirketin üyelik durumunu kontrol et
            if (role == "admin")
            {
                return true;
            }
            var person = _personService.GetByEntity(p => p.CompanyEmail == email);
            var company = _companyService.GetByEntity(c => c.CompanyName == person.Company.CompanyName);
            var employees = _personService.GetAll().Where(e => e.CompanyId == company.Id);
            if (person != null)
            {
                // Burada üyelik durumu kontrol edilir.
                if (employees.Count() > person.Company.CompanyMemberships.NumberOfEmployee)
                {
                    MakeStatusPassive(person.Company.Id);
                    return false;

                }
                else if (DateTime.Now < person.Company.CompanyMemberships.ExpiryDate)
                {
                    person.Company.Status = Status.Active;
                    return true;
                }

            }
            return false;
        }
        public bool IsMembershipActive(string email, string role)
        {

            // Email'e göre ilgili şirketin üyelik durumunu kontrol et
            if (role == "admin")
            {
                return true;
            }
            var person = _personService.GetByEntity(p => p.CompanyEmail == email);
            var company = _companyService.GetByEntity(c => c.CompanyName == person.Company.CompanyName);
            var employees = _personService.GetAll().Where(e => e.CompanyId == company.Id);
            if (person != null)
            {
                // Burada üyelik durumu kontrol edilir.                
                if (DateTime.Now > person.Company.CompanyMemberships.ExpiryDate)
                {
                    MakeStatusPassive(person.Company.Id);
                }
                else if (DateTime.Now < person.Company.CompanyMemberships.ExpiryDate)
                {
                    person.Company.Status = Status.Active;
                    return true;
                }

            }
            return false;
        }
        public void MakeStatusPassive(Guid companyId)
        {

            var company = _companyService.Find(companyId);
            company.CompanyMemberships.Status = Status.DeActive;
            company.Status = Status.DeActive;
            _companyService.Edit(company);
        }

    }

}
