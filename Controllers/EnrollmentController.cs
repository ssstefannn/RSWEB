using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RSWEB.Data;
using RSWEB.Models;
using RSWEB.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RSWEB.Controllers
{
    public class EnrollmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EnrollmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            IQueryable<Enrollment> enrollmentsQuery = _context.Enrollments.AsQueryable().Include(e => e.Course).Include(e => e.Student);
            return View(await enrollmentsQuery.ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.CourseId = new SelectList(_context.Courses, "Id", "Title");
            ViewBag.StudentId = new SelectList(_context.Students, "Id", "FullName");
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CourseId,StudentId,Semester,Year,Grade,SeminalUrl,ProjectUrl,ExamPoints,SeminalPoints,ProjectPoints,AdditionalPoints,FinishDate")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(enrollment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(enrollment);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            ViewBag.CourseId = new SelectList(_context.Courses, "Id", "Title", enrollment.CourseId);
            ViewData["studentId"] = new SelectList(_context.Students, "Id", "FullName", enrollment.StudentId);
            return View(enrollment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,CourseId,StudentId,Semester,Year,Grade,SeminalUrl,ProjectUrl,ExamPoints,SeminalPoints,ProjectPoints,AdditionalPoints,FinishDate")] Enrollment enrollment)
        {
            if (id != enrollment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(enrollment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(enrollment);
        }

        private bool EnrollmentExists(long id)
        {
            return _context.Enrollments.Any(e => e.Id == id);
        }

        public async Task<IActionResult> StudentsEnrolledAtCourse(int? id, string teacher, int year)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .FirstOrDefaultAsync(m => m.Id == id);

            string[] names = teacher.Split(" ");
            var teacherModel = await _context.Teachers.FirstOrDefaultAsync(m => m.FirstName == names[0] && m.LastName == names[1]);
            ViewBag.teacher = teacher;
            ViewBag.course = course.Title;
            ViewBag.courseId = course.Id;
            var enrollment = _context.Enrollments.Where(x => x.CourseId == id && (x.Course.FirstTeacherId == teacherModel.Id || x.Course.SecondTeacherId == teacherModel.Id))
            .Include(e => e.Course)
            .Include(e => e.Student);
            await _context.SaveChangesAsync();
            IQueryable<int?> yearsQuery = _context.Enrollments.OrderBy(m => m.Year).Select(m => m.Year).Distinct();
            IQueryable<Enrollment> enrollmentQuery = enrollment.AsQueryable();
            if (year != null && year != 0)
            {
                enrollmentQuery = enrollmentQuery.Where(x => x.Year == year);
            }
            else
            {
                enrollmentQuery = enrollmentQuery.Where(x => x.Year == yearsQuery.Max());
            }

            if (enrollment == null)
            {
                return NotFound();
            }

            EnrollmentFilterViewModel viewmodel = new EnrollmentFilterViewModel
            {
                enrollments = await enrollmentQuery.ToListAsync(),
                yearsList = new SelectList(await yearsQuery.ToListAsync())
            };

            return View(viewmodel);
        }

        //teacher role
        public async Task<IActionResult> EditAsTeacher(long? id, string teacher)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            ViewBag.teacher = teacher;
            ViewBag.courseId = new SelectList(_context.Courses, "Id", "Title", enrollment.
                CourseId);
            ViewBag.studentId = new SelectList(_context.Students, "Id", "FullName", enrollment.StudentId);
            return View(enrollment);
        }

        //teacher role
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsTeacher(long id, string teacher, [Bind("Id,CourseId,StudentId,Semester,Year,Grade,SeminalUrl,ProjectUrl,ExamPoints,SeminalPoints,ProjectPoints,AdditionalPoints,FinishDate")] Enrollment enrollment)
        {
            if (id != enrollment.Id)
            {
                return NotFound();
            }
            string temp = teacher;
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(enrollment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("StudentsEnrolledAtCourse", new { id = enrollment.CourseId, teacher = temp, year = enrollment.Year });
            }
            ViewData["courseId"] = new SelectList(_context.Courses, "CourseId", "Title", enrollment.CourseId);
            ViewData["studentId"] = new SelectList(_context.Students, "Id", "FirstName", enrollment.StudentId);
            return View(enrollment);
        }

        //student role
        public async Task<IActionResult> StudentCourses(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.Id == id);

            ViewBag.student = student.FullName;

            IQueryable<Enrollment> enrollment = _context.Enrollments.Where(x => x.StudentId == id)
            .Include(e => e.Course)
            .Include(e => e.Student);
            await _context.SaveChangesAsync();

            if (enrollment == null)
            {
                return NotFound();
            }

            return View(await enrollment.ToListAsync());
        }

        //student role
        public async Task<IActionResult> EditAsStudent(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = _context.Enrollments.Where(m => m.Id == id).Include(x => x.Student).Include(x => x.Course).First();
            IQueryable<Enrollment> enrollmentQuery = _context.Enrollments.AsQueryable();
            enrollmentQuery = enrollmentQuery.Where(m => m.Id == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            EditAsStudentViewModel viewmodel = new EditAsStudentViewModel
            {
                enrollment = await enrollmentQuery.Include(x => x.Student).Include(x => x.Course).FirstAsync(),
                seminalUrlName = enrollment.SeminalUrl
            };
            ViewData["courseId"] = new SelectList(_context.Courses, "Id", "Title", enrollment.CourseId);
            ViewData["studentId"] = new SelectList(_context.Students, "Id", "FirstName", enrollment.StudentId);
            return View(viewmodel);
        }

        //student role
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsStudent(long id, EditAsStudentViewModel viewmodel)
        {
            if (id != viewmodel.enrollment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    if (viewmodel.seminalUrlFile != null)
                    {
                        string uniqueFileName = UploadedFile(viewmodel);
                        viewmodel.enrollment.SeminalUrl = uniqueFileName;
                    }
                    else
                    {
                        viewmodel.enrollment.SeminalUrl = viewmodel.seminalUrlName;
                    }

                    _context.Update(viewmodel.enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(viewmodel.enrollment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("StudentCourses", new { id = viewmodel.enrollment.StudentId });
            }

            ViewData["courseId"] = new SelectList(_context.Courses, "Id", "Title", viewmodel.enrollment.CourseId);
            ViewData["studentId"] = new SelectList(_context.Students, "Id", "FirstName", viewmodel.enrollment.StudentId);
            return View(viewmodel);
        }

        private string UploadedFile(EditAsStudentViewModel viewmodel)
        {
            string uniqueFileName = null;

            if (viewmodel.seminalUrlFile != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/seminals");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(viewmodel.seminalUrlFile.FileName);
                string fileNameWithPath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    viewmodel.seminalUrlFile.CopyTo(stream);
                }
            }
            return uniqueFileName;
        }
    }
}
