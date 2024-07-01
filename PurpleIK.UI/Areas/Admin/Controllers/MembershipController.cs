using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurpleIK.Core.Enums;
using PurpleIK.Entities;
using PurpleIK.Services.Concretes;
using PurpleIK.Services.Interfaces;
using PurpleIK.UI.Areas.Admin.Models.VM.MembershipVM;
using PurpleIK.UI.Areas.Admin.Models.VM.PermissionVM;

namespace PurpleIK.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class MembershipController : Controller
    {
        private readonly ICompanyService _companyService;
        private readonly IMembershipService _membershipService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public MembershipController(ICompanyService companyService, IMembershipService membershipService, IMapper mapper, IConfiguration configuration)
        {
            _companyService = companyService;
            _membershipService = membershipService;
            _mapper = mapper;
            _configuration = configuration;
        }

        public IActionResult Index()
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

        [HttpGet]

        public IActionResult AddMembership() 
        {

            return View();
        }

        [HttpPost]
        public IActionResult AddMembership(MembershipAddVM vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Membership m = _mapper.Map<Membership>(vm);
                    m.CreatedDate = DateTime.Now;
                    m.ModifiedDate = DateTime.Now;
                    m.Status = Status.Active;
                    _membershipService.Add(m);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {

                }
            }
            TempData["message"] = $"Bir Hata Oluştu";
            return View();
        }

        [HttpGet]
        public IActionResult EditMembership(Guid id)
        {
            Membership m = _membershipService.Find(id);
            MembershipEditVM vm = _mapper.Map<MembershipEditVM>(m);
            return View(vm);
        }

        [HttpPost]
        public IActionResult EditMembership(MembershipEditVM vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Membership m = _membershipService.Find(vm.Id);
                    m.Name = vm.Name;
                    m.SubscriptionPeriod = vm.SubscriptionPeriod;
                    m.Description = vm.Description;
                    m.Price = vm.Price;
                    m.NumberOfEmployee = vm.NumberOfEmployee;
                    m.ModifiedDate = DateTime.Now;
                    m.Status = Status.Active;

                    _membershipService.Edit(m);

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {

                }
            }
            TempData["message"] = $"Bir Hata Oluştu";
            return View();
        }

        public IActionResult RemoveMembership(Guid id)
        {
            try
            {
                Membership m = _membershipService.Find(id);
                _membershipService.Remove(m);
            }
            catch (Exception)
            {


            }
            return RedirectToAction("Index");
        }
        //private void MakePassive()
        //{

        //    if (DateTime.Now > memberships.)
        //}
        //public IActionResult Index(Guid companyId)
        //{
        //    TempData["CompanyId"] = companyId;
        //    // companyId parametresiyle ilgili işlemler yapılabilir
        //    var company = _companyService.Find(companyId);
        //    return View();
        //    var member = _membershipService.GetAll().Where(x => x.Company.Id == companyId && x.Status == Status.Approval);

        //    List<MembershipIndexVM> companyList = new List<MembershipIndexVM>();

        //    if (member != null)
        //    {

        //    }
        //}
    }
}
