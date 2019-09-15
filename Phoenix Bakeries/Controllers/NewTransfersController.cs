using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Phoenix_Bakeries.Models;

namespace Phoenix_Bakeries.Controllers
{
    public class NewTransfersController : Controller
    {
        private readonly HttpClient client = new HttpClient();
        private string AccountName = "";

        // GET: NewTransfers/Create
        public async Task<IActionResult> Create()
        {
            List<BankList> banks = new List<BankList>();

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"{Environment.GetEnvironmentVariable("secretKey")}");
            //get list of banks
            string responseData = await client.GetStringAsync("https://api.paystack.co/bank");

            try
            {
                GetBanks res = JsonConvert.DeserializeObject<GetBanks>(responseData);
                foreach (BankData item in res.data)
                {
                    banks.Add(new BankList { BankCode = item.code, BankName = item.name });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Occurred: {ex.Message}");
            }
           
            ViewData["Banks"] = new SelectList(banks, "BankCode", "Bank");

            client.Dispose();

            return View();
        }

        // POST: NewTransfers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Nuban,Description,AccountNumber,BankCode,Currency")] NewTransfer newTransfer)
        {
            if (ModelState.IsValid)
            {
                //validate account details
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"{Environment.GetEnvironmentVariable("secretKey")}");

                string request = string.Format($"https://api.paystack.co/bank/resolve?account_number={newTransfer.AccountNumber}&bank_code={newTransfer.BankCode}");

                try
                {
                    //get response
                    HttpResponseMessage resDataStream = await client.GetAsync(request);
                    //check if account details are valid
                    if (resDataStream.IsSuccessStatusCode)
                    {
                        string resData = await resDataStream.Content.ReadAsStringAsync();
                        AccountValidation res = JsonConvert.DeserializeObject<AccountValidation>(resData);
                        AccountName = res.data.account_name;
                    }
                    else
                    {
                        TempData["AccountError"] = "Wrong Account Details... Please Reconfirm Account Details";
                        return RedirectToPage("./Create");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Occurred: {ex.Message}");
                }

                //create recipient after validating account details
                try
                {
                    string content = $"{{\"type\":\"nuban\",\"name\":\"{AccountName}\",\"description\":\"{newTransfer.Description}\",\"account_number\":\"{Convert.ToString(newTransfer.AccountNumber)}\",\"bank_code\":\"{newTransfer.BankCode}\",\"currency\":\"{newTransfer.Currency}\"}}";
                    HttpResponseMessage recipientRes = await client.PostAsync("https://api.paystack.co/transferrecipient", new StringContent(content));

                    //check if request is successful
                    if (recipientRes.IsSuccessStatusCode)
                    {
                        //Transfer to Recipient
                    }
                    else
                    {
                        return RedirectToPage("./Create");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Occurred: {ex.Message}");
                }

            }
            return View(newTransfer);
        }
    }
}
