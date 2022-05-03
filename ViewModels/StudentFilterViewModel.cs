using RSWEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RSWEB.ViewModels
{
    public class StudentFilterViewModel
    {
        public IList<Student> students { get; set; }

        public string FullName { get; set; }

        public string StudentId { get; set; }
    }
}
