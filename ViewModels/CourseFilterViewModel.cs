using RSWEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RSWEB.ViewModels
{
    public class CourseFilterViewModel
    {
        public IList<Course> courses { get; set; }

        public string Programme { get; set; }

        public string Title { get; set; }

        public int Semester { get; set; }
    }
}
