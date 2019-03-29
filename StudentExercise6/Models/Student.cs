using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using StudentExercise6.Models;

namespace StudentExercise6.Models
{
    public class Student
    {
        public int Id { get; set; }

        //? What is this?
        [Required]
        [StringLength(50, MinimumLength = 2)]

        //[Required]
        public string StudentFirstName { get; set; }

        //[Required]
        public string StudentLastName { get; set; }

        //[Required]
        public string StudentSlackHandle { get; set; }

        //[Required]
        public int student_cohort_id { get; set; }




        //?: Why doesn't Cohort need to have a [Required] tag?
        public Cohort Cohort { get; set; }

        public List<Exercise> Exercise { get; set; } = new List<Exercise>();
    }
}