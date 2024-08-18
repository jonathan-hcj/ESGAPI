using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ESGAPI.Controllers
{
    [ApiController]
    [Route("customer")]
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
                    "DECLARE @exists BIT = CASE WHEN EXISTS(SELECT CustomerRef FROM Customer WHERE CustomerRef = @customerref) THEN 1 ELSE 0 END " +
                    "" +
                    "IF @exists = 1 " +
                    "SELECT 2 " + 
                    "ELSE " +
                    "BEGIN" +
                    "   INSERT INTO Customer (CustomerRef, " +
                    "               CustomerName," +
                    "               AddressLine1," +
                    "               AddressLine2," +
                    "               Town," +
                    "               County," +
                    "               Country," +
                    "               Postcode)" +
                    "   VALUES      (@customerref, @customername, @addressLine1, @addressLine2, @town, @county, @country, @postcode) " +
                    "   SELECT @@ROWCOUNT " +
                    "END ";

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

                var result = Convert.ToInt32(command.ExecuteScalar());

                switch(result)
                {
                    case 0:
                        return StatusCode(StatusCodes.Status400BadRequest,
                            $"Customer record could not be inserted");

                    case 2:
                        return StatusCode(StatusCodes.Status400BadRequest,
                            $"Customer account already exists");

                    default:
                        return CreatedAtAction(nameof(Get),
                            new { id = customer.CustomerRef }, null);
                }
            }

            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Error creating new customer record: {e.Message}");
            }
        }


    }
}
