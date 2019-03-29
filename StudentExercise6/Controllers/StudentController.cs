using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentExercise6.Models;
using Microsoft.Extensions.Configuration;   //This line is how we reference the IConfiguration interface. It represents a set of key/value application configuration properties


//---------------------------------------------------------------------------------------------------------------
//---------------------------------------------------------------------------------------------------------------
namespace StudentExercise6.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public StudentsController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            }
        }

        // GET: api/Students?q=joe&include=exercise
        [HttpGet]
        public IEnumerable<Student> Get(string include, string q)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    if (include == "exercise")
                    {

                        // StudentId = student_id
                        // ExerciseId = assigned_exercise_id
                        // CohortId = student_cohort_id


                        cmd.CommandText = @"SELECT s.Id,
                                               s.StudentFirstName,
                                               s.StudentLastName,
                                               s.StudentSlackHandle,
                                               s.student_cohort_id,
                                               c.CohortName,
                                               e.id,
                                               e.ExerciseName,
                                               e.ExerciseLanguage
                                          from Student s
                                               left join Cohort c ON s.student_cohort_id = c.id
                                               left join StudentExercise se ON s.id = se.student_id
                                               left join Exercise e ON se.assigned_exercise_id = e.id
                                         WHERE 1 = 1";
                    }
                    else
                    {
                        cmd.CommandText = @"SELECT s.Id,
                                               s.StudentFirstName,
                                               s.StudentLastName,
                                               s.StudentSlackHandle,
                                               s.student_cohort_id,
                                               c.CohortName
                                          FROM Student s
                                               LEFT JOIN Cohort c ON s.student_cohort_id = c.id
                                         WHERE 1 = 1";
                    }
                    if (!string.IsNullOrWhiteSpace(q))
                    {
                        cmd.CommandText += @" AND 
                                             (s.StudentFirstName LIKE @q OR
                                              s.StudentLastName LIKE @q OR
                                              s.StudentSlackHandle LIKE @q)";
                        cmd.Parameters.Add(new SqlParameter("@q", $"%{q}%"));
                    }

                    SqlDataReader reader = cmd.ExecuteReader();

                    Dictionary<int, Student> students = new Dictionary<int, Student>();
                    while (reader.Read())
                    {
                        int Id = reader.GetInt32(reader.GetOrdinal("Id"));
                        if (!students.ContainsKey(Id))
                        {
                            Student newStudent = new Student
                            {
                                Id = Id,
                                StudentFirstName = reader.GetString(reader.GetOrdinal("studentFirstName")),
                                StudentLastName = reader.GetString(reader.GetOrdinal("studentLastName")),
                                StudentSlackHandle = reader.GetString(reader.GetOrdinal("studentSlackHandle")),
                                student_cohort_id = reader.GetInt32(reader.GetOrdinal("student_cohort_id")),
                                Cohort = new Cohort
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    CohortName = reader.GetString(reader.GetOrdinal("cohortName"))
                                }
                            };

                            students.Add(Id, newStudent);
                        }
                        if (include == "Exercise")
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("assigned_exercise_id")))
                            {
                                Student currentStudent = students[Id];
                                currentStudent.Exercise.Add(
                                    new Exercise
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                        ExerciseLanguage = reader.GetString(reader.GetOrdinal("exerciseLanguage")),
                                        ExerciseName = reader.GetString(reader.GetOrdinal("exerciseName")),
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

        // GET: api/Students/5?include=exercise
        [HttpGet("{id}", Name = "GetSingleStudent")]
        public Student Get(int id, string include)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    if (include == "Exercise")
                    {
                        cmd.CommandText = @"SELECT s.id,
                                               s.StudentFirstName,
                                               s.StudentLastName,
                                               s.StudentSlackHandle,
                                               s.student_cohort_id,
                                               c.CohortName,
                                               e.id AS ExerciseId,
                                               e.ExerciseName,
                                               e.ExerciseLanguage
                                          FROM Student s
                                               LEFT JOIN Cohort c on s.CohortId = c.id
                                               LEFT JOIN StudentExercise se on s.id = se.studentid
                                               LEFT JOIN Exercise e on se.exerciseid = e.id";
                    }
                    else
                    {
                        cmd.CommandText = @"SELECT s.id,
                                               s.StudentFirstName,
                                               s.StudentLastName,
                                               s.StudentSlackHandle,
                                               s.student_cohort_id,
                                               c.CohortName
                                          FROM Student s
                                               LEFT JOIN Cohort c on s.student_cohort_id = c.id";
                    }
                    cmd.CommandText += " WHERE s.id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    Student student = null;
                    while (reader.Read())
                    {
                        if (student == null)
                        {
                            student = new Student
                            {
                                Id = id,
                                StudentFirstName = reader.GetString(reader.GetOrdinal("StudentFirstName")),
                                StudentLastName = reader.GetString(reader.GetOrdinal("StudentLastName")),
                                StudentSlackHandle = reader.GetString(reader.GetOrdinal("StudentSlackHandle")),
                                student_cohort_id = reader.GetInt32(reader.GetOrdinal("student_cohort_id")),
                                Cohort = new Cohort
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("student_cohort_id")),
                                    CohortName = reader.GetString(reader.GetOrdinal("CohortName"))
                                }
                            };
                        }

                        if (include == "exercise")
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("ExerciseId")))
                            {
                                student.Exercise.Add(
                                    new Exercise
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                        ExerciseLanguage = reader.GetString(reader.GetOrdinal("ExerciseLanguage")),
                                        ExerciseName = reader.GetString(reader.GetOrdinal("ExerciseName")),
                                    }
                                );
                            }
                        }
                    }

                    reader.Close();
                    return student;
                }
            }
        }
        // POST: api/Students
        [HttpPost]
        public ActionResult Post([FromBody] Student newStudent)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Student (studentFirstName, studentLastName, studentSlackHandle, student_cohort_id)
                                             OUTPUT INSERTED.Id
                                             VALUES (@studentFirstName, @studentLastName, @studentSlackHandle, @student_cohort_id)";
                    cmd.Parameters.Add(new SqlParameter("@studentFirstName", newStudent.StudentFirstName));
                    cmd.Parameters.Add(new SqlParameter("@studentLastName", newStudent.StudentLastName));
                    cmd.Parameters.Add(new SqlParameter("@studentSlacHandle", newStudent.StudentSlackHandle));
                    cmd.Parameters.Add(new SqlParameter("@student_cohort_id", newStudent.student_cohort_id));

                    int newId = (int)cmd.ExecuteScalar();
                    newStudent.Id = newId;
                    return CreatedAtRoute("GetSingleStudent", new { id = newId }, newStudent);
                }
            }
        }
        // PUT: api/Students/5
        [HttpPut("{id}")]
        public ActionResult Put([FromBody] Student student)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Student
                                             SET 
                                                StudentFirstName = @studentFirstName,
                                                StudentLastName = @studentLastName,
                                                StudentSlackHandle = @studentSlackHandle,
                                                student_cohort_id = @student_cohort_id
                                                
                                             WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", student.Id));
                    cmd.Parameters.Add(new SqlParameter("@studentFirstName", student.StudentFirstName));
                    cmd.Parameters.Add(new SqlParameter("@studentLastName", student.StudentLastName));
                    cmd.Parameters.Add(new SqlParameter("@studentSlacHandle", student.StudentSlackHandle));
                    cmd.Parameters.Add(new SqlParameter("@student_cohort_id", student.student_cohort_id));

                    int rowsAffected = cmd.ExecuteNonQuery();
                    student.Id = rowsAffected;
                    return CreatedAtRoute("GetSingleStudent", new { id = student.Id }, student);
                }
            }
        }
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}