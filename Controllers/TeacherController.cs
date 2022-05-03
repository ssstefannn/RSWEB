using Microsoft.AspNetCore.Mvc;
using RSWEB.Data;
using RSWEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using RSWEB.ViewModels;

namespace RSWEB.Controllers
{
    public class TeacherController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TeacherController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string FullName, string AcademicRank, string Degree)
        {
            IQueryable<Teacher> teachersQuery = _context.Teachers.AsQueryable();
            if (!string.IsNullOrEmpty(FullName))
            {
                if (FullName.Contains(" "))
                {
                    string[] names = FullName.Split(" ");
                    teachersQuery = teachersQuery.Where(x => x.FirstName.Contains(names[0]) || x.LastName.Contains(names[1]) ||
                    x.FirstName.Contains(names[1]) || x.LastName.Contains(names[0]));
                }
                else
                {
                    teachersQuery = teachersQuery.Where(x => x.FirstName.Contains(FullName) || x.LastName.Contains(FullName));
                }
                if (!string.IsNullOrEmpty(AcademicRank))
                {
                    teachersQuery = teachersQuery.Where(x => x.AcademicRank.Contains(AcademicRank));
                }
                if (!string.IsNullOrEmpty(Degree))
                {
                    teachersQuery = teachersQuery.Where(x => x.Degree.Contains(Degree));
                }
            }
            if (!string.IsNullOrEmpty(AcademicRank))
            {
                teachersQuery = teachersQuery.Where(x => x.AcademicRank.Contains(AcademicRank));
            }
            if (!string.IsNullOrEmpty(Degree))
            {
                teachersQuery = teachersQuery.Where(x => x.Degree.Contains(Degree));
            }
            var TeacherFilterVM = new TeacherFilterViewModel
            {
                teachers = await teachersQuery.ToListAsync()
            };
            return View(TeacherFilterVM);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Degree,AcademicRank,OfficeNumber,HireDate")] Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                _context.Add(teacher);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(teacher);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            IQueryable<Course> coursesQuery = _context.Courses.AsQueryable();

            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null)
            {
                return NotFound();
            }
            coursesQuery = coursesQuery.Where(x => x.FirstTeacherId.Equals(id) || x.SecondTeacherId.Equals(id));
            var TeacherDetailsVM = new TeacherDetailsViewModel
            {
                teacher = teacher,
                courses = await coursesQuery.ToListAsync()
            };
            return View(TeacherDetailsVM);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }
            return View(teacher);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Degree,AcademicRank,OfficeNumber,HireDate")] Teacher teacher)
        {
            if (id != teacher.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teacher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(teacher.Id))
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
            return View(teacher);
        }

        private bool TeacherExists(int id)
        {
            return _context.Teachers.Any(e => e.Id == id);
        }
    }

}
