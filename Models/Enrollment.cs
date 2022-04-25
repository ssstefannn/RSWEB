using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace RSWEB.Models
{
    public class Enrollment
    {
        [Key]
        public System.Int64 Id { get; set; }

        public int? CourseId { get; set; }
        [Required]
        [Display(Name = "Course")]
        public Course? Course { get; set; }

        public System.Int64? StudentId { get; set; }
        [Required]
        [Display(Name = "Student")]
        public Student? Student { get; set; }

        [Display(Name = "Semester")]
        [StringLength(10)]
        public string Semester { get; set; }

        [Display(Name = "Year")]
        public int Year { get; set; }

        [Display(Name = "Grade")]
        public int Grade { get; set; }

        [Display(Name = "Seminal URL")]
        [StringLength(255)]
        public string SeminalUrl { get; set; }

        [Display(Name = "Project URL")]
        [StringLength(255)]
        public string ProjectUrl { get; set; }

        [Display(Name = "Exam Points")]
        public int ExamPoints { get; set; }

        [Display(Name = "Seminal Points")]
        public int SeminalPoints { get; set; }

        [Display(Name = "Project Points")]
        public int ProjectPoints { get; set; }

        [Display(Name = "Additional Points")]
        public int AdditionalPoints { get; set; }

        [Display(Name = "Finish Date")]
        [DataType(DataType.Date)]
        public DateTime FinishDate { get; set; }

    }

}
