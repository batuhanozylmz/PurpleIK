using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PurpleIK.Core;
using PurpleIK.Entities;
using PurpleIK.Services.Interfaces;
using PurpleIK.UI.Models;
using PurpleIK.UI.Models.VM.CommentVM;
using System.Diagnostics;

namespace PurpleIK.UI.Controllers
{
    public class HomeController : Controller
    {
        ICommentService _commentService;
        UserManager<AppUser> _userManager;
        IPersonService _personService;
        IMapper _mapper;
        public HomeController(ICommentService service, IPersonService personService, IMapper mapper, UserManager<AppUser> userManager)
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
                    comment.Status = item.Status;
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
        public IActionResult Apps()
        {
            return View();
        }
        public IActionResult Pages()
        {
            return View();
        }
        public IActionResult AboutUs()
        {
            return View();
        }
        public IActionResult ErrorPage()
        {
            return View();
        }

    }
}
