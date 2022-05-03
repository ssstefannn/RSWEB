using Microsoft.AspNetCore.Mvc.Rendering;
using RSWEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RSWEB.ViewModels
{
    public class CourseEditViewModel
    {
        public Course course { get; set; }

        public IEnumerable<long>? selectedStudents { get; set; }

        public IEnumerable<SelectListItem>? studentsEnrolledList { get; set; }

        public int? year { get; set; }
    }
}
