using Microsoft.AspNetCore.Http;
using RSWEB.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RSWEB.ViewModels
{
    public class EditAsStudentViewModel
    {
        public Enrollment enrollment { get; set; }

        [Display(Name = "Seminal File")]
        public IFormFile? seminalUrlFile { get; set; }

        public string? seminalUrlName { get; set; }
    }
}
