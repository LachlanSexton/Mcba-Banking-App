using Admin_Web_App.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Admin_Web_App.Controllers
{
    [Route("Mcba/Admin")]
    public class AdminController : Controller
    {
        [HttpGet]
        public IActionResult Admin() => View();

        //Api call to lock an account
        private async Task<IActionResult> LockAccountPost(int lockCustomerId)
        {
            string apiUrl = $"http://localhost:5130/Customer/lock/{lockCustomerId}";
            using (HttpClient httpClient = new HttpClient())
            {
                StringContent content = new StringContent("", Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PutAsync(apiUrl, content);
                response.EnsureSuccessStatusCode();
            }

            return RedirectToAction("Admin");
        }

        //Api Call to unlock an account
        private async Task<IActionResult> UnlockAccountPost(int unlockCustomerId)
        {
            string apiUrl = $"http://localhost:5130/Customer/unlock/{unlockCustomerId}";
            using (HttpClient httpClient = new HttpClient())
            {
                StringContent content = new StringContent("", Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PutAsync(apiUrl, content);
                response.EnsureSuccessStatusCode();
            }

            return RedirectToAction("Admin");
        }

        //Call lock or unlock methods based on action received
        [HttpPost]
        public async Task<IActionResult> LockUnlockAccount(int? customerId, string action)
        {
            if (customerId == null)
            {
                ModelState.AddModelError("customerId", "Customer not found.");
                return View();
            }

            if (action == "lock")
            {
                // Lock account
                await LockAccountPost(customerId.Value);            }
            else if (action == "unlock")
            {
                // Unlock account logic
                await UnlockAccountPost(customerId.Value);
            }

            // Redirect to appropriate page after performing lock/unlock
            return RedirectToAction("Admin");
        }

        //Render Customer profile page
        [HttpGet]
        [Route("CustomerProfile")]
        public ActionResult CustomerProfile()
        {
            return View();
        }

        //Search for a customer using Id and return their details to display them
        [HttpPost]
        [Route("FindCustomer")]
        public async Task<IActionResult> FindCustomer(int id)
        {
            string apiUrl = $"http://localhost:5130/Customer/{id}";

            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
                
                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("id", "Customer not found.");
                    return View("CustomerProfile");
                }
                string jsonResponse = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON to a Customer object
                Customer foundCustomer = JsonConvert.DeserializeObject<Customer>(jsonResponse);

                // Pass the foundCustomer to the view
                return View("CustomerProfile", foundCustomer);
            }
        }

        //Update customer details
        [HttpPost]
        [Route("UpdateCustomer")]
        public async Task<IActionResult> UpdateCustomer(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return View("CustomerProfile", customer);
            }

            string apiUrl = $"http://localhost:5130/Customer/{customer.CustomerID}";
            using (HttpClient httpClient = new HttpClient())
            {
                string jsonContent = JsonConvert.SerializeObject(customer);
                StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PutAsync(apiUrl, content);
                response.EnsureSuccessStatusCode();
            }

            return RedirectToAction("Admin");

        }
    }
}
