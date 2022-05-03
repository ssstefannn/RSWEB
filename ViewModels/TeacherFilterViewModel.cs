using Microsoft.AspNetCore.Mvc.Rendering;
using RSWEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RSWEB.ViewModels
{
    public class TeacherFilterViewModel
    {
        public IList<Teacher> teachers { get; set; }

        public string FullName { get; set; }

        public string AcademicRank { get; set; }

        public string Degree { get; set; }
    }
}
