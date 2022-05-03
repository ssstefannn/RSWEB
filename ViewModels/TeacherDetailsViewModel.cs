using RSWEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RSWEB.ViewModels
{
    public class TeacherDetailsViewModel
    {
        public Teacher teacher { get; set; }

        public IList<Course> courses { get; set; }
    }
}
