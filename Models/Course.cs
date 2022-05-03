using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace RSWEB.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Credits")]
        public int Credits { get; set; }

        [Required]
        [Display(Name = "Semester")]
        public int Semester { get; set; }

        [StringLength(100)]
        [Display(Name = "Programme")]
        public string? Programme { get; set; }

        [StringLength(25)]
        [Display(Name = "Education Level")]
        public string? EducationLevel { get; set; }

        public int? FirstTeacherId { get; set; }
        [Display(Name = "First Teacher")]
        public Teacher? FirstTeacher { get; set; }

        public int? SecondTeacherId { get; set; }
        [Display(Name = "Second Teacher")]
        public Teacher? SecondTeacher { get; set; }

        public ICollection<Enrollment>? Enrollments { get; set; }
    }
}
