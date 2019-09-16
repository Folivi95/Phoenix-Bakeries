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
        private List<BankList> banks = new List<BankList>();
        private string AccountName = "";
        private int Amount = 0;
        private string postedBankName;
        private string RecipientCode = "";
        private string TransferCode = "";

        // GET: NewTransfers/Index
        public async Task<IActionResult> Index()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {Environment.GetEnvironmentVariable("secretKey")}");
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
           
            ViewData["Banks"] = new SelectList(banks, "BankCode", "BankName");

            client.Dispose();

            return View();
        }

        // POST: NewTransfers/Index
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Bind("ID,Nuban,Description,AccountNumber,Amount,BankCode,Currency")] NewTransfer newTransfer)
        {
            if (ModelState.IsValid)
            {               
                //validate account details
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {Environment.GetEnvironmentVariable("secretKey")}");

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
                        return RedirectToAction("Index");
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
                        try
                        {
                            string recipientData = await recipientRes.Content.ReadAsStringAsync();
                            RecipientVM res = JsonConvert.DeserializeObject<RecipientVM>(recipientData);
                            RecipientCode = res.data.recipient_code;
                            //client.Dispose(); 

                            //Transfer to Recipient
                            Amount = Convert.ToInt32(newTransfer.Amount);
                            //client.DefaultRequestHeaders.Accept.Clear();
                            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            //client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {Environment.GetEnvironmentVariable("secretKey")}");
                            string transferContent = $"{{\"source\":\"balance\",\"reason\":\"{newTransfer.Description}\",\"amount\":{Amount * 100},\"recipient\":\"{RecipientCode}\"}}";
                            HttpResponseMessage transferRes = await client.PostAsync("https://api.paystack.co/transfer", new StringContent(transferContent));

                            if (transferRes.IsSuccessStatusCode)
                            {
                                string transCodeData = await transferRes.Content.ReadAsStringAsync();
                                NewTransferVM transCode = JsonConvert.DeserializeObject<NewTransferVM>(transCodeData);
                                TransferCode = transCode.data.transfer_code;
                                //provide placeholder values to View
                                TempData["otpRequired"] = "yes";
                                TempData["NameAcc"] = AccountName;
                                TempData["Amount"] = newTransfer.Amount;
                                TempData["Bank"] = res.data.details.bank_name;
                                TempData["NumberAcc"] = newTransfer.AccountNumber;
                                TempData["TransCode"] = TransferCode;
                                client.Dispose();
                                return View(newTransfer);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error Occurred: {ex.Message}");
                        }
                    }
                    else
                    {
                        TempData["AccountError"] = "Error Occurred While Creating Recipient";
                        client.Dispose();
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Occurred: {ex.Message}");
                }

            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IndexConfirmed([Bind("ID,Nuban,Description,AccountNumber,BankCode,Currency")] NewTransfer newTransfer, string otpValue, string transferCode)
        {
            try
            {
                //Finalize Transfer to Recipient
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {Environment.GetEnvironmentVariable("secretKey")}");
                string finalizeContent = $"{{\"transfer_code\":\"{transferCode}\",\"otp\":\"{otpValue}\"}}";
                HttpResponseMessage finalizeRes = await client.PostAsync("https://api.paystack.co/transfer/finalize_transfer", new StringContent(finalizeContent));

                if (finalizeRes.IsSuccessStatusCode)
                {
                    return View("Details");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Occurrred: {ex.Message}");
            }

            //display error message
            TempData["AccountError"] = "Transfer Failed... Please Try Again";
            return RedirectToAction(nameof(Index));
        }
    }
}
