using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Wyklad3.Models;
using Wyklad3.Services;
using Wyklad3.DTOs.Requests;
using Wyklad3.DTOs.Responses;

namespace Wyklad3.Controllers
{
    [Route("api/enrollment")]
    [ApiController]




    public class EnrollmentController : ControllerBase
    {
        [HttpPost]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
            {

            


            var st = new Student();
            st.FirstName = request.FirstName;
            /*...*/

            using(var con = new SqlConnection("Data Source = db - mssql; Initial Catalog = s17776; Integrated Security = True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();
                var tran = con.BeginTransaction();
                com.CommandText = "select IdStudies from studies where name = @name";
                com.Parameters.AddWithValue("name", request.Studies);
                var dr = com.ExecuteReader();
                if (!dr.Read())
                {
                    tran.Rollback();
                    return BadRequest("Studia nie istnieja");
                }
                int idstudies = (int)dr["IdStudies"];

                com.CommandText = "Insert into Student(indexNumber, FirstName) values (@Index, @Fname)";
                com.Parameters.AddWithValue("index", request.IndexNumber);
                com.Parameters.AddWithValue("Fname", request.FirstName);

                com.ExecuteNonQuery();
                tran.Commit();

            }

            var response = new EnrollStudentResponse();
            response.LastName = st.LastName;
            /*...*/


            return Ok(response);

            }

    /*
        [HttpPost]
        string conString = "Data Source=db-mssql;Initial Catalog = s17776; Integrated Security = True";
        public async Task<IActionResult> EnrollStudnet()
        {
            using (var connect = new SqlConnection(conString))
            using (var command = new SqlCommand())
            {
                command.Connection = connect;
                connect.Open();
                var tran = connect.BeginTransaction();

                try
                {

                }
                catch (Sql)

            }    
            */
        }


    }
