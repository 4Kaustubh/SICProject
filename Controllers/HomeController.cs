using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using SICProject.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace SICProject.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SicdbContext _db;
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger, SicdbContext db, IMapper mapper)
        {
            _logger = logger;
            _db = db;
            _mapper = mapper;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult Registration()
        {
            // Get the list of Departments from the database
            var Departments = _db.Departmentmasters.ToList();
            ViewBag.Departments = new SelectList(Departments, "DepartmentId", "DepartmentName");

            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Registration(RegistrationmasterVM registrationmaster)
        {
            var Departments = _db.Departmentmasters.ToList();
            ViewBag.Departments = new SelectList(Departments, "DepartmentId", "DepartmentName");

            if (ModelState.IsValid)
            {
                Registrationmaster reg = new Registrationmaster();
                reg = _mapper.Map<Registrationmaster>(registrationmaster);


                reg.Password = CreatePassword();
                reg.DepartmentName = _db.Departmentmasters.FirstOrDefault(d => d.DepartmentId == registrationmaster.DepartmentId)?.DepartmentName;

                _db.Registrationmasters.Add(reg);
                _db.SaveChanges();
                if (reg.RegistrationId > 0)
                {
                    TempData["success"] = "Registration Successful";
                }
                else
                {
                    TempData["error"] = "Registration Failed";
                }

                return RedirectToAction("Index");
            }
            TempData["error"] = "Registration please check all field carefully.";
            return View(registrationmaster);
        }
        private string CreatePassword()
        {
            string prefix = "RE";
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            string suffix = new string(Enumerable.Range(0, 6).Select(x => chars[random.Next(chars.Length)]).ToArray());
            return prefix + suffix;
        }

        [AllowAnonymous]
        public IActionResult login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult login(loginVM std)
        {

            var studentUser = _db.Registrationmasters.FirstOrDefault(x => x.Email == std.Email && x.Password == std.AppPwd);
            // Replace with your user validation logic
            if (studentUser != null && !string.IsNullOrWhiteSpace(studentUser.Password))
            {

                if (studentUser.IsApproved == 0)
                {
                    TempData["Error"] = "Dear, Your Accounts is not approved now.";
                    return View();
                }

                // Set user session
                HttpContext.Session.SetString("UserSession", studentUser.StudentName ?? string.Empty);
                HttpContext.Session.SetString("User", studentUser.Email ?? string.Empty);
                HttpContext.Session.SetString("AppPwd", studentUser.Password?.ToString() ?? string.Empty);

                // Cookie Authentication example (optional)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, studentUser.StudentName??string.Empty)
                };
                //var userRoles = db.TblUsersRoles.Where(x => x.UserId == myUser.Id).ToList();
                //// Add role claims 
                //if (userRoles.Any())
                //{
                //    foreach (var role in userRoles)
                //    {
                //        claims.Add(new Claim(ClaimTypes.Role, role.Role));
                //    }
                //}
                var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = false  // if we want store claims data persistent than set true value
                };
                HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties).Wait();
                return RedirectToAction("dashboard", "Home");
            }
            else
            {
                TempData["Error"] = "Invalid Username Id or Password";
                return View();
            }
        }

        public IActionResult dashboard()
        {
            var userSession = HttpContext.Session.GetString("UserSession");
            var userEmail = HttpContext.Session.GetString("User");
            var userPwd = HttpContext.Session.GetString("AppPwd");
            if (string.IsNullOrEmpty(userSession) || string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(userPwd))
            {
                return RedirectToAction("login", "Home");
            }
            var studentUser = _db.Registrationmasters.FirstOrDefault(x => x.Email == userEmail && x.Password == userPwd);
            if (studentUser == null)
            {
                return RedirectToAction("login", "Home");
            }
            // Get the list of All Booking as per student from the database
            var AllBooking = _db.Bookingmasters.Where(x => x.StudentId == studentUser.RegistrationId).ToList();
            ViewBag.AllBooking = AllBooking;
            return View();
        }
        public IActionResult Logout()
        {

            HttpContext.Session.Clear();
            HttpContext.SignOutAsync("CookieAuth").Wait();
            HttpContext.User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity());
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
            return RedirectToAction("login", "Home");
        }

        [AllowAnonymous]
        public IActionResult forgetPassword()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult forgetPassword(loginVM user)
        {
            if (user == null)
            {
                TempData["Error"] = "Please enter your email address.";
                return View();
            }
            var studentUser = _db.Registrationmasters.FirstOrDefault(x => x.Email == user.Email && x.MobileNumber==user.AppMobileNumber);
            if (studentUser != null)
            {
                // Send email logic here
                // For example, you can use SMTP to send an email with the password reset link
                TempData["success"] = "Your Password is " + studentUser.Password;
                return RedirectToAction("login", "Home");
            }
            else
            {
                TempData["Error"] = "User not found. Contacts to Admin. Thanks";
            }
            return View();
        }
        [AllowAnonymous]
        public IActionResult changePassword()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult changePassword(loginVM user)
        {
            if (user == null)
            {
                TempData["Error"] = "Please enter your email address.";
                return View();
            }

            var studentUser = _db.Registrationmasters.FirstOrDefault(x =>
                x.Email == user.Email &&
                x.MobileNumber == user.AppMobileNumber &&
                x.Password == user.AppPwd);

            if (studentUser != null)
            {
                // Assign the new password
                studentUser.Password = user.AppPwdNew;

                _db.Registrationmasters.Update(studentUser);
                _db.SaveChanges();

                TempData["Success"] = "Your password has been changed successfully.";
                return RedirectToAction("Login", "Home");
            }
            else
            {
                TempData["Error"] = "User not found. Please contact the admin.";
            }

            return View();
        }


        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
