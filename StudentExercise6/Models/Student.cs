using System.Collections.Generic;
using StudentExercise6.Models;

namespace StudentExercise6.Models
{
    public class Student
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2)]

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string SlackHandle { get; set; }

        [Required]
        public int CohortId { get; set; }


        //?: Why doesn't Cohort need to have a [Required] tag?
        public Cohort Cohort { get; set; }

        public List<Exercise> Exercises { get; set; } = new List<Exercise>();
        public string StudentFirstName { get; internal set; }
    }
}