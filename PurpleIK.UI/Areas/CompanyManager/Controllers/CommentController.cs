using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PurpleIK.Core;
using PurpleIK.Core.Enums;
using PurpleIK.Entities;
using PurpleIK.Services.Concretes;
using PurpleIK.Services.Interfaces;
using PurpleIK.UI.Areas.CompanyManager.Models.VM.CommentVM;
using PurpleIK.UI.Areas.CompanyManager.Models.VM.DebitVM;
using PurpleIK.UI.Areas.CompanyManager.Models.VM.EmployeeManagerVM;
using PurpleIK.UI.Areas.CompanyManager.Models.VM.PushMoneyVM;
using PurpleIK.UI.Areas.Employee.Models.VM.DebitEmployeeVM;
using PurpleIK.UI.Utility;
using System;
using System.Security.Claims;

namespace PurpleIK.UI.Areas.CompanyManager.Controllers
{
    [Area("CompanyManager")]
    [Authorize(Roles = "companymanager")]
    public class CommentController : Controller
    {
        ICommentService _commentService;
        UserManager<AppUser> _userManager;
        IPersonService _personService;
        IMapper _mapper;
        public CommentController(ICommentService service, IPersonService personService, IMapper mapper, UserManager<AppUser> userManager)
        {
            _commentService = service;
            _personService = personService;
            _mapper = mapper;
            _userManager = userManager;

        }
        public IActionResult ListComment()
        {
            try
            {
                // Giriş yapmış kullanıcının adını al
                var userEmailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var companyManager = _personService.GetByEntity(x => x.CompanyEmail.ToLower() == userEmailClaim);
                var companyManagerId = companyManager.Id;


                var isSuperManager = IsSuperManager(companyManager, companyManager.Company);
                ViewData["IsSuperManager"] = isSuperManager;
                
                List<CommentIndexVM> vm = new();
                
                var commentList = _commentService.GetAll().Where(comment => comment.Person.CompanyId == companyManager.CompanyId).ToList();

                foreach (var item in commentList)
                {
                    var comment = new CommentIndexVM();
                    comment.Id = item.Id;
                    comment.Summary = item.Summary;
                    comment.CommentText = item.CommentText;
                    comment.Title = item.Title;
                    comment.CreatedDate = item.CreatedDate.Value.ToString("dd.MM.yyyy");
                    comment.Department = item.Person.Department.Value.GetDisplayName();
                    comment.CompanyName = item.Person.Company.CompanyName;
                    comment.ActiveId = companyManagerId;
                    comment.PersonId = item.PersonId;
                    comment.Status = item.Status;

                    var person = _personService.GetByEntity(x => x.Id == item.PersonId);
                    if (person != null)
                    {
                        comment.CompanyManagerName = $"{person.FirstName} {person.LastName}";
                    }

                    if (item.Photo != null)
                    {
                        comment.Picture = $"data:image/png;base64,{Convert.ToBase64String(item.Photo)}";
                    }

                    vm.Add(comment);
                }

                var cmVm = _mapper.Map<List<CommentIndexVM>>(vm);
                return View(cmVm);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> CommentAdd()
        {
            // Giriş yapmış kullanıcının adını al
            var userEmailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var companyManager = _personService.GetByEntity(x => x.CompanyEmail.ToLower() == userEmailClaim);

            var model = new CommentAddVM();
            model.PersonId = companyManager.Id;
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> CommentAdd(CommentAddVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userEmailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                    var person = _personService.GetByEntity(x => x.CompanyEmail.ToLower() == userEmailClaim);


                    Comment c = _mapper.Map<Comment>(model);
                    c.CreatedDate = DateTime.Now;
                    c.CommentText = model.CommentText;
                    c.PersonId = model.PersonId;
                    c.Status = Status.Approval;
                    c.Title = person.Company.CompanyName;
                    using (var ms = new MemoryStream())
                    {
                        model._Photo.CopyTo(ms);
                        c.Photo = ms.ToArray();
                    }

                    _commentService.Add(c);

                    return RedirectToAction("ListComment");
                }
                catch (Exception ex)
                {

                }
            }
            TempData["message"] = $"Bir hata oluştu!";
            return View(model);
        }

        private bool IsSuperManager(Person employee, Company company)
        {
            return company.SuperManagerEmail == employee.CompanyEmail;
        }
        
        public async Task<IActionResult> CommentApprove(Guid id)
        {
            var comment = _commentService.Find(id);

            // Zimmetin durumunu "Onaylı" olarak güncelle
            comment.Status = Status.Active;
            _commentService.Edit(comment);

            //TempData["SuccessMessage"] = "Zimmet başarıyla onaylandı.";
            return RedirectToAction("ListComment");
        }
        [HttpGet]
        public async Task<IActionResult> CommentReject(Guid id)
        {
            var vm = new CommentRejectVM { Id = id };
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> CommentReject(CommentRejectVM vm)
        {
            if (ModelState.IsValid)
            {
                var debit = _commentService.Find(vm.Id);

                // Zimmetin durumunu "Reddedilen" olarak güncelle
                debit.Status = Status.DeActive;
                _commentService.Edit(debit);

                // E-posta gönderme işlemleri
                string mailBody = $"Yorum reddedildi. Reddetme nedeni: {vm.Reject}";
                MailHelper.SendMail(debit.Person.PersonalEmail, " Yorum Reddetme Maili", mailBody, null, null);

                return RedirectToAction("ListComment");
            }
            return View(vm);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var comment = _commentService.Find(id);

            CommentEditVM model = _mapper.Map<CommentEditVM>(comment);
            if (comment.Photo != null)
            {
                model.PictureBase64 = $"data:image/png;base64,{Convert.ToBase64String(comment.Photo)}";
            }
            model.PersonId = comment.PersonId;
            model.Department = comment.Person.Department;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CommentEditVM vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userEmailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                    var person = _personService.GetByEntity(x => x.CompanyEmail.ToLower() == userEmailClaim);

                    var comment = _commentService.Find(vm.Id);

                    // Gerekli güncellemeleri yap
                    comment.CommentText = vm.CommentText;
                    comment.Title = person.Company.CompanyName;
                    comment.Summary = vm.Summary;
                    comment.Status = Status.Approval;
                    comment.ModifiedDate = DateTime.Now;
                    comment.PersonId = vm.PersonId;
                    // Eğer yeni resim seçilmemişse, mevcut resmi kullan
                    if (vm._Photo != null)
                    {
                        using (var ms = new MemoryStream())
                        {
                            vm._Photo.CopyTo(ms);
                            comment.Photo = ms.ToArray();
                        }
                    }
                    else
                    {
                        comment.Photo = comment.Photo;
                    }

                    _commentService.Edit(comment);
                    return RedirectToAction("ListComment");
                }
                catch (Exception ex)
                {
                    TempData["message"] = $"Bir hata oluştu: {ex.Message}";
                    return View(vm);
                }
            }

            // ModelState geçerli değilse, formu tekrar görüntüle
            TempData["message"] = $"Lütfen formu doğru şekilde doldurun.";
            return View(vm);
        }
        [HttpGet]
        public IActionResult Remove(Guid id)
        {
            try
            {
                Comment c = _commentService.Find(id);
                _commentService.Remove(c);
            }
            catch (Exception)
            {

            }
            return RedirectToAction("ListComment");
        }

    }
}
