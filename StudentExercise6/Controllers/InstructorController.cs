
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentExercise5_WebAPI.Models;

namespace StudentExercise5_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorController : ControllerBase
    {
        public SqlConnection Connection
        {
            get
            {
                string connectionString = "Server=HNEAL-PC\\SQLEXPRESS;Database=StudentExercise3;Integrated Security=true";
                return new SqlConnection(connectionString);
            }
        }

        // GET: api/Instructors
        [HttpGet]
        public IEnumerable<Instructor> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT i.Id, i.InstructorFirstName, i.InstructorLastName,
                                               i.InstructorSlackHandle, i.instructor_cohort_id, c.CohortName
                                          FROM Instructor i INNER JOIN Cohort c ON i.instructor_cohort_id = c.Id";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Instructor> instructors = new List<Instructor>();
                    while (reader.Read())
                    {
                        Instructor instructor = new Instructor
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            InstructorFirstName = reader.GetString(reader.GetOrdinal("InstructorFirstName")),
                            InstructorLastName = reader.GetString(reader.GetOrdinal("InstructorLastName")),
                            InstructorSlackHandle = reader.GetString(reader.GetOrdinal("InstructorSlackHandle")),
                            instructor_cohort_id = reader.GetInt32(reader.GetOrdinal("instructor_cohort_id")),
                            Cohort = new Cohort
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("instructor_cohort_id")),
                                CohortName = reader.GetString(reader.GetOrdinal("CohortName"))
                            }
                        };

                        instructors.Add(instructor);
                    }

                    reader.Close();
                    return instructors;
                }
            }
        }

        // GET: api/Instructors/5
        [HttpGet("{id}", Name = "GetInstructor")]
        public Instructor Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT i.id, i.InstructorFirstName, i.InstructorLastName,
                                               i.InstructorSlackHandle, i.instructor_cohort_id, c.CohortName
                                          FROM Instructor i INNER JOIN Cohort c ON i.instructor_cohort_id = c.id
                                         WHERE i.id = @id;";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Instructor instructor = null;
                    if (reader.Read())
                    {
                        instructor = new Instructor
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            InstructorFirstName = reader.GetString(reader.GetOrdinal("InstructorFirstName")),
                            InstructorLastName = reader.GetString(reader.GetOrdinal("InstructorLastName")),
                            InstructorSlackHandle = reader.GetString(reader.GetOrdinal("InstructorSlackHandle")),
                            instructor_cohort_id = reader.GetInt32(reader.GetOrdinal("instructor_cohort_id")),
                            Cohort = new Cohort
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("cohortid")),
                                CohortName = reader.GetString(reader.GetOrdinal("cohortname"))
                            }
                        };
                    }

                    reader.Close();
                    return instructor;
                }
            }
        }

        // POST: api/Instructors
        [HttpPost]
        public ActionResult Post([FromBody] Instructor newInstructor)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO instructor (InstructorFirstName, InstructorLastName, InstructorSlackHandle, instructor_cohort_id)
                                             OUTPUT INSERTED.Id
                                             VALUES (@InstructorFirstName, @InstructorLastName, @InstructorSlackHandle, @instructor_cohort_id)";
                    cmd.Parameters.Add(new SqlParameter("@InstructorFirstName", newInstructor.InstructorFirstName));
                    cmd.Parameters.Add(new SqlParameter("@InstructorLastName", newInstructor.InstructorLastName));
                    cmd.Parameters.Add(new SqlParameter("@InstructorSlackHandle", newInstructor.InstructorSlackHandle));
                    cmd.Parameters.Add(new SqlParameter("@instructor_cohort_id", newInstructor.instructor_cohort_id));

                    int newId = (int)cmd.ExecuteScalar();
                    newInstructor.Id = newId;
                    return CreatedAtRoute("GetInstructor", new { id = newId }, newInstructor);
                }
            }
        }

        // PUT: api/Instructors/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Instructor instructor)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE instructor 
                                           SET firstname = @InstructorFirstName, 
                                               lastname = @InstructorLastName,
                                               slackhandle = @InstructorSlackHandle, 
                                               cohortid = @instructor_cohort_id
                                         WHERE id = @id;";
                    cmd.Parameters.Add(new SqlParameter("@InstructorFirstName", instructor.InstructorFirstName));
                    cmd.Parameters.Add(new SqlParameter("@InstructorLastName", instructor.InstructorLastName));
                    cmd.Parameters.Add(new SqlParameter("@InstructorSlackHandle", instructor.InstructorSlackHandle));
                    cmd.Parameters.Add(new SqlParameter("@instructor_cohort_id", instructor.instructor_cohort_id));
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM instructor WHERE id = @id;";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}