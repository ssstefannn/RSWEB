using Microsoft.AspNetCore.Mvc;
using RSWEB.Data;
using RSWEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RSWEB.ViewModels;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RSWEB.Controllers
{
    public class CourseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CourseController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string Title, int Semester, string Programme)
        {
            IQueryable<Course> coursesQuery = _context.Courses.AsQueryable();
            if (!string.IsNullOrEmpty(Title))
            {
                coursesQuery = coursesQuery.Where(x => x.Title.Contains(Title));
            }
            if (Semester != null && Semester != 0)
            {
                coursesQuery = coursesQuery.Where(s => s.Semester == Semester);
            }
            if (!string.IsNullOrEmpty(Programme))
            {
                coursesQuery = coursesQuery.Where(p => p.Programme == Programme);
            }
            var CoursefilterVM = new CourseFilterViewModel
            {
                courses = await coursesQuery.Include(c => c.FirstTeacher).Include(c => c.SecondTeacher).ToListAsync()
            };

            return View(CoursefilterVM);
        }

        public IActionResult Create()
        {
            var Teachers = _context.Teachers.Select(teacher => teacher).ToList();

            List<SelectListItem> teacherIDs = Teachers.ConvertAll(a =>
            {
                return new SelectListItem()
                {
                    Text = a.FullName,
                    Value = a.Id.ToString(),
                    Selected = false
                };
            });
            ViewBag.TeacherIDs = teacherIDs;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Credits,Semester,Programme,EducationLevel,FirstTeacherId,SecondTeacherId")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(course);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.Include(c => c.FirstTeacher).Include(c => c.SecondTeacher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }
            IQueryable<Enrollment> enrollmentsQuery = _context.Enrollments.AsQueryable();
            IQueryable<Student> studentsQuery = _context.Students.AsQueryable();
            enrollmentsQuery = enrollmentsQuery.Where(x => x.CourseId.Equals(id));
            var query = studentsQuery.Join(enrollmentsQuery,
                    student => student.Id,
                    enrollment => enrollment.StudentId,
                    (student,enrollment) =>
                        student);
            var CourseDetailsVM = new CourseDetailsViewModel
            {
                course = course,
                students = await query.ToListAsync()
            };
            return View(CourseDetailsVM);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = _context.Courses.Where(m => m.Id == id).Include(x => x.Enrollments).First();
            IQueryable<Course> coursesQuery = _context.Courses.AsQueryable();
            coursesQuery = coursesQuery.Where(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }
            var students = _context.Students.AsEnumerable();
            students = students.OrderBy(s => s.FullName);

            CourseEditViewModel viewmodel = new CourseEditViewModel
            {
                course = await coursesQuery.Include(c => c.FirstTeacher).Include(c => c.SecondTeacher).FirstAsync(),
                studentsEnrolledList = new MultiSelectList(students, "Id", "FullName"),
                selectedStudents = course.Enrollments.Select(sa => sa.StudentId)
            };

            ViewBag.TeacherId = new SelectList(_context.Set<Teacher>(), "Id", "FullName");
            //ViewData["Students"] = new SelectList(_context.Set<Student>(), "Id", "fullName", course.enrollments);
            return View(viewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CourseEditViewModel viewmodel)
        {
            if (id != viewmodel.course.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viewmodel.course);
                    await _context.SaveChangesAsync();

                    var course = _context.Courses.Where(m => m.Id == id).First();
                    string Semester;
                    if (course.Semester % 2 == 0)
                    {
                        Semester = "leten";
                    }
                    else
                    {
                        Semester = "zimski";
                    }

                    IEnumerable<long> selectedStudents = viewmodel.selectedStudents;
                    if (selectedStudents != null)
                    {
                        IQueryable<Enrollment> toBeRemoved = _context.Enrollments.Where(s => !selectedStudents.Contains(s.StudentId) && s.CourseId == id);
                        _context.Enrollments.RemoveRange(toBeRemoved);

                        IEnumerable<long> existEnrollments = _context.Enrollments.Where(s => selectedStudents.Contains(s.StudentId) && s.CourseId == id).Select(s => s.StudentId);
                        IEnumerable<long> newEnrollments = selectedStudents.Where(s => !existEnrollments.Contains(s));

                        foreach (int StudentId in newEnrollments)
                            _context.Enrollments.Add(new Enrollment { StudentId = StudentId, CourseId = id, Semester = Semester, Year = viewmodel.year });

                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        IQueryable<Enrollment> toBeRemoved = _context.Enrollments.Where(s => s.CourseId == id);
                        _context.Enrollments.RemoveRange(toBeRemoved);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(viewmodel.course.Id))
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

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }
    }

}
