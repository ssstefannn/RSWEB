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
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using RSWEB.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;

namespace RSWEB.Controllers
{
    public class TeacherController : Controller
    {
        private readonly ApplicationDbContext _context;
        public IServiceProvider _serviceProvider;

        public TeacherController(ApplicationDbContext context,IServiceProvider serviceProvider)
        {
            _context = context;
            _serviceProvider = serviceProvider;
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Degree,AcademicRank,OfficeNumber,HireDate")] Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                _context.Add(teacher);
                await _context.SaveChangesAsync();
                var UserManager = _serviceProvider.GetRequiredService<UserManager<RSWEBUser>>();
                var User = new RSWEBUser();
                User.Email = "teacher"+teacher.Id+"@rsweb.com";
                User.UserName = "teacher" + teacher.Id + "@rsweb.com";
                User.LinkId = teacher.Id;
                string userPWD = "Teacher"+teacher.Id;
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                if (chkUser.Succeeded) { var result1 = await UserManager.AddToRoleAsync(User, "Teacher"); }
                return RedirectToAction(nameof(Index));
            }
            
            return View(teacher);
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            IQueryable<Course> courses = _context.Courses.AsQueryable();
            IQueryable<Course> courses1 = courses.Where(x => x.FirstTeacherId == teacher.Id);
            IQueryable<Course> courses2 = courses.Where(x => x.SecondTeacherId == teacher.Id);
            foreach (var course in courses1)
            {
                course.FirstTeacherId = null;
            }
            foreach (var course in courses2)
            {
                course.SecondTeacherId = null;
            }
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
                profilePictureName = teacher.profilePicture
            };
            return View(TeacherDetailsVM);
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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

        public async Task<IActionResult> EditPicture(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = _context.Teachers.Where(x => x.Id == id).First();
            if (teacher == null)
            {
                return NotFound();
            }

            TeacherDetailsViewModel viewmodel = new TeacherDetailsViewModel
            {
                teacher = teacher,
                profilePictureName = teacher.profilePicture
            };

            return View(viewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPicture(long id, TeacherDetailsViewModel viewmodel)
        {
            if (id != viewmodel.teacher.Id)
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
                        viewmodel.teacher.profilePicture = uniqueFileName;
                    }
                    else
                    {
                        viewmodel.teacher.profilePicture = viewmodel.profilePictureName;
                    }

                    _context.Update(viewmodel.teacher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(viewmodel.teacher.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", new { id = viewmodel.teacher.Id });
            }
            return View(viewmodel);
        }

        private string UploadedFile(TeacherDetailsViewModel viewmodel)
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
