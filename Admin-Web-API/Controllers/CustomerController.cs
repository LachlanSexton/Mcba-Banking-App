using Admin_Web_API.Models;
using Microsoft.AspNetCore.Mvc;


namespace Admin_Web_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {

        private readonly Repository<Customer> _customerRepository;
        private readonly Repository<Login> _loginRepository;

        public CustomerController(Repository<Customer> customerRepository, Repository<Login> loginRepository)
        {
            _customerRepository = customerRepository;
            _loginRepository = loginRepository;
        }
        
        // Get call to retrieve customer information by ID
        [HttpGet("{id}")]
        public IActionResult GetCustomer(int id)
        {
            var customer = _customerRepository.GetById(id);
            if (customer == null)
            {
                return NotFound();
            }
            // Return Http 200 with customer
            return Ok(customer);
        }

        // Http put to update customer information
        // Takes customer id as request header
        [HttpPut("{id}")]
        public IActionResult UpdateCustomer(int id, [FromBody] Customer updatedCustomer)
        {
            var existingCustomer = _customerRepository.GetById(id);

            // Return 400 not found if customer does not exist in db
            if (existingCustomer == null)
            {
                return NotFound();
            }

            existingCustomer.CustomerID = id;
            existingCustomer.Name = updatedCustomer.Name;
            existingCustomer.TFN = updatedCustomer.TFN;
            existingCustomer.Address = updatedCustomer.Address;
            existingCustomer.City = updatedCustomer.City;
            existingCustomer.PostCode = updatedCustomer.PostCode;
            existingCustomer.State = updatedCustomer.State;
            existingCustomer.Mobile = updatedCustomer.Mobile;
            // Update the existing customer with the request body passed through the API

            _customerRepository.Update(existingCustomer);

            // Return Http 200 
            return Ok(existingCustomer);
        }

        // Lock customer by altering the locked attribute in the login table
        // Takes customer id as input
        [HttpPut("lock/{id}")]
        public IActionResult LockCustomer(int id)
        {
            // Search by customer Id in login repository for object
            var existingLogin = _loginRepository.GetById(id);

            if (existingLogin == null)
            {
                return NotFound();
            }

            // Lock the account and save in DB
            existingLogin.locked = true;

            _loginRepository.Update(existingLogin);

            return Ok(existingLogin);
        }

        // Unlock customer by altering the locked attribute in the login table
        [HttpPut("unlock/{id}")]
        public IActionResult UnlockCustomer(int id)
        {
            var existingLogin = _loginRepository.GetById(id);

            if (existingLogin == null)
            {
                return NotFound();
            }

            // Set the locked attribute to false, unlocking the account
            existingLogin.locked = false;

            _loginRepository.Update(existingLogin);

            return Ok(existingLogin);
        }


    }
}

