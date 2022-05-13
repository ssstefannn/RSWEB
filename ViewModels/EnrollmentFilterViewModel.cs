using Microsoft.AspNetCore.Mvc.Rendering;
using RSWEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RSWEB.ViewModels
{
    public class EnrollmentFilterViewModel
    {
        public List<Enrollment> enrollments { get; set; }

        public SelectList yearsList { get; set; }
        public int year { get; set; }
    }
}
