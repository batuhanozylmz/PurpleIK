using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PdfSharp.Drawing;
using PurpleIK.Core.Enums;
using PurpleIK.Entities;
using PurpleIK.Services.Concretes;
using PurpleIK.Services.Interfaces;
using PurpleIK.UI.Areas.CompanyManager.Models.VM.DebitVM;
using PurpleIK.UI.Areas.CompanyManager.Models.VM.ExpenseVM;
using PurpleIK.UI.Areas.CompanyManager.Models.VM.PermissionVM;
using PurpleIK.UI.Models.VM.AccountVM;
using PurpleIK.UI.Utility;
using System.Data;
using System.Security.Claims;

namespace PurpleIK.UI.Areas.Employee.Controllers
{
    [Area("CompanyManager")]
    [Authorize(Roles = "companymanager")]
    public class ExpenseController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ICompanyService _companyService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IPersonService _personService;
        private readonly IExpenseService _expenseService;


        public ExpenseController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ICompanyService companyService, IMapper mapper, IConfiguration configuration, IPersonService personService, IExpenseService expenseService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _companyService = companyService;
            _mapper = mapper;
            _configuration = configuration;
            _personService = personService;
            _expenseService = expenseService;
        }
        public async Task<IActionResult> Index()
        {
            // Giriş yapmış kullanıcının adını al
            var userEmailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var companyManager = _personService.GetByEntity(x => x.CompanyEmail.ToLower() == userEmailClaim);

            List<ExpenseIndexVM> vm = new List<ExpenseIndexVM>();

            var expenseList = _expenseService.GetAll().Where(comment => comment.Person.CompanyId == companyManager.CompanyId).ToList(); // aynı şirkette olanları listeledi
            foreach (var item in expenseList)
            {
                var expense = new ExpenseIndexVM();
                expense.Id = item.Id;
                expense.Name = item.Name;
                expense.Description = item.Description;
                expense.ExpenseDate = item.ExpenseDate;
                expense.CreatedDate = item.CreatedDate;
                expense.ExpenseForm = item.ExpenseForm;
                expense.Price = item.Price;
                expense.PersonId = item.PersonId;

                //kaydının sahibi olan çalışanı bul
                var employee = _personService.GetAll()
                    .FirstOrDefault(x => x.Id == item.PersonId && x.Company.Id == companyManager.CompanyId && x.Status == Status.Active);

                expense.Employee = employee != null ? $"{employee.FirstName} {employee.LastName}" : "";
                if (employee.ProfilePhoto != null)
                {
                    expense.Picture = $"data:image/png;base64,{Convert.ToBase64String(employee.ProfilePhoto)}";
                }
                expense.Status = item.Status.ToString();
                vm.Add(expense);
            }

            var model = _mapper.Map<List<ExpenseIndexVM>>(vm);
            return View(model);

        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var vm = new ExpenseAddVM();

            // Giriş yapmış kullanıcının adını al
            var userEmailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var companyManager = _personService.GetByEntity(x => x.CompanyEmail.ToLower() == userEmailClaim);

            var persons = _personService.GetAll()
                 .Where(x => x.Company.Id == companyManager.CompanyId && x.Status == Status.Active && x.AppUserId != null).ToList();
            vm.Persons = persons;

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ExpenseAddVM vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "İzin Talebi ile ilgili tüm alanlara veri giriniz";
                return View(vm);
            }
            try
            {
                // Giriş yapmış kullanıcının adını al
                var userEmailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var companyManager = _personService.GetByEntity(x => x.CompanyEmail.ToLower() == userEmailClaim);

                Expense p = _mapper.Map<Expense>(vm.ExpenseAddItem);

                p.CreatedDate = DateTime.Now;
                p.Status = Status.Active;

                var person = _personService.GetByEntity(x => x.Id == vm.ExpenseAddItem.PersonId);

                p.PersonId = person.Id;
                p.Person = person;

                if (vm.ExpenseAddItem.ExpenseFormFile != null && vm.ExpenseAddItem.ExpenseFormFile.Length > 0)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        vm.ExpenseAddItem.ExpenseFormFile.CopyTo(ms);
                        p.ExpenseForm = ms.ToArray();
                    }
                }

                _expenseService.Add(p);

                var subject = "Harcama Bilgisi";
                var mailBody = $"Sevgili {companyManager.FirstName} {companyManager.LastName},<br><br>";
                mailBody += $"Sizin için bir harcama kaydı eklendi. <br><br>";
                mailBody += $"Saygılarımızla,<br>{companyManager.FirstName}{companyManager.LastName}";

                MailHelper.SendMail(companyManager.PersonalEmail, subject, mailBody, null, null);

                TempData["SuccessMessage"] = "Başarıyla Eklendi.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                return View(vm);
            }
        }

        public IActionResult GetDocument(Guid id, string layoutName)
        {
            var expense = _expenseService.Find(id);
            if (expense == null || expense.ExpenseForm == null)
            {
                TempData["ErrorMessage"] = "Dosya Bulunamadı";
                if (layoutName == "Edit")
                {
                    return RedirectToAction(layoutName, new { id });
                }
                return RedirectToAction(layoutName);
            }
            string contentType = "application/pdf";
            if (IsPng(expense.ExpenseForm))
            {
                contentType = "image/png";
            }
            else if (IsJpeg(expense.ExpenseForm))
            {
                contentType = "image/jpeg";
            }
            return File(expense.ExpenseForm, contentType);
        }
        private bool IsPng(byte[] fileBytes)
        {

            return fileBytes[0] == 0x89 &&
                   fileBytes[1] == 0x50 &&
                   fileBytes[2] == 0x4E &&
                   fileBytes[3] == 0x47 &&
                   fileBytes[4] == 0x0D &&
                   fileBytes[5] == 0x0A &&
                   fileBytes[6] == 0x1A &&
                   fileBytes[7] == 0x0A;
        }
        private bool IsJpeg(byte[] fileBytes)
        {
            return fileBytes[0] == 0xFF &&
                   fileBytes[1] == 0xD8;
        }
        public async Task<IActionResult> ChangeExpenseStatus(Guid id, Status newStatus)
        {
            try
            {
                var expense = _expenseService.Find(id);

                if (expense == null)
                {
                    TempData["ErrorMessage"] = "Harcama Bulunamadı.";
                    return RedirectToAction("Index");
                }
                var person = _personService.GetByEntity(x => x.Id == expense.PersonId);

                // E-posta gönderme işlemleri
                string mailBody = "";
                if (newStatus == Status.Active)
                {
                    expense.Status=Status.Active;      
                    mailBody = $"Harcama Yönetici Tarafından Onaylandı.";

                    MailHelper.SendMail(person.PersonalEmail, "Harcama Bilgisi", mailBody, null, null);
                }
                else if (newStatus == Status.DeActive)
                {
                    expense.Status = Status.DeActive;
                    mailBody = $"Harcama Yönetici Tarafından Red Edildi";
                    MailHelper.SendMail(person.PersonalEmail, "Harcama Bilgisi", mailBody, null, null);
                }
                expense.ModifiedDate = DateTime.Now;
                _expenseService.Edit(expense);
                TempData["SuccessMessage"] = "İşlem Başarıyla gerçekleşti.";
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "İşlem Başarısız oldu.." + " " + ex.Message;
                return RedirectToAction("Index");
            }
        }
        private bool IsSuperManager(Person employee, Company company)
        {
            return company.SuperManagerEmail == employee.CompanyEmail &&
                   company.SuperManagerFirstName.ToLower() == employee.FirstName.ToLower() &&
                   company.SuperManagerLastName.ToLower() == employee.LastName.ToLower();
        }
    }
}
