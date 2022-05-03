using RSWEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RSWEB.ViewModels
{
    public class CourseDetailsViewModel
    {
        public Course course { get; set; }

        public IList<Student> students { get; set; }
    }
}
