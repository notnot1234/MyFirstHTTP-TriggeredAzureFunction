using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
using System.Runtime.ConstrainedExecution;
using System.Data.Entity.Infrastructure;


namespace MyFirstHTTP_TriggeredAzureFunction
{

    public static class Function1
    {
        [FunctionName("sales-data")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<Sales>(requestBody);

            //Error msg
            string[] error_msg;
           error_msg = new string[] { };

            //Error handling for Branch ID
            try
            {
                var check_BranchId = input.BranchId;
                if (check_BranchId == 0)
                {
                    Console.WriteLine("asdad");
                    error_msg = error_msg.Append($"BranchId is required").ToArray();
                }

            }
            catch (Exception)
            {
                error_msg = error_msg.Append($"Error in BranchId please check if the format is correct").ToArray();
            }

            //Error handling for transaction ID

            try
            {
                var check_TransactionId = input.TransactionId;
                if (check_TransactionId is null || String.IsNullOrEmpty(check_TransactionId))
                {
                    error_msg = error_msg.Append($"TransactionId is required").ToArray();
                }

            }
            catch (Exception)
            {
                var check_TransactionId = input.TransactionId;
                if (check_TransactionId is null)
                {
                    error_msg = error_msg.Append($"Error in TransactionId please check if the format is correct").ToArray();
                }
            }

            //Error handling for Transaction Date
            try
            {
                var check_TransactionDate = input.TransactionDate;
                if (check_TransactionDate == default(DateTime))
                {
                    error_msg = error_msg.Append($"TransactionDate is required").ToArray();
                }

            }
            catch (Exception)
            {
                error_msg = error_msg.Append($"Error in TransactionDate please check if the format is correct").ToArray();
            }


            //Error handling for Amount
            try
            {
                var check_Amount = input.Amount;
                if (check_Amount == 0)
                {
                    error_msg = error_msg.Append($"Amout is required").ToArray();
                }

            }
            catch (Exception)
            {
                error_msg = error_msg.Append($"Error in Amout please check if the format is correct").ToArray();

            }

            if (error_msg.Length > 1)
            {
                Console.WriteLine(string.Join("\r\n", error_msg));
                return new OkObjectResult(new { Message = string.Join(". ", error_msg) });
            }

            var salesdata = new Sales
                {
                    BranchId = input.BranchId,
                    TransactionId = input.TransactionId,
                    TransactionDate = input.TransactionDate,
                    Amount = input.Amount,
                    LoyaltyCardNumber = input.LoyaltyCardNumber
                };



                using (AppDBContext context = new AppDBContext())
                {


                    // Check for the duplicate transaction id


                    var result_transaction_id = context.SalesData.Any(x => x.TransactionId == input.TransactionId);
                    if (result_transaction_id == false)
                    {
                        // Add new data
                        log.LogInformation($"Creating transaction id {input.TransactionId}");


                        context.SalesData.Add(salesdata);
                        context.SaveChanges();
                        return new OkObjectResult(new { Message = $"Data succesfully saved transaction id {input.TransactionId}", Data = salesdata });

                    }

                    else
                    {

                        // Check if the details are the same

                        var data_transaction = context.SalesData.First(a => a.TransactionId == input.TransactionId);
                        if (data_transaction.TransactionId == input.TransactionId &&
                             data_transaction.TransactionDate == input.TransactionDate &&
                             data_transaction.Amount == input.Amount &&
                             data_transaction.LoyaltyCardNumber == input.LoyaltyCardNumber)
                        {

                            return new OkObjectResult(new { Message = $"Data already added transaction id {input.TransactionId}", Data = salesdata });

                        }
                        else
                        {

                            data_transaction.Amount = input.Amount;
                            data_transaction.TransactionDate = input.TransactionDate;
                            data_transaction.LoyaltyCardNumber = input.LoyaltyCardNumber;

                            context.Entry(salesdata).State = EntityState.Modified;

                            context.Entry(salesdata).State = EntityState.Modified;
                            try
                            {
                                context.SaveChanges();
                            }
                            catch (DbUpdateConcurrencyException ex)
                            {

                                // Update the values of the entity that failed to save from the store
                                ex.Entries.Single().Reload();
                            }
                            context.SaveChanges();

                            return new OkObjectResult(new { Message = $"Data updated transaction id {input.TransactionId}", Data = salesdata });
                        }


                    }

                }


        }

    }
}