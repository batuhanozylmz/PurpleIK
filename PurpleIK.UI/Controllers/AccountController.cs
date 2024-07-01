using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PurpleIK.Core.Enums;
using PurpleIK.Entities;
using PurpleIK.Services.Concretes;
using PurpleIK.Services.Interfaces;
using PurpleIK.UI.Areas.Admin.Models.VM.MembershipVM;
using PurpleIK.UI.Areas.CompanyManager.Models.VM.EmployeeManagerVM;
using PurpleIK.UI.Models.VM.AccountVM;
using PurpleIK.UI.Utility;
using System.Data;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PurpleIK.UI.Controllers
{

    public class AccountController : Controller
    {
        private readonly IPersonService _personService;
        private readonly ICompanyService _companyService;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMembershipService _membershipService;
        private readonly ICompanyMembershipService _companyMembershipService;




        public AccountController(UserManager<AppUser> userManager, IPersonService personService, ICompanyService companyService, IMapper mapper, IConfiguration configuration, SignInManager<AppUser> signInManager, IMembershipService membershipService, ICompanyMembershipService companyMembership)
        {
            _personService = personService;
            _companyService = companyService;
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;
            _signInManager = signInManager;
            _membershipService = membershipService;
            _companyMembershipService = companyMembership;

        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
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
            var company = _companyService.GetByEntity(c=>c.CompanyName == person.Company.CompanyName);
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
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM vm)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(vm.Email);

                if (user != null)
                {                   
                   
                    if (user.Status==Status.Active)
                    {
                        var result = await _signInManager.PasswordSignInAsync(user.UserName, vm.Password, false, false);
                        if (result.Succeeded)
                        {
                            var roles = await _userManager.GetRolesAsync(user);
                            if (roles.Any())
                            {
                                // Kullanıcının rolleri varsa, istediğiniz işlemleri yapabilirsiniz
                                foreach (var role in roles)
                                {
                                    bool isMembershipActive = IsMembershipActive(vm.Email, role);
                                    bool checkMembershipLimits = CheckMembershipLimits(vm.Email, role);

                                    if (role == "employee" && isMembershipActive)
                                    {
                                        return RedirectToAction("Home", "Employee", new { area = "Employee" });


                                    }
                                    else if (role == "companymanager" && isMembershipActive && !checkMembershipLimits)
                                    {
                                        TempData["MembershipErrorMessage"] = "Çalışan Sayınız Aktif üyeliğinizin üstündedir. Lütfen Çalışan sayınıza uygun üyelik seçiniz";
                                        return RedirectToAction("MembershipList", "CompanyMembership", new { area = "CompanyManager" });
                                    }
                                    else if (role == "companymanager" && isMembershipActive)
                                    {

                                        return RedirectToAction("Home", "Company", new { area = "CompanyManager" });
                                    }
                                   

                                    else if (role == "companymanager" && !isMembershipActive)
                                    {
                                        TempData["MembershipErrorMessage"] = "Şirketinizin üyelik süresi dolmuştur. Lütfen üyeliğinizi yenileyin.";
                                        return RedirectToAction("MembershipList", "CompanyMembership", new { area = "CompanyManager" });
                                    }
                                    

                                    else if (role == "admin")
                                    {
                                        return RedirectToAction("Home", "Admin", new { area = "Admin" });
                                    }
                                    else
                                    {
                                        TempData["ErrorMessage"] = "Hesabınız aktif değildir. Şirket üyeliğini yenileyiniz";
                                        return RedirectToAction("Login");
                                    }
                                    
                                }
                            }
                            // Eğer kullanıcı rolleri yoksa veya belirli roller için eşleşme bulunamadıysa buraya düşer
                            return View();
                        }
                    }
                    TempData["ErrorMessage"] = "Hasabınız Aktif Değildir.";
                    return RedirectToAction("Login");
                }

                TempData["ErrorMessage"] = "Geçersiz e-posta veya şifre.";
            }
            return View(vm);
        }
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index","Home");
        }
        [HttpGet]
        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM vm)
        {
            if (ModelState.IsValid)
            {
                var person = _personService.GetByEntity(x =>
                    x.PersonalEmail.ToLower() == vm.Email.ToLower() &&
                    x.FirstName.ToLower() == vm.FirstName.ToLower() &&
                    x.LastName.ToLower() == vm.LastName.ToLower() &&
                    x.Status == Status.Active);

                if (person != null)
                {
                    // Kullanıcıya şifresini mail olarak gönderme
                    string mailBody = $"Giriş Bilgileriniz:<br>Kullanıcı Adı: {person.CompanyEmail}<br>Şifre: {person.Password}";

                    MailHelper.SendMail(vm.Email, "Kullanıcı Bilgisi Hatırlatma Maili", mailBody, null, null);

                    TempData["SuccessMessage"] = "Giris Bilgileriniz Mail Olarak Gonderilmistir.";
                    return RedirectToAction("ForgotPassword");
                }
                else
                {
                    TempData["ErrorMessage"] = "Gecersiz e-posta ve kullanici bilgileri";
                    return RedirectToAction("ForgotPassword");
                }
            }
            return View(vm);
        }
        [HttpGet]
        public IActionResult CompanyRegister(Guid planId)
        {
            CompanyRegisterVM vm = new CompanyRegisterVM();
            vm.PlanId = planId;
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> CompanyRegister(CompanyRegisterVM vm)
        {
            if (ModelState.IsValid)
            {
                try
                {                


                    Company company = _mapper.Map<Company>(vm);
                    company.Status = Status.DeActive;                       

                    Person person = new Person();
                    // person.ProfilePhoto =;
                    person.Status = Status.DeActive;
                    person.PersonalPhoneNumber = vm.PhoneNumber;
                    person.Address = vm.Address;
                    person.FirstName = vm.SuperManagerFirstName;
                    person.LastName = vm.SuperManagerLastName;
                    person.PersonalEmail = vm.CompanyEmail;

                    

                    string email = CreateUserInformation.CompanyMailNameCreate(company.CompanyName, person.FirstName, person.LastName);
                    string userName = CreateUserInformation.UserNameCreate(person.FirstName, person.LastName);
                    var findUser = await _userManager.FindByNameAsync(userName);

                    if (findUser != null)
                    {
                        email = CreateUserInformation.CompanyMailNameCreate(company.CompanyName, person.LastName, person.FirstName);
                        userName = CreateUserInformation.UserNameCreate(person.LastName, person.FirstName);
                    }
                    person.CompanyEmail = email;



                    person.Password = vm.SuperManagerPassword;
                    person.Department = Department.Administration;
                    person.Gender = Gender.Other;

                    company.SuperManagerEmail = person.CompanyEmail;                    
                    _companyService.Add(company);

                    person.Company = company;
                    person.CompanyId = company.Id;

                    //Üyelik Tanımla
                    if (vm.PlanId != null)
                    {
                        var membership = _membershipService.GetByEntity(x => x.Id == vm.PlanId);

                        CompanyMembership companyMembership = new CompanyMembership();
                        if (vm.NumberOfEmployees <= membership.NumberOfEmployee)
                        {
                            companyMembership.NumberOfEmployee = vm.NumberOfEmployees;
                        }
                        else
                        {
                            TempData["MembershipsErrorMessage"] = "Seçtiğiniz üyelik şirket çalışanı sınırına uymuyor lütfen uygun üyelik seçiniz";
                            _companyService.Remove(company);
                            return RedirectToAction("Memberships");
                        }
                        companyMembership.Status = Status.Active;
                        companyMembership.CompanyName = vm.CompanyName;
                        companyMembership.Duration = 15;
                        companyMembership.ApplicationDate = DateTime.Now;
                        companyMembership.Price = membership.Price;
                        companyMembership.Name = membership.Name;
                        companyMembership.ExpiryDate = DateTime.Now.AddDays(companyMembership.Duration.Value);
                        companyMembership.CompanyId = company.Id;
                        companyMembership.MembershipId = membership.Id;
                        companyMembership.SubscriptionPeriod = membership.SubscriptionPeriod;
                        _companyMembershipService.Add(companyMembership);
                    }

                    // AppUser oluştur
                    AppUser user = new AppUser();
                    user.UserName = userName;
                    user.Email = person.CompanyEmail;
                    user.PhoneNumber = person.PersonalPhoneNumber;
                    user.TwoFactorEnabled = false;
                    user.EmailConfirmed = false;
                    user.CreatedDate = DateTime.Now;
                    user.Status = Status.DeActive;
                    var result = await _userManager.CreateAsync(user, person.Password);
                    if (result != null)
                    {
                        await _userManager.AddToRoleAsync(user, "companymanager");
                    }

                    person.AppUser = user;
                    person.AppUserId = user.Id;

                    // Person nesnesini veritabanına ekle
                    _personService.Add(person);

                    // E-posta gönderme işlemleri
                    string baseAddress = _configuration.GetSection("baseAddress").Value;
                    string mailBody = MailBodyTemplate.MailBody(company.CompanyName, $"{baseAddress}/Account/AccountConfirm?userEmail={person.CompanyEmail}");
                    MailHelper.SendMail(company.CompanyEmail, "Aktivasyon Maili", mailBody, null, null);

                    TempData["SuccessMessage"] = "Kayit isleminiz tamamlandi. Mail adresinize gelen linke tiklayarak hesabinizi onaylayin.";

                    return RedirectToAction("Login");

                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Bir Hata Olustu: {ex.Message}";
                }
            }

            TempData["ErrorMessage"] = "Bir Hata Olustu";
            return RedirectToAction("CompanyRegister");
        }
        public async Task<IActionResult> AccountConfirm(string userEmail)
        {
            if (!string.IsNullOrWhiteSpace(userEmail))
            {
                try
                {
                    var company = _companyService.GetByEntity(x => x.SuperManagerEmail == userEmail);
                    var appUser = await _userManager.FindByEmailAsync(userEmail);
                    var person = _personService.GetByEntity(x => x.CompanyEmail == userEmail);
                    if (company != null && appUser != null && person != null)
                    {
                        company.Status = Status.Approval;
                        company.ModifiedDate = DateTime.Now;
                        _companyService.Edit(company);

                        person.Status = Status.Approval;
                        person.ModifiedDate = DateTime.Now;
                        _personService.Edit(person);

                        appUser.ModifiedDate = DateTime.Now;
                        appUser.Status = Status.Approval;
                        appUser.EmailConfirmed = true;
                        var result = await _userManager.UpdateAsync(appUser);


                        if (result.Succeeded)
                        {
                            TempData["SuccessMessage"] = "Kullanici Mail Onaylandi.Sisteme giris bilgisi icin mail gonderilecektir. Iyi gunler.";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Kullanici Onaylanirken bir hata olustu.";
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Kullanici bilgisine ulasilamadi";
                    }
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "Kullanici Onaylanmadi";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Gecersiz kullanici bilgisi";
            }

            return RedirectToAction("Login");
        }
        [HttpGet]
        public async Task<IActionResult> PersonalSettings()
        {
            // Giriş yapmış kullanıcının adını al
            var userEmailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var person = _personService.GetByEntity(x => x.CompanyEmail.ToLower() == userEmailClaim);

            var user = await _userManager.FindByEmailAsync(userEmailClaim);
            var roles = await _userManager.GetRolesAsync(user);

            // Eğer çalışanın rolleri varsa, ilk rolü al
            if (roles.Any())
            {
                if (roles.First() == "employee")
                {
                    ViewBag.Layout = "~/Areas/Employee/Views/Shared/_EmployeeLayout.cshtml";
                }
                else if (roles.First() == "companymanager")
                {
                    ViewBag.Layout = "~/Areas/CompanyManager/Views/Shared/_CompanyLayout.cshtml";
                }
                else
                {
                    ViewBag.Layout = null;
                }
            }

            PersonalSettingsVM model = _mapper.Map<PersonalSettingsVM>(person);
            
            if (person.ProfilePhoto != null)
            {
                model.PictureBase64 = $"data:image/png;base64,{Convert.ToBase64String(person.ProfilePhoto)}";
            }
            model.CompanyName = person.Company.CompanyName;
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> PersonalSettings(PersonalSettingsVM vm)
        {
            if (ModelState.IsValid)
            {
                try
                {   //personel bilgisi al
                    var employee = _personService.Find(vm.Id);
                   string employeeCompany= employee.CompanyEmail;

                    // Company bilgisini veritabanından al
                    var company = _companyService.GetAll().FirstOrDefault(x => x.Id == employee.CompanyId);
                    string companySuperManagerEmail= company.SuperManagerEmail;

                    // Include metodu kullanarak ilişkili verileri alın
                    var user = _userManager.Users.FirstOrDefault(u => u.Id == employee.AppUserId);



                    // Person özelliklerini güncelle
                    employee.FirstName = vm.FirstName ?? employee.FirstName;
                    employee.LastName = vm.LastName ?? employee.LastName;
                    employee.PersonalEmail = vm.PersonalEmail ?? employee.PersonalEmail;
                    employee.Address = vm.Address ?? employee.Address;
                    employee.PersonalPhoneNumber = vm.PersonalPhoneNumber ?? employee.PersonalPhoneNumber;
                    employee.Department = vm.Department ?? employee.Department;
                    employee.ModifiedDate = DateTime.Now;
                    employee.Password = vm.Password ?? employee.Password;

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

                    

                    // AppUser oluştur ve güncelle
                    if (user != null)
                    {
                        string email = CreateUserInformation.CompanyMailNameCreate(company.CompanyName, employee.FirstName, employee.LastName);
                        string userName = CreateUserInformation.UserNameCreate(employee.FirstName, employee.LastName);
                        var findUser = await _userManager.FindByEmailAsync(email);

                        // Yeni şifre hash'ini oluştur
                        var newPasswordHash = _userManager.PasswordHasher.HashPassword(user, vm.Password);

                        // Şifrenin geçerliliğini doğrula
                        var passwordValidator = new PasswordValidator<AppUser>();
                        var validationResult = await passwordValidator.ValidateAsync(_userManager, user, vm.Password);
                        if (validationResult.Succeeded)
                        {
                            // Eğer şifre geçerliyse, eski şifre hash'ini yeni şifre hash'iyle değiştir
                            user.PasswordHash = newPasswordHash;

                            // Kullanıcıyı güncelle
                            var updateResult = await _userManager.UpdateAsync(user);

                            if (updateResult.Succeeded)
                            {
                                // Başarılı güncelleme işlemi
                            }
                            else
                            {
                                // Güncelleme işlemi başarısız oldu
                            }
                        }

                        if (employee.CompanyEmail == email)
                        {
                            employee.CompanyEmail = employee.CompanyEmail;
                            user.UserName = user.UserName;
                            user.Email = employee.CompanyEmail;
                        }
                        else
                        {
                            if (findUser != null)
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

                        if (employeeCompany==companySuperManagerEmail)
                        {
                            company.SuperManagerEmail = employee.CompanyEmail;
                            company.SuperManagerFirstName = employee.FirstName;
                            company.SuperManagerLastName = employee.LastName;
                            company.ModifiedDate = DateTime.Now;
                            _companyService.Edit(company);
                        }
                        // Kullanıcı adını ve normalleştirilmiş kullanıcı adını güncelle ----------------------------
                        await _userManager.UpdateNormalizedUserNameAsync(user);
                        await _userManager.UpdateNormalizedEmailAsync(user);

                        employee.AppUser = user;
                        employee.AppUserId = user.Id;
                        _personService.Edit(employee);
                    }

                    // E-posta gönderme işlemleri
                    string mailBody = $"KullanıcıAdı: {employee.CompanyEmail} Şifre:{employee.Password}";
                    MailHelper.SendMail(employee.PersonalEmail, " Kullanici Güncelleştirme  Maili", mailBody, null, null);

                    // Başarılıysa başka bir ekrana yönlendir veya işlemi tamamla
                    TempData["SuccessMessage"] = "Personbel Başarıyla Guncellendi.";

                    
                    var roles = await _userManager.GetRolesAsync(user);
                    return RedirectToAction("Login");
                    
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
                    return RedirectToAction("PersonelSettings");
                }

                // İlgili person verisine bağlı kullanıcıyı bul
                var user = await _userManager.FindByIdAsync(employee.AppUserId.ToString());

                // Eğer kullanıcı bulunamazsa hata mesajı gönder
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Kullanıcı Bulunamadı.";
                    return RedirectToAction("PersonelSettings");
                }
                var company = _companyService.GetByEntity(x => x.Id == employee.CompanyId);

                // Süper yönetici kontrolü
                if (IsSuperManager(employee, company))
                {
                    TempData["ErrorMessage"] = "Süper Şirket yönetici olduğu için işlem yapılamaz.";
                    return RedirectToAction("PersonelSettings");
                }
                else
                {
                    // Person ve ilgili kullanıcıyı belirtilen duruma getir
                    user.Status = newStatus;
                    employee.Status = newStatus;
                    _personService.Edit(employee);

                    if (await _userManager.IsInRoleAsync(user, "companymanager"))
                    {
                        var employees = _personService.GetAll().Where(x => x.CompanyId == employee.CompanyId);
                        foreach (var emp in employees)
                        {
                            var empUser = await _userManager.FindByIdAsync(emp.AppUserId.ToString());
                            if (empUser != null)
                            {
                                if (!IsSuperManager(emp, company))
                                {
                                    empUser.Status = Status.DeActive;
                                    emp.Status = Status.DeActive;
                                    _personService.Edit(emp);

                                    // E-posta gönderme işlemleri
                                    string mailBody = $"Kullanıcınız Yönetici Tarafından Pasifleştirildi";
                                    MailHelper.SendMail(emp.PersonalEmail, " Kullanici Deactive Maili", mailBody, null, null);
                                }
                            }
                        }
                    }
                    if (await _userManager.IsInRoleAsync(user, "employee"))
                    {
                        employee.Status = Status.DeActive;
                        user.Status = Status.DeActive;
                        _personService.Edit(employee);

                        // E-posta gönderme işlemleri
                        string mailBody = $"Kullanıcınız Pasifleştirildi";
                        MailHelper.SendMail(employee.PersonalEmail, " Kullanici Deactive Maili", mailBody, null, null);
                    }

                    TempData["SuccessMessage"] = "İşlem Başarıyla gerçekleşti.";
                    return RedirectToAction("Login");

                }

            }
            catch (Exception ex)
            {
                // Hata durumunda hata mesajı gönder ve listeleme sayfasına yönlendir
                TempData["ErrorMessage"] = "İşlem Başarısız oldu." + " " + ex.Message;
                return RedirectToAction("PersonalSettings");
            }
        }
        private bool IsSuperManager(Person employee, Company company)
        {
            return company.SuperManagerEmail == employee.CompanyEmail &&
                   company.SuperManagerFirstName.ToLower() == employee.FirstName.ToLower() &&
                   company.SuperManagerLastName.ToLower() == employee.LastName.ToLower();
        }

        public IActionResult Memberships()
        {
            List<MembershipIndexVM> vm = new();
            var membershipList = _membershipService.GetAll();

            foreach (var item in membershipList)
            {
                var membership = new MembershipIndexVM();
                membership.Id = item.Id;
                membership.Name = item.Name;
                membership.SubscriptionPeriod = item.SubscriptionPeriod;
                membership.NumberOfEmployee = (int)item.NumberOfEmployee;
                membership.Description = item.Description;
                membership.Price = (decimal)item.Price;
                membership.Status = Status.Active;
                vm.Add(membership);
            }
            var msVm = _mapper.Map<List<MembershipIndexVM>>(vm);
            return View(msVm);

        }
        
    }
}

