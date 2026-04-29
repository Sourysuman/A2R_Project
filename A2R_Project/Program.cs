using A2R_Project.Context;
using A2R_Project.Controllers;
using A2R_Project.Interface;
using A2R_Project.Interfaces;
using A2R_Project.Models;
using A2R_Project.Repositories;
using A2R_Project.Repositories.Interfaces;
using A2R_Project.Repository;
using A2R_Project.Services;
using A2RSystemInterface;
using A2RSystemWebApp.Interfaces;
using A2RSystemWebApp.Repository;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<AppDbContext>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IStateRepository, StateRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();

builder.Services.AddScoped<A2R_Project.Interface.IExpenseCategoryRepository, A2R_Project.Repositories.ExpenseCategoryRepository>();
builder.Services.AddScoped<IExpenseRepository,ExpenseRepository>();
builder.Services.AddScoped<IStudentInquiryRepository, StudentInquiryRepository>();
builder.Services.AddScoped<IBranchRepository, BranchRepository>();

builder.Services.AddScoped<IFollowUpRepository, FollowUpRepository>();
builder.Services.AddScoped<IAdmissionRepository, AdmissionRepository>();
builder.Services.AddScoped<A2R_Project.Interfaces.IExpenseReportRepository, A2R_Project.Repository.ExpenseReportRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<ILoginRepository,LoginRepository>();
builder.Services.AddScoped<IAccessControl, AccessControlRepository>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IPermissionService, PermissionService>(); 
builder.Services.AddMemoryCache();

// Program.cs - ADD THESE 6 LINES
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();  // 🔥 MISSING - REQUIRED for Session!
// AFTER app.UseRouting() ADD:

var app = builder.Build();

app.UseExceptionHandler("/Error");   // 👈 move outside

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=AdminLogin}/{action=Login}/{id?}");

app.Run();
