﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ESGAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ILogger<CustomerController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "{reference}")]
        public  ActionResult<Customer> Get(string reference)
        {
            if (reference == null)
            {
                return NotFound();
            }
            else
            {
                try
                {
                    var builder = new SqlConnectionStringBuilder
                    {
                        DataSource = "JONATHANS_LT\\SQLEXPRESS",
                        UserID = "API1",
                        Password = "Blat",
                        InitialCatalog = "ESG",
                        TrustServerCertificate = true
                    };

                    var sql = 
                        "SELECT TOP 1   CustomerRef, " +
                        "               CustomerName," +
                        "               AddressLine1," +
                        "               AddressLine2," +
                        "               Town," +
                        "               County," +
                        "               Country," +
                        "               Postcode " +
                        "FROM           Customer " +
                        "WHERE          CustomerRef = @customerref";

                    var connection = new SqlConnection(builder.ConnectionString);
                    connection.Open();

                    var command = new SqlCommand(sql, connection);
                    command.Parameters.Add("@customerref", SqlDbType.NVarChar);
                    command.Parameters["@customerref"].Value = reference;

                    var reader = command.ExecuteReader();
                    
                    var dataTable = new DataTable();
                    dataTable.Load(reader);

                    if (dataTable.Rows.Count == 0)
                    {
                        return NotFound();
                    }
                    else { 
                        var customer = new Customer
                        {
                            CustomerRef = Convert.ToString(dataTable.Rows[0]["CustomerRef"]),
                            CustomerName = Convert.ToString(dataTable.Rows[0]["CustomerName"]),
                            AddressLine1 = Convert.ToString(dataTable.Rows[0]["AddressLine1"]),
                            AddressLine2 = Convert.ToString(dataTable.Rows[0]["AddressLine2"]),
                            Town = Convert.ToString(dataTable.Rows[0]["Town"]),
                            County = Convert.ToString(dataTable.Rows[0]["County"]),
                            Country = Convert.ToString(dataTable.Rows[0]["Country"]),
                            Postcode = Convert.ToString(dataTable.Rows[0]["Postcode"]),
                        };
                        return customer;
                    }
                }
                catch (Exception Ex)
                {
                    return BadRequest();
                }
            }
        }
/*
  {
  "customerRef": "012255",
  "customerName": "Sarah Campbell Jones",
  "addressLine1": "15, Albany Street",
  "addressLine2": "Burton Road",
  "town": "Lincoln",
  "county": "Lincs",
  "country": "GB",
  "postcode": "LN13HY"
}
*/
    [HttpPost]
        public ActionResult<Customer> Post(Customer customer)
        {
            try
            {
                var builder = new SqlConnectionStringBuilder
                {
                    DataSource = "JONATHANS_LT\\SQLEXPRESS",
                    UserID = "API1",
                    Password = "Blat",
                    InitialCatalog = "ESG",
                    TrustServerCertificate = true
                };

                var sql =
                    "INSERT INTO Customer (CustomerRef, " +
                    "               CustomerName," +
                    "               AddressLine1," +
                    "               AddressLine2," +
                    "               Town," +
                    "               County," +
                    "               Country," +
                    "               Postcode)" +
                    "VALUES (@customerref, @customername, @addressLine1, @addressLine2, @town, @county, @county, @postcode) ";

                var connection = new SqlConnection(builder.ConnectionString);
                connection.Open();

                var command = new SqlCommand(sql, connection);
                command.Parameters.Add("@customerref", SqlDbType.NVarChar);
                command.Parameters["@customerref"].Value = customer.CustomerRef;

                command.Parameters.Add("@customername", SqlDbType.NVarChar);
                command.Parameters["@customername"].Value = customer.CustomerName;

                command.Parameters.Add("@addressLine1", SqlDbType.NVarChar);
                command.Parameters["@addressLine1"].Value = customer.AddressLine1;

                command.Parameters.Add("@addressLine2", SqlDbType.NVarChar);
                command.Parameters["@addressLine2"].Value = customer.AddressLine2;

                command.Parameters.Add("@town", SqlDbType.NVarChar);
                command.Parameters["@town"].Value = customer.Town;

                command.Parameters.Add("@county", SqlDbType.NVarChar);
                command.Parameters["@county"].Value = customer.County;

                command.Parameters.Add("@country", SqlDbType.NVarChar);
                command.Parameters["@country"].Value = customer.Country;

                command.Parameters.Add("@postcode", SqlDbType.NVarChar);
                command.Parameters["@postcode"].Value = customer.Postcode;

                var reader = command.ExecuteNonQuery();

                return CreatedAtAction(nameof(Get),
                    new { id = customer.CustomerRef }, null);
            }

            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error creating new customer record");
            }
        }
    }
}
