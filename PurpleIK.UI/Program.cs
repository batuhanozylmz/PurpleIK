using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PdfSharp.Charting;
using PurpleIK.Context;
using PurpleIK.Entities;
using PurpleIK.Services.Concretes;
using PurpleIK.Services.Interfaces;
using PurpleIK.UI.Profiles;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<PurpleDbContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("Con")));

//TODO: Mapper iþlemi
builder.Services.AddAutoMapper(typeof(MapperProfile));

builder.Services.AddIdentity<AppUser, AppRole>(x =>
{
    x.SignIn.RequireConfirmedEmail = true;
    x.SignIn.RequireConfirmedPhoneNumber = false;
    x.SignIn.RequireConfirmedAccount = false;

    x.User.RequireUniqueEmail = true;

    x.Password.RequiredLength = 3;
    x.Password.RequireUppercase = false;
    x.Password.RequireLowercase = false;
    x.Password.RequireNonAlphanumeric = false;
    x.Password.RequiredUniqueChars = 0;




}).AddEntityFrameworkStores<PurpleDbContext>().AddDefaultTokenProviders();

//TODO:ýnterface ve clasllarý otomatik oluþturma
builder.Services.AddTransient<ICompanyService, CompanyService>();
builder.Services.AddTransient<IPersonService, PersonService>();
builder.Services.AddTransient<IDebitService, DebitService>();
builder.Services.AddTransient<ICommentService, CommentService>();
builder.Services.AddTransient<IMembershipService, MembershipService>();
builder.Services.AddTransient<ICompanyMembershipService, CompanyMembershipService>();
builder.Services.AddTransient<IShiftService, ShiftService>();
builder.Services.AddTransient<IExpenseService, ExpenseService>();






builder.Services.AddTransient<IPublicHolidaysService, PublicHolidaysService>();
builder.Services.AddTransient<IPermissionService, PermissionService>();
builder.Services.AddTransient<IPushMoneyService, PushMoneyService>();
builder.Services.AddTransient<IPersonalInformationService, PersonalInformationService>();
builder.Services.AddTransient<IPersonPermissionService, PersonPermissionService>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Account/Login"; // Eriþim reddedildiðinde yönlendirilecek sayfa
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    endpoints.MapFallbackToController("ErrorPage", "Home"); // Belirli bir URL için 404 hatasý yönetimi
});

app.Run();
