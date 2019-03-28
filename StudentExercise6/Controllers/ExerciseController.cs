using Microsoft.AspNetCore.Mvc;
using StudentExercise6.Models;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace StudentExercise6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExerciseController : ControllerBase
    {
        public SqlConnection Connection
        {
            get
            {
                string connectionString = "Server=HNEAL-PC\\SQLEXPRESS;Database=StudentExercise3;Integrated Security=true";
                return new SqlConnection(connectionString);
            }
        }

        // GET: api/Exercise
        [HttpGet]
        public IEnumerable<Exercise> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT 
                                            e.Id,   
                                            e.ExerciseName, 
                                            e.ExerciseLanguage
                                        FROM Exercise e";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Exercise> exercises = new List<Exercise>();
                    while (reader.Read())
                    {
                        Exercise exercise = new Exercise
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            ExerciseName = reader.GetString(reader.GetOrdinal("exerciseName")),
                            ExerciseLanguage = reader.GetString(reader.GetOrdinal("exerciseLanguage"))
                        };

                        exercises.Add(exercise);
                    }

                    reader.Close();
                    return exercises;
                }
            }
        }

        // GET: api/Exercises/1
        [HttpGet("{id}", Name = "GetExercise")]
        public Exercise Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT 
                                            i.id, 
                                            i.exerciseName,
                                            i.exerciseLanguage
                                          FROM Exercise e
                                         WHERE i.id = @id;";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Exercise exercise = null;
                    if (reader.Read())
                    {
                        exercise = new Exercise
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            ExerciseName = reader.GetString(reader.GetOrdinal("exerciseName")),
                            ExerciseLanguage = reader.GetString(reader.GetOrdinal("exerciseLanguage"))
                        };
                    }

                    reader.Close();
                    return exercise;
                }
            }
        }

        // POST: api/Exercise
        [HttpPost]
        public ActionResult Post([FromBody] Exercise newExercise)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO exercise (ExerciseName, ExerciseLanguage)
                                             OUTPUT INSERTED.Id
                                             VALUES (@exerciseName, @exerciseLanguage)";
                    cmd.Parameters.Add(new SqlParameter("@ExerciseName", newExercise.ExerciseName));
                    cmd.Parameters.Add(new SqlParameter("@ExerciseLanguage", newExercise.ExerciseLanguage));

                    int newId = (int)cmd.ExecuteScalar();
                    newExercise.Id = newId;
                    return CreatedAtRoute("GetExercise", new { id = newId }, newExercise);
                }
            }
        }

        // PUT: api/Instructors/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Exercise exercise)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Exercise 
                                           SET ExerciseName = @exerciseName, 
                                               ExerciseLanguage = @exerciseLanguage
                                         WHERE id = @id;";

                    cmd.Parameters.Add(new SqlParameter("@exerciseName", exercise.ExerciseName));
                    cmd.Parameters.Add(new SqlParameter("@exerciseLanguage", exercise.ExerciseLanguage));

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
                    cmd.CommandText = "DELETE FROM exercise WHERE id = @id;";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}