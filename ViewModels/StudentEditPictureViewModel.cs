using Microsoft.AspNetCore.Http;
using RSWEB.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RSWEB.ViewModels
{
    public class StudentEditPictureViewModel
    {

        public Student? student { get; set; }

        [Display(Name = "Upload picture")]
        public IFormFile? profilePictureFile { get; set; }

        [Display(Name = "Picture name")]
        public string? profilePictureName { get; set; }
    }
}
