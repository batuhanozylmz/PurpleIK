using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PurpleIK.Core.Enums;
using PurpleIK.Entities;
using PurpleIK.Services.Concretes;
using PurpleIK.Services.Interfaces;
using PurpleIK.UI.Areas.CompanyManager.Models.VM.ExpenseVM;
using PurpleIK.UI.Areas.Employee.Models.VM.DebitEmployeeVM;
using PurpleIK.UI.Areas.Employee.Models.VM.ExpenseEmployeeVM;
using PurpleIK.UI.Utility;
using System.Security.Claims;

namespace PurpleIK.UI.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class ExpenseEmployeeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ICompanyService _companyService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IPersonService _personService;
        private readonly IExpenseService _expenseService;

        public ExpenseEmployeeController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ICompanyService companyService, IMapper mapper, IConfiguration configuration, IPersonService personService, IExpenseService expenseService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _companyService = companyService;
            _mapper = mapper;
            _configuration = configuration;
            _personService = personService;
            _expenseService = expenseService;
        }
        public IActionResult ExpenseEmployeeList()
        {
            // Giriş yapmış kullanıcının AppUser kimliğini al
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            // Kullanıcının kimlik bilgisini kullanarak personel kaydını bul
            var person = _personService.GetByEntity(x => x.AppUserId == Guid.Parse(userId));

            // Çalışanın tüm harcamalarını getirir
            var expensesForEmployee = _expenseService.GetAll()
                .Where(expense => expense.PersonId == person.Id)
                .Select(expense => new ExpenseEmployeeItem
                {
                    Id = expense.Id,
                    Name = expense.Name,
                    ExpenseDate = expense.ExpenseDate,
                    Description = expense.Description,
                    CreatedDate = expense.CreatedDate,
                    Price = expense.Price,
                    ExpenseForm = expense.ExpenseForm,
                    PersonId = expense.PersonId,
                    ManagerId = expense.ManagerId,
                    Status = expense.Status.ToString()
                })
                .ToList();

            var vm = new ExpenseEmployeeIndexVM
            {
                ExpenseEmployeeItems = expensesForEmployee
            };
            return View(vm);
        }
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            // Giriş yapmış kullanıcının adını al
            var userEmailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            var vm = new ExpenseEmployeeAddVM
            {
                Persons = new List<Person>()
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ExpenseEmployeeAddVM vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Giriş yapmış kullanıcının adını al
                    var userEmailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                    var person = _personService.GetByEntity(x => x.CompanyEmail.ToLower() == userEmailClaim);

                    Expense p = _mapper.Map<Expense>(vm.ExpenseEmployeeAddItem);

                    p.CreatedDate = DateTime.Now;
                    p.Status = Status.Approval;
                    p.PersonId = person.Id;

                    if (vm.ExpenseEmployeeAddItem.ExpenseFormFile != null && vm.ExpenseEmployeeAddItem.ExpenseFormFile.Length > 0)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            vm.ExpenseEmployeeAddItem.ExpenseFormFile.CopyTo(ms);
                            p.ExpenseForm = ms.ToArray();
                        }
                    }

                    _expenseService.Add(p);                  
                    return RedirectToAction("ExpenseEmployeeList");
                }
                catch (Exception ex)
                {
                    
                    return View(vm);
                }
            }
            TempData["message"] = $"Bir hata oluştu!";
            return RedirectToAction("ExpenseEmployeeList");
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
    }
}
