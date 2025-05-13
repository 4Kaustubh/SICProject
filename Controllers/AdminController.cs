using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SICProject.Models;
using Microsoft.AspNetCore.Mvc.Rendering;


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

        [HttpPost]
        public IActionResult Edit(DepartmentmasterVM departmentmaster)
        {
            if (ModelState.IsValid)
            {
                var department = _db.Departmentmasters.FirstOrDefault(d => d.DepartmentId == departmentmaster.DepartmentId);
                if (department == null)
                {
                    return NotFound();
                }

                // ✅ Correct mapping: into the existing tracked entity
                _mapper.Map(departmentmaster, department);

                _db.SaveChanges();

                TempData["Success"] = "Department updated successfully!";
                return RedirectToAction("ListOfDepartment");
            }

            TempData["Error"] = "Please check all fields carefully.";
            return View(departmentmaster);
        }
        public IActionResult Delete(int id)
        {
            var department = _db.Departmentmasters.FirstOrDefault(d => d.DepartmentId == id);
            if (department == null)
            {
                return NotFound();
            }
            _db.Departmentmasters.Remove(department);
            _db.SaveChanges();
            TempData["success"] = "Department Deleted Successfully";
            return RedirectToAction("ListOfDepartment");
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
        public IActionResult EditStudent(int id)
        {
            var student = _db.Registrationmasters.FirstOrDefault(s => s.RegistrationId == id);
            if (student == null)
            {
                return NotFound();
            }

            var studentVM = _mapper.Map<RegistrationmasterVM>(student);
            ViewBag.Departments = new SelectList(_db.Departmentmasters.ToList(), "DepartmentId", "DepartmentName", student.DepartmentId);

            return View("EditStudent", studentVM);

        }
        [HttpPost]
        public IActionResult EditStudent(RegistrationmasterVM studentVM)
        {
            if (ModelState.IsValid)
            {
                var student = _db.Registrationmasters.FirstOrDefault(s => s.RegistrationId == studentVM.RegistrationId);
                if (student == null)
                {
                    return NotFound();
                }

                // Map updated values into existing entity
                _mapper.Map(studentVM, student);

                _db.SaveChanges();

                TempData["Success"] = "Student updated successfully!";
                return RedirectToAction("ListOfStudent");
            }

            // Repopulate departments in case of error
            ViewBag.Departments = new SelectList(_db.Departmentmasters.ToList(), "DepartmentId", "DepartmentName", studentVM.DepartmentId);
            TempData["Error"] = "Please check all fields carefully.";
            return View("EditStudent", studentVM);

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
