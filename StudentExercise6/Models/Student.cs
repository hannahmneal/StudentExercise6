using StudentExercise5_WebAPI.Models;
using System.Collections.Generic;

namespace StudentExercise5_WebAPI.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SlackHandle { get; set; }
        public int CohortId { get; set; }
        public Cohort Cohort { get; set; }
        public List<Exercise> Exercises { get; set; } = new List<Exercise>();
        public string StudentFirstName { get; internal set; }
    }
}