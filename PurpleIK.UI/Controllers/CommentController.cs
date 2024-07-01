using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PurpleIK.Core;
using PurpleIK.Core.Enums;
using PurpleIK.Entities;
using PurpleIK.Services.Interfaces;
using PurpleIK.UI.Areas.CompanyManager.Models.VM.CommentVM;
using PurpleIK.UI.Areas.CompanyManager.Models.VM.EmployeeManagerVM;
using PurpleIK.UI.Models.VM.CommentVM;
using System.Security.Claims;

namespace PurpleIK.UI.Controllers
{
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
        public IActionResult Index()
        {
            try
            {
                List<CommentListVM> vm = new List<CommentListVM>();

                var commentList = _commentService.GetAll();

                foreach (var item in commentList)
                {
                    var comment = new CommentListVM();
                    comment.Id = item.Id;
                    comment.CompanyName = item.Person.Company.CompanyName;
                    comment.CompanyManagerName = item.Person.FirstName + " " + item.Person.LastName;
                    comment.PersonId = item.PersonId;
                    comment.Department = item.Person.Department.Value.GetDisplayName();
                    comment.Status= item.Status;
                    comment.Summary = item.Summary;

                    if (item.Photo != null)
                    {
                        comment.Picture = $"data:image/png;base64,{Convert.ToBase64String(item.Photo)}";
                    }

                    vm.Add(comment);
                }

                var cmVm = _mapper.Map<List<CommentListVM>>(vm);
                return View(cmVm);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
        public IActionResult Detail(Guid id)
        {
            // Çalışanı id'ye göre al
            var commentDetail = _commentService.GetByEntity(x => x.Id == id);

            // Görünüm modelini oluştur
            var commentDetailVM = _mapper.Map<CommentDetailVM>(commentDetail);
            commentDetailVM.CompanyName = commentDetail.Person.Company.CompanyName;
            commentDetailVM.CompanyManagerName = commentDetail.Person.FirstName + " " + commentDetail.Person.LastName;
            commentDetailVM.PersonId = commentDetail.PersonId;
            commentDetailVM.Department = commentDetail.Person.Department.GetDisplayName();
            commentDetailVM.Status = commentDetail.Status.GetDisplayName();
            commentDetailVM.CommentText = commentDetail.CommentText;
            commentDetailVM.Title = commentDetail.Title;

            if (commentDetail.Person.Company.Logo != null)
            {
                commentDetailVM.LogoPicture = $"data:image/png;base64,{Convert.ToBase64String(commentDetail.Person.Company.Logo)}";
            }
            // Profil resmi varsa, base64 formatına dönüştürerek görünüm modeline ekle
            if (commentDetail.Photo != null)
            {
                commentDetailVM.Picture = $"data:image/png;base64,{Convert.ToBase64String(commentDetail.Photo)}";
            }

            return View(commentDetailVM);
        }

    }
}
