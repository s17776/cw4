using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Wyklad3.DTOs.Requests;
using Wyklad3.DTOs.Responses;
using Wyklad3.Models;
using Wyklad3.Services;

namespace Wyklad3.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private IDbService _dbService;
        
        string conString = "Data Source=db-mssql;Initial Catalog = s17776; Integrated Security = True";

        [HttpPost]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            EnrollStudentResponse esr = new EnrollStudentResponse() { };

            using (var con = new SqlConnection(conString))
            using (var com = new SqlCommand())
            {
                con.Open();
                var tran = con.BeginTransaction();
                com.Connection = con;
                com.Transaction = tran;
                try
                {
                    com.CommandText = "SELECT IdStudy AS idStudies FROM Studies WHERE Name=@name";
                    com.Parameters.AddWithValue("name", request.Studies);
                    var dr = com.ExecuteReader();
                    if (!dr.Read())
                    {
                        dr.Close();
                        tran.Rollback();
                        return NotFound("nie ma takiego kierunku");
                    }

                    int idStudies = (int)dr["idStudies"];
                    dr.Close();

                    com.CommandText = "SELECT IndexNumber FROM Student WHERE IndexNumber= '" + request.IndexNumber + "'";

                    dr = com.ExecuteReader();
                    if (dr.Read())
                    {
                        dr.Close();
                        tran.Rollback();
                        return BadRequest("Nr indeksu nie jest unikatowy");
                    }
                    dr.Close();

                    int idEnrollment;

                    com.CommandText = "SELECT IdEnrollment FROM Enrollment WHERE IdEnrollment = (SELECT MAX(IdEnrollment) FROM Enrollment)";
                    dr = com.ExecuteReader();

                    if (!dr.Read())
                    {
                        idEnrollment = 1;
                        dr.Close();
                        com.CommandText = "INSERT INTO Enrollment(IdEnrollment, Semester, IdStudy, StartDate) VALUES (" + idEnrollment + ", 1, " + idStudies + ", GetDate())";
                        com.ExecuteNonQuery();
                    }
                    idEnrollment = (int)dr["IdEnrollment"];
                    dr.Close();

                    string sDateFormat = "dd.MM.yyyy";
                    DateTime BirthDate = DateTime.ParseExact(request.Birthdate.ToString(), sDateFormat, CultureInfo.InvariantCulture);

                    com.CommandText = $"Insert INTO Student VALUES (@IndexNumber, @FirstName, @LastName, @BirthDate, @IdEnrollment)";
                    com.Parameters.AddWithValue("IndexNumber", request.IndexNumber);
                    com.Parameters.AddWithValue("FirstName", request.FirstName);
                    com.Parameters.AddWithValue("LastName", request.LastName);
                    com.Parameters.AddWithValue("BirthDate", BirthDate);
                    com.Parameters.AddWithValue("IdEnrollment", idEnrollment);
                    com.ExecuteNonQuery();

                    esr.IdEnrollment = idEnrollment;
                    esr.IdStudy = idStudies;
                    esr.Semester = 1;
                    esr.StartDate = DateTime.Now;
                    tran.Commit();
                    tran.Dispose();
                    return StatusCode((int)HttpStatusCode.Created, esr);

                }
                catch (SqlException exc)
                {
                    tran.Rollback();
                    return BadRequest(exc.Message);
                }
            }
        }



    }
}