using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SICProject.Models;

namespace SICProject.Controllers
{
    public class AdminController : Controller
    {
        private readonly SicdbContext _db;
        private readonly IMapper _mapper;
        public AdminController(SicdbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public IActionResult ListOfDepartment()
        {
            var departments = _db.Departmentmasters.ToList();
            return View(departments);
        }

        public IActionResult AddDepartment()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddDepartment(DepartmentmasterVM departmentmaster)
        {
            if (ModelState.IsValid)
            {
                Departmentmaster department = new Departmentmaster();
                department = _mapper.Map<Departmentmaster>(departmentmaster);
                _db.Departmentmasters.Add(department);
                _db.SaveChanges();
                if (department.DepartmentId > 0)
                {
                    TempData["success"] = "Department Added Successfully";
                }
                else
                {
                    TempData["error"] = "Department Addition Failed";
                }
                return RedirectToAction("ListOfDepartment");
            }
            TempData["error"] = "Please check all fields carefully.";
            return View(departmentmaster);
        }
        public IActionResult Edit(int id)
        {
            var department = _db.Departmentmasters.FirstOrDefault(d => d.DepartmentId == id);
            if (department == null)
            {
                return NotFound();
            }
            var departmentVM = _mapper.Map<DepartmentmasterVM>(department);
            return View(departmentVM);
         
        }
        public IActionResult ListOfStudent()
        {
            var students = _db.Registrationmasters.ToList();
            List<RegistrationmasterVM> stdList = _mapper.Map(students, new List<RegistrationmasterVM>());
            foreach (var student in stdList)
            {
                var department = _db.Departmentmasters.FirstOrDefault(d => d.DepartmentId == student.DepartmentId);
                if (department != null)
                {
                    student.DepartmentName = department.DepartmentName;
                }
            }
            return View(stdList);
        }

        [HttpPost]
        public IActionResult ToggleApproval(int id, bool isApproved)
        {
            var student = _db.Registrationmasters.FirstOrDefault(x => x.RegistrationId == id);
            if (student == null)
            {
                return NotFound();
            }

            student.IsApproved = isApproved ? 1UL : 0UL; 
            _db.SaveChanges();

            return Json(new { success = true, message = "Approval status updated." });
        }
    }
}
