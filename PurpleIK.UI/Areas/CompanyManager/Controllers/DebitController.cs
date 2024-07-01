using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PurpleIK.Core.Enums;
using PurpleIK.Entities;
using PurpleIK.Services.Concretes;
using PurpleIK.Services.Interfaces;
using PurpleIK.UI.Areas.CompanyManager.Models.VM.DebitVM;
using PurpleIK.UI.Areas.CompanyManager.Models.VM.PushMoneyVM;
using PurpleIK.UI.Utility;
using System;
using System.Drawing;
using System.Security.Claims;


namespace PurpleIK.UI.Areas.CompanyManager.Controllers
{
    [Area("CompanyManager")]
    [Authorize(Roles = "companymanager")]
    public class DebitController : Controller
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ICompanyService _companyService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IPersonService _personService;
        private readonly IDebitService _debitService;


        public DebitController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ICompanyService companyService, IMapper mapper, IConfiguration configuration, IPersonService personService, IDebitService debitService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _companyService = companyService;
            _mapper = mapper;
            _configuration = configuration;
            _personService = personService;
            _debitService = debitService;
        }
        public async Task<IActionResult> ListDebit()
        {
            // Giriş yapmış kullanıcının adını al
            var userEmailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var companyManager = _personService.GetByEntity(x => x.CompanyEmail.ToLower() == userEmailClaim);
            ViewBag.CompanyManager = companyManager;

            List<DebitIndexVM> vm = new();

            var debitList = _debitService.GetAll().Where(comment => comment.Person.CompanyId == companyManager.CompanyId).ToList();

            foreach (var item in debitList)
            {
                var debit = new DebitIndexVM();
                debit.Id = item.Id;
                debit.ProductName = item.ProductName;
                debit.ReceiptDate = item.ReceiptDate;
                debit.DeliveryDate = item.DeliveryDate;
                debit.DebitForm = item.DebitForm;
                debit.PersonId = (Guid)item.PersonId;

                // Zimmet kaydının sahibi olan çalışanı bul
                var employee = _personService.GetAll()
                    .FirstOrDefault(x => x.Id == item.PersonId && x.Company.Id == companyManager.CompanyId && x.Status == Status.Active);

                if (employee.ProfilePhoto != null)
                {
                    debit.Picture = $"data:image/png;base64,{Convert.ToBase64String(employee.ProfilePhoto)}";
                }

                debit.Employee = employee != null ? $"{employee.FirstName} {employee.LastName}" : "";
                debit.Status = item.Status.ToString();
                vm.Add(debit);
            }
            var pmVm = _mapper.Map<List<DebitIndexVM>>(vm);
            return View(pmVm);
        }
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            // Giriş yapmış kullanıcının adını al
            var userEmailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var companyManager = _personService.GetByEntity(x => x.CompanyEmail.ToLower() == userEmailClaim);

            var persons = _personService.GetAll()
                .Where(x => x.Company.Id == companyManager.CompanyId && x.Status == Status.Active && x.AppUserId != null)
                .ToList();

            var vm = new DebitAddVM
            {
                Persons = persons
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Add(DebitAddVM vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Giriş yapmış kullanıcının adını al
                    var userEmailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                    var person = _personService.GetByEntity(x => x.CompanyEmail.ToLower() == userEmailClaim);

                    Debit p = _mapper.Map<Debit>(vm.DebitAddItem);
                    
                    p.CreatedDate = DateTime.Now;
                    p.ModifiedDate = DateTime.Now;

                    p.Status = person.Id == p.PersonId ? Status.Active : Status.Approval;

                    p.ManagerId = person.Id;

                    if (vm.DebitAddItem.DebitFormFile != null && vm.DebitAddItem.DebitFormFile.Length > 0)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            vm.DebitAddItem.DebitFormFile.CopyTo(ms);
                            p.DebitForm = ms.ToArray();
                        }
                    }

                    _debitService.Add(p);

                    await SendDebitEmail(p,"eklendi");
                    TempData["SuccessMessage"] = "Zimmet Ekleme Başarıyla Oluşturuldu.";
                    return RedirectToAction("ListDebit");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Zimmet Ekleme Başarısız Oldu";
                    return View(vm);
                }
            }
            TempData["ErrorMessage"] = "Zimmet Ekleme ile ilgili tüm alanlara veri giriniz";
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var debit = _debitService.Find(id);
            if (debit == null)
            {
                TempData["ErrorMessage"] = "Zimmet Bulunamadı";
                return RedirectToAction("ListDebit");
            }

            var userEmailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var companyManager = _personService.GetByEntity(x => x.CompanyEmail.ToLower() == userEmailClaim);

            var persons = _personService.GetAll()
                .Where(x => x.Company.Id == companyManager.CompanyId && x.Status == Status.Active && x.AppUserId != null).ToList();

            var vm = new DebitEditVM
            {
                DebitEditItem = new DebitEditItem
                {
                    Id = debit.Id, // Id değerinin atanması
                    ProductName = debit.ProductName,
                    ReceiptDate = debit.ReceiptDate,
                    DeliveryDate= debit.DeliveryDate,
                    PersonId = debit.PersonId,
                    DebitFormFile_ = debit.DebitForm,
                    ManagerId = debit.ManagerId
                },
                Persons = persons
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DebitEditVM vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Giriş yapmış kullanıcının adını al
                    var userEmailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                    var person = _personService.GetByEntity(x => x.CompanyEmail.ToLower() == userEmailClaim);

                    // Debiti bul
                    var debit = _debitService.Find(vm.DebitEditItem.Id);
                    if (debit == null)
                    {
                        return NotFound();
                    }

                    // Değişiklikleri güncelle
                    debit.ProductName = vm.DebitEditItem.ProductName;
                    debit.ReceiptDate = vm.DebitEditItem.ReceiptDate;
                    debit.PersonId = vm.DebitEditItem.PersonId;
                    debit.ModifiedDate = DateTime.Now;
                    debit.Status = person.Id == debit.PersonId ? Status.Active : Status.Approval;
                    debit.ManagerId = person.Id;

                    if (vm.DebitEditItem.DebitFormFile != null && vm.DebitEditItem.DebitFormFile.Length > 0)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            vm.DebitEditItem.DebitFormFile.CopyTo(ms);
                            debit.DebitForm = ms.ToArray();
                        }
                    }

                    // Değişiklikleri kaydet
                    _debitService.Edit(debit);

                    await SendDebitEmail(debit, "güncellendi");
                    TempData["SuccessMessage"] = "Zimmet Başarıyla Güncellendi.";
                    return RedirectToAction("ListDebit");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Zimmet Düzenleme İşlemi Başarısız Oldu";
                    return View(vm);
                }
            }

            // ModelState geçersizse, formu tekrar göster
            TempData["ErrorMessage"] = "Zimmet ile ilgili tüm alanlara veri giriniz";
            return View(vm);
        }
        public IActionResult GetDocument(Guid id, string layoutName)
        {
            var debit = _debitService.Find(id);
            if (debit == null || debit.DebitForm == null)
            {
                TempData["ErrorMessage"] = "Dosya Bulunamadı";
                if (layoutName == "Edit")
                {
                    return RedirectToAction(layoutName, new { id });
                }
                return RedirectToAction(layoutName);
            }

            return File(debit.DebitForm, "application/pdf");
        }
        [HttpGet]
        public async Task<IActionResult> Remove(Guid id)
        {
            try
            {
                Debit p = _debitService.Find(id);
                _debitService.Remove(p);
                await SendDebitEmaili(p);
                TempData["SuccessMessage"] = "Zimmet Başarıyla Silindi.";

            }
            catch (Exception)
            {

            }
            return RedirectToAction("ListDebit");
        }

        private async Task SendDebitEmail(Debit debit,string x)
        {
            try
            {
                var person = _personService.GetByEntity(x => x.Id == debit.PersonId);
                if (person != null)
                {
                    // E-posta adresi formatını doğrula
                    if (!IsValidEmail(person.PersonalEmail))
                    {
                        // Geçersiz e-posta adresini ele al
                        // Hatasını kaydedebilir veya bir istisna fırlatabilirsiniz
                        throw new FormatException("Geçersiz e-posta adresi formatı.");
                    }

                    var subject = "Zimmet Bilgisi";
                    var body = $"Sevgili {person.FirstName} {person.LastName},<br><br>";
                    body += $"Sizin için bir zimmet kaydı {x}. Lütfen ekteki zimmet formuna bakınız.<br><br>";
                    body += "Saygılarımızla,<br>Şirket yöneticisi";

                    // Byte dizisini base64 kodlanmış dizeye dönüştür
                    string attachmentData = Convert.ToBase64String(debit.DebitForm);

                    MailHelper.SendMail(person.PersonalEmail, subject, body, debit.DebitForm, "Zimmet_formu.pdf");
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions...
            }
        }
        // E-posta adresi formatını doğrulamak için yöntem
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        private async Task SendDebitEmaili(Debit debit)
        {
            try
            {
                var person = _personService.GetByEntity(x => x.Id == debit.PersonId);
                if (person != null)
                {
                    // E-posta adresi formatını doğrula
                    if (!IsValidEmail(person.PersonalEmail))
                    {
                        // Geçersiz e-posta adresini ele al
                        // Hatasını kaydedebilir veya bir istisna fırlatabilirsiniz
                        throw new FormatException("Geçersiz e-posta adresi formatı.");
                    }

                    var subject = "Zimmet Bilgisi";
                    var body = $"Sevgili {person.FirstName} {person.LastName},<br><br>";
                    body += $"Sizin için bir zimmet kaydı silindi.<br><br>";
                    body += "Saygılarımızla,<br>Şirket yöneticisi";


                    MailHelper.SendMail(person.PersonalEmail, subject, body,null,null);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions...
            }
        }
        public async Task<IActionResult> ActiveDebit(Guid id)
        {
            var debit = _debitService.Find(id);

            // Zimmetin durumunu "Onaylı" olarak güncelle
            debit.Status = Status.Active;
            _debitService.Edit(debit);

            TempData["SuccessMessage"] = "Zimmet başarıyla onaylandı.";
            return RedirectToAction("ListDebit");
        }
        public async Task<IActionResult> DeActiveDebit(Guid id)
        {
            var debit = _debitService.Find(id);

            // Zimmetin durumunu "Reddet" olarak güncelle
            debit.Status = Status.DeActive;
            _debitService.Edit(debit);

            TempData["SuccessMessage"] = "Zimmet başarıyla reddedildi.";
            return RedirectToAction("ListDebit");
        }
    }
}
