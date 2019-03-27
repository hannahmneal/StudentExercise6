using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudentExercise5_WebAPI.Models;


namespace StudentExercise5_WebAPI.Models
{
    public class Instructor
    {
        public int Id { get; set; }
        public string InstructorFirstName { get; set; }
        public string InstructorLastName { get; set; }
        public string InstructorSlackHandle { get; set; }
        public int instructor_cohort_id { get; set; }

        //NOTE: VS prompted me to add a property or internal field for "Cohort = new Cohort" when getting all instructors with cohorts. I chose to add a property: 
        public Cohort Cohort { get; internal set; }
    }
}
