using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SICProject.Models;
using System.Diagnostics;

namespace SICProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SicdbContext _db;
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger, SicdbContext db,IMapper mapper)
        {
            _logger = logger;
            _db = db;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Registration()
        {
            // Get the list of Departments from the database
            var Departments = _db.Departmentmasters.ToList();
            ViewBag.Departments = new SelectList(Departments, "DepartmentId", "DepartmentName");

            return View();
        }
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
                if (reg.RegistrationId >0)
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

        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
