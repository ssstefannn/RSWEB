using Microsoft.AspNetCore.Mvc.Rendering;
using RSWEB.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RSWEB.ViewModels
{
    public class StudentEditViewModel
    {
        public Student student { get; set; }

        public IEnumerable<long>? selectedCourses { get; set; }

        public IEnumerable<SelectListItem>? coursesEnrolledList { get; set; }

        [Display(Name = "Year")]
        public int? year { get; set; }

        [Display(Name = "Semester")]
        public string? semester { get; set; }

        public string? profilePictureName { get; set; }
    }
}
