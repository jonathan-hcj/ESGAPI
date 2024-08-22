using ESGAPI.Contexts;
using ESGAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity.Infrastructure;

namespace ESGAPI.Controllers
{
    /// <summary>
    /// Manages customer objects
    /// </summary>
    [ApiController]
    [Route("customer")]
     public class CustomerController(IConfiguration configuration, CustomerDbContext context) : ControllerBase
    {
        private readonly CustomerDbContext customerDbContext = context;
        private readonly IConfiguration configuration = configuration;

        /// <summary>
        /// Returns a customer object
        /// </summary>
        [HttpGet(Name = "{reference}")]
        public  ActionResult<Customer> Get(string reference)
        {
            var customer = customerDbContext.Customer.FirstOrDefault(x => x.CustomerRef != null && x.CustomerRef.Equals(reference));

            return customer != null ? customer : NotFound();
        }

        /// <summary>
        /// Create a customer object
        /// </summary>
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
