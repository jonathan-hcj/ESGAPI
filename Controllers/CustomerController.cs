using ESGAPI.Contexts;
using ESGAPI.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Entity.Infrastructure;

namespace ESGAPI.Controllers
{
    [ApiController]
    [Route("customer")]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerDbContext customerDbContext = new();
        private readonly ILogger<CustomerController> _logger;
        private readonly SqlConnectionStringBuilder builder = new()
        {
            DataSource = "JONATHANS_LT\\SQLEXPRESS",
            UserID = "API1",
            Password = "Blat",
            InitialCatalog = "ESG",
            TrustServerCertificate = true
        };

        public CustomerController(ILogger<CustomerController> logger)
        {
            _logger = logger;

            customerDbContext.Database.EnsureCreated();
        }

        [HttpGet(Name = "{reference}")]
        public  ActionResult<Customer> Get(string reference)
        {
            var customer = customerDbContext.Customer.FirstOrDefault(x => x.CustomerRef != null && x.CustomerRef.Equals(reference));

            return customer != null ? customer : NotFound();
        }

        [HttpPost] 
        public ActionResult<Customer> Post(Customer customer)
        {
            try
            {
                /* ensure then no insert is attempted without a primary key */
                if (String.IsNullOrWhiteSpace(customer.CustomerRef))
                {
                    return StatusCode(StatusCodes.Status400BadRequest,
                        $"Customer has no reference specified");
                }

                /* fail if the primary key is already in use */
                var existingCustomer = customerDbContext.Customer.FirstOrDefault(x => x.CustomerRef != null && x.CustomerRef.Equals(customer.CustomerRef));
                if (existingCustomer != null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest,
                        $"Customer account already exists");
                }
                else
                {
                    customerDbContext.Customer.Add(customer);
                    customerDbContext.SaveChanges();

                    return CreatedAtAction(nameof(Get),
                        new { id = customer.CustomerRef }, null);
                }
            }

            /* these should be covered, but you miy still get connection or concurrency errors etc */
            catch (DbUpdateException e)
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                    $"Customer record could not be inserted");
            }

            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Error creating new customer record: {e.Message}");
            }
        }


    }
}
