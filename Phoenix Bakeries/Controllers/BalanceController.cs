using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Phoenix_Bakeries.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Phoenix_Bakeries.Controllers
{
    public class BalanceController : Controller
    {
        private readonly HttpClient client = new HttpClient();

        public async Task<IActionResult> Index()
        {
            //get account balance and display it
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {Environment.GetEnvironmentVariable("secretKey")}");

            try
            {

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Occurred: {ex.Message}");
            }
            string response = await client.GetStringAsync("https://api.paystack.co/balance");
            BalanceAndTopUpViewModel row = JsonConvert.DeserializeObject<BalanceAndTopUpViewModel>(response);

            //check if account balance returned a value
            if (row.status)
            {
                foreach (Models.Balance item in row.data)
                {
                    ViewData["AccountBalance"] = string.Format("{0:n}", Convert.ToDecimal(item.balance) / 100);
                }
            }
            else
            {
                ViewData["AccountBalance"] = "Not Available";
            }

            client.Dispose();

            return View();
        }
    }
}