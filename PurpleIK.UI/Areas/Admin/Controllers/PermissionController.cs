using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PurpleIK.Core.Enums;
using PurpleIK.Entities;
using PurpleIK.Services.Interfaces;
using PurpleIK.UI.Areas.Admin.Models.VM.PermissionVM;

namespace PurpleIK.UI.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class PermissionController : Controller
    {
        IPermissionService _permissionService;
        IMapper _mapper;
        public PermissionController(IPermissionService service,IMapper mapper)
        {
            _permissionService = service;
            _mapper = mapper;
        }
        
        public IActionResult Index(IndexPermissionVM vm)
        {
            List<Permission> permissionList;

            if (!string.IsNullOrWhiteSpace(vm.IzinAdi))
            {
                permissionList = _permissionService.GetBy(x => x.PermissionType.Contains(vm.IzinAdi) && x.Status == Status.Active).ToList();
                return View(vm);
            }
            vm = new IndexPermissionVM();
            permissionList = _permissionService.GetBy(x => x.Status == Status.Active).ToList();
            vm.Permissions = _mapper.Map<List<IndexPermissionItem>>(permissionList);
            return View(vm);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Add(AddPermissionVM permission)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Permission p = _mapper.Map<Permission>(permission);
                    p.CreatedDate = DateTime.Now;
                    p.ModifiedDate = DateTime.Now;
                    p.Status = Status.Active;
                    _permissionService.Add(p);
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
        public IActionResult Edit(Guid id)
        {
            Permission p = _permissionService.Find(id);
            EditPermissonVM vm = _mapper.Map<EditPermissonVM>(p);
            return View(vm);
        }
        [HttpPost]
        public IActionResult Edit(EditPermissonVM permission)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Permission p = _permissionService.Find(permission.Id);
                    p.PermissionType = permission.PermissionType;
                    p.ModifiedDate = DateTime.Now;
                    p.Status = Status.Active;

                    _permissionService.Edit(p);

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {

                }
            }
            TempData["message"] = $"Bir Hata Oluştu";
            return View();
        }
        public IActionResult Remove(Guid id)
        {
            try
            {
                Permission p = _permissionService.Find(id);
                _permissionService.Remove(p);
            }
            catch (Exception)
            {


            }
            return RedirectToAction("Index");
        }
    }
}
