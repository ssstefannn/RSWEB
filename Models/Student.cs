using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace RSWEB.Models
{
    public class Student
    {
        [Key]
        public System.Int64 Id { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "Student ID")]
        public string StudentId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Enrollment Date")]
        [DataType(DataType.Date)]
        public DateTime? EnrollmentDate { get; set; }

        [Display(Name = "Acquired Credits")]
        public int? AcquiredCredits { get; set; }

        [Display(Name = "Current Semester")]
        public int? CurrentSemester { get; set; }

        [Display(Name = "Education Level")]
        [StringLength(25)]
        public string? EducationLevel { get; set; }

        public ICollection<Enrollment>? Enrollments { get; set; }

        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", FirstName, LastName);
            }
        }
    }
}
