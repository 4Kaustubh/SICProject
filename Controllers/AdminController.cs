using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SICProject.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using System.Linq;


namespace SICProject.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly SicdbContext _db;
        private readonly IMapper _mapper;
        public AdminController(SicdbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        //------------------------------Booking start------------------------------//
        public IActionResult SelectInstrumentOrStudent()
        {
            ViewBag.Instruments = _db.Instrumentsmasters.Select(i => new SelectListItem { Value = i.InstrumentsId.ToString(), Text = i.InstrumentName })
                .ToList();
            ViewBag.Students = _db.Registrationmasters.Select(s => new SelectListItem { Value = s.RegistrationId.ToString(), Text = s.StudentName })
                .ToList();
            return View();
        }

        [HttpPost]
        public IActionResult ListOfBooking(int? InstrumentId, int? StudentId)
        {
            if ((InstrumentId == null || InstrumentId == 0) && (StudentId == null || StudentId == 0))
            {
                TempData["error"] = "Please select at least one option: Instrument or Student.";
                return RedirectToAction("SelectInstrumentOrStudent");
            }

            var bookings = _db.Bookingmasters
                .Where(b =>
                    (InstrumentId == null || InstrumentId == 0 || b.InstrumentId == InstrumentId) &&
                    (StudentId == null || StudentId == 0 || b.StudentId == StudentId))
                .ToList();

            // Load instruments and students dictionaries for lookup
            var instruments = _db.Instrumentsmasters
                .ToDictionary(i => i.InstrumentsId, i => i.InstrumentName);

            var students = _db.Registrationmasters
                .ToDictionary(s => s.RegistrationId, s => s.StudentName);

            var bookingVMs = bookings.Select(b => new BookingmasterVM
            {
                BookingId = b.BookingId,
                InstrumentId = b.InstrumentId,
                InstrumentName = (b.InstrumentId.HasValue && instruments.ContainsKey(b.InstrumentId.Value))
                       ? instruments[b.InstrumentId.Value]
                       : "Unknown",
                StudentId = b.StudentId,
                StudentName = (b.StudentId.HasValue && students.ContainsKey(b.StudentId.Value))
                    ? students[b.StudentId.Value]
                    : "Unknown",
                Remarks = b.Remarks,
                BookingDate = b.BookingDate,
                SlotStart = b.SlotStart,
                SlotEnd = b.SlotEnd
            }).ToList();

            return View(bookingVMs);
        }

    //------------------------------Booking end------------------------------//
    //---------------------------- Department start---------------------------//
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
        //---------------------------- Department end---------------------------//
        //---------------------------- Student start---------------------------//
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
            var Departments = _db.Departmentmasters.ToList();
            ViewBag.Departments = new SelectList(Departments, "DepartmentId", "DepartmentName");
            var student = _db.Registrationmasters.FirstOrDefault(s => s.RegistrationId == id);
            if (student == null)
            {
                return NotFound();
            }

            var studentVM = _mapper.Map<RegistrationmasterVM>(student);
            studentVM.ConfirmAppEmailId = student.Email;
            return View("EditStudent", studentVM);

        }
        [HttpPost]
        public IActionResult EditStudent(RegistrationmasterVM registrationmaster)
        {
            var std = _db.Registrationmasters.FirstOrDefault(s => s.RegistrationId == registrationmaster.RegistrationId);
            var Department = _db.Departmentmasters.FirstOrDefault(x => x.DepartmentId == registrationmaster.DepartmentId);
            if (Department != null)
            {
                registrationmaster.DepartmentName = Department.DepartmentName;
            }
            if (std != null)
            {
                registrationmaster.Password = std.Password;
                registrationmaster.Remarks = std.Remarks;
                registrationmaster.IsApproved = std.IsApproved == null ? (bool?)null :
                                  std.IsApproved == 1 ? true :
                                  std.IsApproved == 0 ? false : (bool?)null;

            }
            registrationmaster.ConfirmAppEmailId = registrationmaster.Email;

            if (ModelState.IsValid)
            {
                var student = _db.Registrationmasters.FirstOrDefault(s => s.RegistrationId == registrationmaster.RegistrationId);
                if (student == null)
                {
                    return NotFound();
                }

                // ✅ Correct mapping: into the existing tracked entity
                _mapper.Map(registrationmaster, student);

                _db.SaveChanges();

                TempData["Success"] = "Student updated successfully!";
                return RedirectToAction("ListOfStudent");
            }

            var Departments = _db.Departmentmasters.ToList();
            ViewBag.Departments = new SelectList(Departments, "DepartmentId", "DepartmentName");
            TempData["Error"] = "Please check all fields carefully.";
            return View(registrationmaster);
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
        //---------------------------- Student end---------------------------//
        //--------------------------- Instruments start---------------------------//
        public IActionResult ListOfInstruments()
        {
            var instruments = _db.Instrumentsmasters.ToList();
            return View(instruments);
        }
        public IActionResult AddInstruments()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddInstruments(InstrumentsmasterVM instrumentsmaster)
        {
            if (ModelState.IsValid)
            {
                Instrumentsmaster instruments = new Instrumentsmaster();
                instruments = _mapper.Map<Instrumentsmaster>(instrumentsmaster);
                _db.Instrumentsmasters.Add(instruments);
                _db.SaveChanges();
                if (instruments.InstrumentsId > 0)
                {
                    TempData["success"] = "Instrument Added Successfully";
                }
                else
                {
                    TempData["error"] = "Instrument Addition Failed";
                }
                return RedirectToAction("ListOfInstruments");
            }
            TempData["error"] = "Please check all fields carefully.";
            return View(instrumentsmaster);
        }

        public IActionResult EditInstruments(int id)
        {
            var instruments = _db.Instrumentsmasters.FirstOrDefault(d => d.InstrumentsId == id);
            if (instruments == null)
            {
                return NotFound();
            }

            var instrumentsVM = _mapper.Map<InstrumentsmasterVM>(instruments);
            return View(instrumentsVM);
        }
        [HttpPost]
        public IActionResult EditInstruments(InstrumentsmasterVM instrumentsmaster)
        {
            if (ModelState.IsValid)
            {
                var instruments = _db.Instrumentsmasters.FirstOrDefault(d => d.InstrumentsId == instrumentsmaster.InstrumentsId);
                if (instruments == null)
                {
                    return NotFound();
                }

                // ✅ Correct mapping: into the existing tracked entity
                _mapper.Map(instrumentsmaster, instruments);

                _db.SaveChanges();

                TempData["Success"] = "Instrument updated successfully!";
                return RedirectToAction("ListOfInstruments");
            }

            TempData["Error"] = "Please check all fields carefully.";
            return View(instrumentsmaster);
        }
        public IActionResult DeleteInstruments(int id)
        {
            var instruments = _db.Instrumentsmasters.FirstOrDefault(d => d.InstrumentsId == id);
            if (instruments == null)
            {
                return NotFound();
            }
            _db.Instrumentsmasters.Remove(instruments);
            _db.SaveChanges();
            TempData["success"] = "Instrument Deleted Successfully";
            return RedirectToAction("ListOfInstruments");
        }
        //--------------------------- Instruments end---------------------------//
        //----------------------------Holiday start----------------------------//
        public IActionResult ListOfHoliday()
        {
            var holidays = _db.Holidaymasters.ToList();
            return View(holidays);
        }

        public IActionResult AddHoliday()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddHoliday(HolidaymasterVM holidaymaster)
        {
            if (ModelState.IsValid)
            {
                Holidaymaster holiday = new Holidaymaster();
                holiday = _mapper.Map<Holidaymaster>(holidaymaster);
                _db.Holidaymasters.Add(holiday);
                _db.SaveChanges();
                if (holiday.HolidayId > 0)
                {
                    TempData["success"] = "Holiday Added Successfully";
                }
                else
                {
                    TempData["error"] = "Holiday Addition Failed";
                }
                return RedirectToAction("ListOfHoliday");
            }
            TempData["error"] = "Please check all fields carefully.";
            return View(holidaymaster);
        }
        public IActionResult EditHoliday(int id)
        {
            var holiday = _db.Holidaymasters.FirstOrDefault(d => d.HolidayId == id);
            if (holiday == null)
            {
                return NotFound();
            }

            var holidayVM = _mapper.Map<HolidaymasterVM>(holiday);
            return View(holidayVM);
        }
        [HttpPost]
        public IActionResult EditHoliday(HolidaymasterVM holidaymaster)
        {
            if (ModelState.IsValid)
            {
                var holiday = _db.Holidaymasters.FirstOrDefault(d => d.HolidayId == holidaymaster.HolidayId);
                if (holiday == null)
                {
                    return NotFound();
                }

                // ✅ Correct mapping: into the existing tracked entity
                _mapper.Map(holidaymaster, holiday);

                _db.SaveChanges();

                TempData["Success"] = "Holiday updated successfully!";
                return RedirectToAction("ListOfHoliday");
            }

            TempData["Error"] = "Please check all fields carefully.";
            return View(holidaymaster);
        }
        public IActionResult DeleteHoliday(int id)
        {
            var holiday = _db.Holidaymasters.FirstOrDefault(d => d.HolidayId == id);
            if (holiday == null)
            {
                return NotFound();
            }
            _db.Holidaymasters.Remove(holiday);
            _db.SaveChanges();
            TempData["success"] = "Holiday Deleted Successfully";
            return RedirectToAction("ListOfHoliday");
        }
        //-----------------------------Holiday end----------------------------//
    }
}
