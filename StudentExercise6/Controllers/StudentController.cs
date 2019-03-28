using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentExercise6.Models;
using Microsoft.Extensions.Configuration;   //This line is how we reference the IConfiguration interface. It represents a set of key/value application configuration properties


namespace StudentExercise6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        //---------------------------------------------------------------------------------------------------------------

        //NOTE: This is how we set up connections to the databse before now:    
        // GET: api/Students
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //NOTE: This is how we do it from now on (note, that the Connection String in appsettings.json does not change):
        private readonly IConfiguration configuration;
        //In order to get IConfiguration to work, you have to put in a using statement at the top of the file: using Microsoft.Extensions.Configuration;
        //It is an interface that represents a set of key/value application configuration

        public StudentController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
                //NOTE:"Default Connection" is what we named our string; it is the key and the Connection String is the value, so we're simply referencing it by key here.
            }
        }

//---------------------------------------------------------------------------------------------------------------

        // GET: api/Students/?q=hannah&include=exercise

        [HttpGet]
        public IEnumerable<Student>
        //NOTE: IEnumerable exposes the enumerator, which supports a simple iteration over a collection of a specified type. We access it with: using System.Collections.Generic
        using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    if (include == "exercise")
                    {
                        cmd.CommandText = @"select s.id as StudentId,
                                               s.StudentFirstName,
                                               s.StudentLastName,
                                               s.StudentSlackHandle,
                                               s.student_cohort_id,
                                               c.[Name] as CohortName,
                                               e.id as ExerciseId,
                                               e.[name] as ExerciseName,
                                               e.[Language]
                                          from student s
                                               left join Cohort c on s.CohortId = c.id
                                               left join StudentExercise se on s.id = se.studentid
                                               left join Exercise e on se.exerciseid = e.id
                                         WHERE 1 = 1";
                    }
                    else
                    {
                        cmd.CommandText = @"select s.id as StudentId,
                                               s.FirstName,
                                               s.LastName,
                                               s.SlackHandle,
                                               s.CohortId,
                                               c.[Name] as CohortName
                                          from student s
                                               left join Cohort c on s.CohortId = c.id
                                         WHERE 1 = 1";
                    }

                    if (!string.IsNullOrWhiteSpace(q))
                    {
                        cmd.CommandText += @" AND 
                                             (s.FirstName LIKE @q OR
                                              s.LastName LIKE @q OR
                                              s.SlackHandle LIKE @q)";
                        cmd.Parameters.Add(new SqlParameter("@q", $"%{q}%"));
                    }

                    SqlDataReader reader = cmd.ExecuteReader();

Dictionary<int, Student> students = new Dictionary<int, Student>();
                    while (reader.Read())
                    {
                        int studentId = reader.GetInt32(reader.GetOrdinal("StudentId"));
                        if (!students.ContainsKey(studentId))
                        {
                            Student newStudent = new Student
                            {
                                Id = studentId,
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                                CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                Cohort = new Cohort
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                    Name = reader.GetString(reader.GetOrdinal("CohortName"))
                                }
                            };

students.Add(studentId, newStudent);
                        }

                        if (include == "exercise")
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("ExerciseId")))
                            {
                                Student currentStudent = students[studentId];
currentStudent.Exercises.Add(
                                    new Exercise
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("ExerciseId")),
                                        Language = reader.GetString(reader.GetOrdinal("Language")),
                                        Name = reader.GetString(reader.GetOrdinal("ExerciseName")),
                                    }
                                );
                            }
                        }
                    }

                    reader.Close();

                    return students.Values.ToList();
                }
            }
        }

//---------------------------------------------------------------------------------------------------------------

//---------------------------------------------------------------------------------------------------------------

        // POST: api/Students
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }
//---------------------------------------------------------------------------------------------------------------

        // PUT: api/Students/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }
//---------------------------------------------------------------------------------------------------------------

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
//---------------------------------------------------------------------------------------------------------------

    }
}