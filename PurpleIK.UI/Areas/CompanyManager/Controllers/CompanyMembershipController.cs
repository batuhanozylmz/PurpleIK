using Microsoft.AspNetCore.Mvc;
using PurpleIK.Services.Interfaces;
using PurpleIK.Core.Enums;
using PurpleIK.UI.Areas.Admin.Models.VM.MembershipVM;
using PurpleIK.UI.Areas.CompanyManager.Models.VM.CompanyMembershipVM;
using AutoMapper;
using PurpleIK.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using PurpleIK.UI.Models.VM.AccountVM;
using System.Numerics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace PurpleIK.UI.Areas.CompanyManager.Controllers
{
    [Area("CompanyManager")]
    [Authorize(Roles = "companymanager")]
    public class CompanyMembershipController : Controller
    {
        private readonly IMembershipService _membershipService;
        private readonly ICompanyService _companyService;
        private readonly ICompanyMembershipService _companyMembershipService;
        private readonly IPersonService _personService;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;


        public CompanyMembershipController(IMembershipService membershipService, ICompanyService companyService, ICompanyMembershipService companyMembershipService, IMapper mapper, UserManager<AppUser> userManager, IPersonService personService)
        {
            _membershipService = membershipService;
            _companyMembershipService = companyMembershipService;
            _companyService = companyService;
            _mapper = mapper;
            _userManager = userManager;
            _personService = personService;
        }

        public IActionResult MembershipList()
        {
            List<MembershipListVM> vm = new();
            var membershipList = _membershipService.GetAll();

            foreach (var item in membershipList)
            {
                var membership = new MembershipListVM();
                membership.PlanId = item.Id;
                membership.Name = item.Name;
                membership.SubscriptionPeriod = item.SubscriptionPeriod;
                membership.NumberOfEmployee = (int)item.NumberOfEmployee;
                membership.Price = (decimal)item.Price;                
                vm.Add(membership);
            }
            var msVm = _mapper.Map<List<MembershipListVM>>(vm);
            return View(msVm);
        }

        [HttpGet]
        public IActionResult Membership(Guid id)
        {
            var membership = _membershipService.Find(id);
            MembershipVM vm = new MembershipVM();
            vm.PlanId = id;
            vm.Name = membership.Name;
            vm.SubscriptionPeriod = membership.SubscriptionPeriod;
            vm.Price = (decimal)membership.Price;
            vm.NumberOfEmployees = membership.NumberOfEmployee;
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Membership(MembershipVM vm)
        {
            if (vm.PlanId != null)
            {
                // Giriş yapmış kullanıcının e-posta id'sini al
                var userEmailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var companyManager = _personService.GetByEntity(x => x.CompanyEmail.ToLower() == userEmailClaim);
                var company = _companyService.GetByEntity(x => x.Id == companyManager.CompanyId);

                var membership = _membershipService.GetByEntity(x => x.Id == vm.PlanId);
                var employees = _personService.GetAll().Where(x => x.CompanyId == company.Id);

                CompanyMembership companyMembership = new CompanyMembership();
                if (employees.Count() <= membership.NumberOfEmployee)
                {
                    companyMembership.NumberOfEmployee = vm.NumberOfEmployees;                    
                }
                else
                {
                    TempData["MembershipErrorMessage"] = "Seçtiğiniz üyelik şirket çalışanı sınırına uymuyor lütfen uygun üyelik seçiniz";
                    return RedirectToAction("MembershipList");
                }
                companyMembership.Status = Status.Active;
                companyMembership.CompanyName = company.CompanyName;
                companyMembership.Duration = vm.Duration;
                companyMembership.ApplicationDate = DateTime.Now;
                companyMembership.SubscriptionPeriod = vm.SubscriptionPeriod;
                companyMembership.Price = membership.Price;
                companyMembership.Name = membership.Name;
                if (companyMembership.SubscriptionPeriod == "Aylık")
                {
                    companyMembership.ExpiryDate = DateTime.Now.AddMonths(companyMembership.Duration.Value);
                }
                else if (companyMembership.SubscriptionPeriod == "Yıllık")
                {
                    companyMembership.ExpiryDate = DateTime.Now.AddYears(companyMembership.Duration.Value);
                }
                companyMembership.CompanyId = company.Id;
                companyMembership.MembershipId = membership.Id;
                companyMembership.SubscriptionPeriod = membership.SubscriptionPeriod;
                _companyMembershipService.Add(companyMembership);
                MakeStatusActive(company.Id);
            }            
            return RedirectToAction("Home", "Company", new { area = "CompanyManager" });
        }

        public void MakeStatusActive(Guid companyId)
        {

            var company = _companyService.Find(companyId);
            if (DateTime.Now < company.CompanyMemberships.ExpiryDate)
            {
                company.CompanyMemberships.Status = Status.Active;
                company.Status = Status.Active;
            }

            _companyService.Edit(company);
        }       
    }
}
