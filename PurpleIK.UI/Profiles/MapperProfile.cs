using AutoMapper;
using Microsoft.Extensions.Hosting;
using PurpleIK.Entities;

using PurpleIK.UI.Areas.Admin.Models.VM.PermissionVM;
using PurpleIK.UI.Areas.Admin.Models.VM.AdminVM;
using PurpleIK.UI.Areas.CompanyManager.Models.VM.EmployeeManagerVM;
using PurpleIK.UI.Areas.CompanyManager.Models.VM.PushMoneyVM;
using PurpleIK.UI.Areas.CompanyManager.Models.VM.DebitVM;
using PurpleIK.UI.Models.VM.AccountVM;
using PurpleIK.UI.Areas.CompanyManager.Models.VM.PersonalInformationVM;
using PurpleIK.UI.Areas.CompanyManager.Models.VM.CommentVM;
using PurpleIK.UI.Areas.Employee.Models.VM.DebitEmployeeVM;
using PurpleIK.UI.Areas.Admin.Models.VM.MembershipVM;
using PurpleIK.UI.Models.VM.CommentVM;
using PurpleIK.UI.Areas.CompanyManager.Models.VM.CompanyVM;
using PurpleIK.UI.Areas.CompanyManager.Models.VM.CompanyMembershipVM;
using PurpleIK.UI.Areas.Employee.Models.VM.EmployeePermision;
using PurpleIK.UI.Areas.CompanyManager.Models.VM.PermissionVM;
using PurpleIK.UI.Areas.CompanyManager.Models.VM.ShiftVM;
using PurpleIK.UI.Areas.CompanyManager.Models.VM.ExpenseVM;
using PurpleIK.UI.Areas.Employee.Models.VM.ExpenseEmployeeVM;

namespace PurpleIK.UI.Profiles
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            this.CreateMap<Company, CompanyRegisterVM>().ReverseMap();            
            this.CreateMap<Company, CompanyListVM>().ReverseMap();
            this.CreateMap<Company, CompanyEditVM>().ReverseMap();

            this.CreateMap<Person, EmployeeAddVM>().ReverseMap();
            this.CreateMap<Person, EmployeeListVM>().ReverseMap();
            this.CreateMap<Person, ForgotPasswordVM>().ReverseMap();
            this.CreateMap<Person, EmployeeEditVM>().ReverseMap();

            this.CreateMap<Comment, CommentIndexVM>().ReverseMap();
            this.CreateMap<Comment, CommentAddVM>().ReverseMap();
            this.CreateMap<Comment, CommentRejectVM>().ReverseMap();
            this.CreateMap<Comment, CommentEditVM>().ReverseMap();
            this.CreateMap<Comment, CommentListVM>().ReverseMap();
            this.CreateMap<Comment, CommentDetailVM>().ReverseMap();
            this.CreateMap<Person, PersonalSettingsVM>().ReverseMap();

            this.CreateMap<PushMoney, PushMoneyIndexVM>().ReverseMap();
            this.CreateMap<PushMoney, PushMoneyAddItem>().ReverseMap();
            this.CreateMap<PushMoney, PushMoneyEditVM>().ReverseMap();

            this.CreateMap<Permission, IndexPermissionItem>().ReverseMap();
            this.CreateMap<Permission, AddPermissionVM>().ReverseMap();
            this.CreateMap<Permission, EditPermissonVM>().ReverseMap();
            
            this.CreateMap<Debit, DebitIndexVM>().ReverseMap();
            this.CreateMap<Debit, DebitAddItem>().ReverseMap();
            this.CreateMap<Debit, DebitEditVM>().ReverseMap();
            this.CreateMap<Debit, DebitEmployeeIndexVM>().ReverseMap();

            this.CreateMap<PersonalInformation, PersonalInformationIndexVM>().ReverseMap();
            this.CreateMap<PersonalInformation, PersonalInformationAddItem>().ReverseMap();
            this.CreateMap<PersonalInformation, PersonalInformationEditVM>().ReverseMap();

            this.CreateMap<PersonPermission, EmployeePermissionsListVM>().ReverseMap();
            this.CreateMap<PersonPermission, EmployeePermissionsAddItem>().ReverseMap();
            this.CreateMap<PersonPermission, EmployeePermissionsEditVM>().ReverseMap();

            this.CreateMap<Expense, ExpenseIndexVM>().ReverseMap();
            this.CreateMap<Expense, ExpenseAddVM>().ReverseMap();
            this.CreateMap<Expense, ExpenseAddItem>().ReverseMap();
            this.CreateMap<Person, IndexItem>().ReverseMap();
            this.CreateMap<Expense, ExpenseEmployeeIndexVM>().ReverseMap();
            this.CreateMap<Expense, ExpenseEmployeeAddVM>().ReverseMap();
            this.CreateMap<Expense, ExpenseEmployeeAddItem>().ReverseMap();


            this.CreateMap<Membership, MembershipAddVM>().ReverseMap();
            this.CreateMap<Membership, MembershipEditVM>().ReverseMap();
            this.CreateMap<Membership, MembershipIndexVM>().ReverseMap();

            this.CreateMap<CompanyMembership, MembershipListVM>().ReverseMap();

            this.CreateMap<PersonPermission, ManagerPermissionIndexVM>().ReverseMap();
            this.CreateMap<PersonPermission, ManagerPermissionAddItem>().ReverseMap();
            this.CreateMap<PersonPermission, ManagerPermissionEditVM>().ReverseMap();

            this.CreateMap<Shift, ShiftIndexVM>().ReverseMap();
            this.CreateMap<Shift, ShiftAddVM>().ReverseMap();


        }
    }
}
