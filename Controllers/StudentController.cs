using Microsoft.AspNetCore.Mvc;
using RSWEB.Data;
using RSWEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RSWEB.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;

namespace RSWEB.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string FullName, string StudentId)
        {
            IQueryable<Student> studentsQuery = _context.Students.AsQueryable();
            if (!string.IsNullOrEmpty(FullName))
            {
                if (FullName.Contains(" "))
                {
                    string[] names = FullName.Split(" ");
                    studentsQuery = studentsQuery.Where(x => x.FirstName.Contains(names[0]) || x.LastName.Contains(names[1]) ||
                    x.FirstName.Contains(names[1]) || x.LastName.Contains(names[0]));
                }
                else
                {
                    studentsQuery = studentsQuery.Where(x => x.FirstName.Contains(FullName) || x.LastName.Contains(FullName));
                }
            }
            if (!string.IsNullOrEmpty(StudentId))
            {
                studentsQuery = studentsQuery.Where(x => x.StudentId.Contains(StudentId));
            }
            var StudentFilterVM = new StudentFilterViewModel
            {
                students = await studentsQuery.ToListAsync()
            };

            return View(StudentFilterVM);
        }

        public IActionResult Create()
        {
            ViewData["Courses"] = new SelectList(_context.Set<Course>(), "Id", "Title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StudentId,FirstName,LastName,EnrollmentDate,AcquiredCredits,CurrentSemester,EducationLevel")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var student = await _context.Students.FindAsync(id);
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            StudentEditPictureViewModel viewmodel = new StudentEditPictureViewModel
            {
                student = student,
                profilePictureName = student.profilePicture
            };

            return View(viewmodel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = _context.Students.Where(x => x.Id == id).Include(x => x.Enrollments).First();
            if (student == null)
            {
                return NotFound();
            }

            var courses = _context.Courses.AsEnumerable();
            courses = courses.OrderBy(s => s.Title);

            StudentEditViewModel viewmodel = new StudentEditViewModel
            {
                student = student,
                coursesEnrolledList = new MultiSelectList(courses, "Id", "Title"),
                selectedCourses = student.Enrollments.Select(x => x.Id)
            };

            ViewData["Courses"] = new SelectList(_context.Set<Course>(), "Id", "Title");
            return View(viewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, StudentEditViewModel viewmodel)
        {
            if (id != viewmodel.student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viewmodel.student);
                    await _context.SaveChangesAsync();

                    var student = _context.Students.Where(x => x.Id == id).First();

                    IEnumerable<long> selectedCourses = viewmodel.selectedCourses;
                    if (selectedCourses != null)
                    {
                        IQueryable<Enrollment> toBeRemoved = _context.Enrollments.Where(s => !selectedCourses.Contains(s.CourseId) && s.StudentId == id);
                        _context.Enrollments.RemoveRange(toBeRemoved);

                        IEnumerable<long> existEnrollments = (IEnumerable<long>)_context.Enrollments.Where(s => selectedCourses.Contains(s.CourseId) && s.StudentId == id).Select(s => s.CourseId);
                        IEnumerable<long> newEnrollments = selectedCourses.Where(s => !existEnrollments.Contains(s));

                        foreach (int courseId in newEnrollments)
                            _context.Enrollments.Add(new Enrollment { StudentId = id, CourseId = courseId, Semester = viewmodel.semester, Year = viewmodel.year });

                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        IQueryable<Enrollment> toBeRemoved = _context.Enrollments.Where(s => s.StudentId == id);
                        _context.Enrollments.RemoveRange(toBeRemoved);
                        await _context.SaveChangesAsync();
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(viewmodel.student.Id))
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
            return View(viewmodel);
        }

        private bool StudentExists(System.Int64 id)
        {
            return _context.Students.Any(e => e.Id == id);
        }

        public async Task<IActionResult> StudentsEnrolled(int? id, string? fullName, string? studentId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .FirstOrDefaultAsync(m => m.Id == id);
            ViewBag.Message = course.Title;
            IQueryable<Student> studentsQuery = _context.Enrollments.Where(x => x.CourseId == id).Select(x => x.Student);
            await _context.SaveChangesAsync();
            if (course == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(fullName))
            {
                if (fullName.Contains(" "))
                {
                    string[] names = fullName.Split(" ");
                    studentsQuery = studentsQuery.Where(x => x.FirstName.Contains(names[0]) || x.LastName.Contains(names[1]) ||
                    x.FirstName.Contains(names[1]) || x.LastName.Contains(names[0]));
                }
                else
                {
                    studentsQuery = studentsQuery.Where(x => x.FirstName.Contains(fullName) || x.LastName.Contains(fullName));
                }
            }
            if (!string.IsNullOrEmpty(studentId))
            {
                studentsQuery = studentsQuery.Where(x => x.Id.Equals(studentId));
            }

            var StudentFilterVM = new StudentFilterViewModel
            {
                students = await studentsQuery.ToListAsync(),
            };

            return View(StudentFilterVM);
        }

        public async Task<IActionResult> EditPicture(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = _context.Students.Where(x => x.Id == id).Include(x => x.Enrollments).First();
            if (student == null)
            {
                return NotFound();
            }

            var courses = _context.Courses.AsEnumerable();
            courses = courses.OrderBy(s => s.Title);

            StudentEditPictureViewModel viewmodel = new StudentEditPictureViewModel
            {
                student = student,
                profilePictureName = student.profilePicture
            };

            return View(viewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPicture(long id, StudentEditPictureViewModel viewmodel)
        {
            if (id != viewmodel.student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (viewmodel.profilePictureFile != null)
                    {
                        string uniqueFileName = UploadedFile(viewmodel);
                        viewmodel.student.profilePicture = uniqueFileName;
                    }
                    else
                    {
                        viewmodel.student.profilePicture = viewmodel.profilePictureName;
                    }

                    _context.Update(viewmodel.student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(viewmodel.student.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", new { id = viewmodel.student.Id });
            }
            return View(viewmodel);
        }

        private string UploadedFile(StudentEditPictureViewModel viewmodel)
        {
            string uniqueFileName = null;

            if (viewmodel.profilePictureFile != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/profilePictures");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(viewmodel.profilePictureFile.FileName);
                string fileNameWithPath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    viewmodel.profilePictureFile.CopyTo(stream);
                }
            }
            return uniqueFileName;
        }
    }

}