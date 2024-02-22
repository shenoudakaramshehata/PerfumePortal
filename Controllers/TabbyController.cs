using CRM.Data;
using CRM.Models;
using DevExpress.ClipboardSource.SpreadsheetML;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;

namespace CRM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TabbyController : ControllerBase

    {
        private PerfumeContext _context;
        public HttpClient httpClient { get; set; }
        private readonly IWebHostEnvironment _hostingEnvironment;

        public TabbyController(IWebHostEnvironment hostingEnvironment, PerfumeContext context)
        {
            _context = context;
            httpClient = new HttpClient();
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<WebhookEvent2Contoller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<WebhookEvent2Contoller>
        //[HttpPost]

        //public async Task<ActionResult> Post([FromBody] WebHookEventResponse model)
        //{

        //    var modelFileName = GenerateUniqueFileName("model");
        //    var modelJSON = JsonConvert.SerializeObject(model);
        //    await SaveDataToFileAsync(modelFileName, modelJSON);
        //    string token = "sk_test_a212d9c5-4c21-4a64-8c96-bfab29894c19";
        //    if (model.status == "AUTHORIZED" && model.captures.Count == 0)
        //    {
        //        string url = "https://api.tabby.ai/api/v1/payments/" + model.id;


        //        var client = new HttpClient();
        //        var request = new HttpRequestMessage(HttpMethod.Get, url);
        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //        //request.Headers.Add("Authorization", "Bearer sk_test_bf04ca3f-b6e5-451b-8013-8aefeaeffd60");
        //        //request.Headers.Add("Cookie", "_cfuvid=R6P0.2ESKgiMKGYmO3NQ4qEZQuDr.W5EPlICzx3a2iQ-1691234705097-0-604800000");
        //        var response = await client.SendAsync(request);
        //        var res = await response.Content.ReadAsStringAsync();
        //        if (res != null)
        //        {
        //            var ResStatus = JsonConvert.DeserializeObject<Root>(res);
        //            if (ResStatus.status == "AUTHORIZED")
        //            {
        //                CultureInfo cultureInfo = CultureInfo.InvariantCulture; // Use the appropriate culture

        //                bool checkResamount = double.TryParse(model.amount, NumberStyles.Any, cultureInfo, out double doubleValue);
        //                if (checkResamount)
        //                {
        //                    var sendPaymentRequest = new
        //                    {
        //                        amount = doubleValue
        //                    };

        //                    var sendPaymentRequestJSON = JsonConvert.SerializeObject(sendPaymentRequest);

        //                    string catureurl = "https://api.tabby.ai/api/v1/payments/" + model.id + "/captures";
        //                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //                    var httpContent = new StringContent(sendPaymentRequestJSON, Encoding.UTF8, "application/json");
        //                    var responseMessage = httpClient.PostAsync(catureurl, httpContent);
        //                    var rescatpture = await responseMessage.Result.Content.ReadAsStringAsync();
        //                    var requestFileName = GenerateUniqueFileName("payment_capture_request");
        //                    var responseFileName = GenerateUniqueFileName("payment_capture_response");

        //                    await SaveDataToFileAsync(requestFileName, sendPaymentRequestJSON);
        //                    await SaveDataToFileAsync(responseFileName, rescatpture);
        //                    return Ok(new { Status = 200, Message = "Succeded" });
        //                }
        //            }
        //        }

        //    }
        //    return Ok(new { Status = 400, Message = "not Succeded" });

        //}


        //[HttpPost]
        //[Route("retrieve")]
        //public async Task<ActionResult> retrieve([FromBody] WebHookEventResponse model)
        //{
        //    var modelFileName = GenerateUniqueFileName("model", model.id);
        //    var captureFileName = GenerateUniqueFileName("capture", model.id);

        //    var modelJSON = JsonConvert.SerializeObject(model);
        //    await SaveDataToFileAsync(modelFileName, modelJSON);
        //    //var modelFileName = GenerateUniqueFileName("model");
        //    //var modelJSON = JsonConvert.SerializeObject(model);
        //    //await SaveDataToFileAsync(modelFileName, modelJSON);
        //    string token = "sk_test_a212d9c5-4c21-4a64-8c96-bfab29894c19";
        //    if (model.status == "AUTHORIZED" && model.captures.Count == 0)
        //    {
        //        string url = "https://api.tabby.ai/api/v1/payments/" + model.id;


        //        var client = new HttpClient();
        //        var request = new HttpRequestMessage(HttpMethod.Get, url);
        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //        //request.Headers.Add("Authorization", "Bearer sk_test_bf04ca3f-b6e5-451b-8013-8aefeaeffd60");
        //        //request.Headers.Add("Cookie", "_cfuvid=R6P0.2ESKgiMKGYmO3NQ4qEZQuDr.W5EPlICzx3a2iQ-1691234705097-0-604800000");
        //        var response = await client.SendAsync(request);
        //        var res = await response.Content.ReadAsStringAsync();
        //        if (res != null)
        //        {
        //            var ResStatus = JsonConvert.DeserializeObject<Root>(res);
        //            if (ResStatus.status == "AUTHORIZED")
        //            {
        //                CultureInfo cultureInfo = CultureInfo.InvariantCulture; // Use the appropriate culture

        //                bool checkResamount = double.TryParse(model.amount, NumberStyles.Any, cultureInfo, out double doubleValue);
        //                if (checkResamount)
        //                {
        //                    var sendPaymentRequest = new
        //                    {
        //                        amount = doubleValue
        //                    };

        //                    var sendPaymentRequestJSON = JsonConvert.SerializeObject(sendPaymentRequest);

        //                    string catureurl = "https://api.tabby.ai/api/v1/payments/" + model.id + "/captures";
        //                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //                    var httpContent = new StringContent(sendPaymentRequestJSON, Encoding.UTF8, "application/json");
        //                    var responseMessage = httpClient.PostAsync(catureurl, httpContent);
        //                    var rescatpture = await responseMessage.Result.Content.ReadAsStringAsync();
        //                    //var requestFileName = GenerateUniqueFileName("payment_capture_request");
        //                    //var responseFileName = GenerateUniqueFileName("payment_capture_response");

        //                    //await SaveDataToFileAsync(requestFileName, sendPaymentRequestJSON);
        //                    //await SaveDataToFileAsync(responseFileName, rescatpture);
        //                    var requestFileName = GenerateUniqueFileName("payment_capture_request", model.id);
        //                    var responseFileName = GenerateUniqueFileName("payment_capture_response", model.id);

        //                    await SaveDataToFileAsync(requestFileName, sendPaymentRequestJSON);
        //                    await SaveDataToFileAsync(responseFileName, rescatpture);
        //                    return Ok(new { Status = 200, Message = "Succeded" });
        //                }
        //            }
        //        }

        //    }
        //    return Ok(new { Status = 400, Message = "not Succeded" });

        //}
        // PUT api/<WebhookEvent2Contoller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }
        [HttpPost]
        [Route("retrieve")]
        public async Task<ActionResult> retrieve([FromBody] WebHookEventResponse model)
        {
            var modelFileName = GenerateUniqueFileName("model", model.id);
            var captureFileName = GenerateUniqueFileName("capture", model.id);

            var modelJSON = JsonConvert.SerializeObject(model);
            await SaveDataToFileAsync(modelFileName, modelJSON);

            //string token = "sk_test_bf04ca3f-b6e5-451b-8013-8aefeaeffd60";
            string token = "sk_74eb86f5-f780-4b08-b73d-1de94a8270ff";

            var order = _context.Order.FirstOrDefault(e => e.TabbyPaymentId == model.id);
            var currency = _context.Country.Include(e => e.Currency).FirstOrDefault(i => i.CountryId == order.CountryId)?.Currency.CurrencyTlen;

            if (model.status == "authorized" /*&& order != null && model.captures != null*/)
            {
                string url = "https://api.tabby.ai/api/v1/payments/" + model.id;

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.SendAsync(request);
                var res = await response.Content.ReadAsStringAsync();

                if (res != null)
                {
                    var ResStatus = JsonConvert.DeserializeObject<Root>(res);

                    if (ResStatus.status == "AUTHORIZED")
                    {

                        //var orderCaptures = _context.OrdersCaptures.ToList();

                        //foreach (var capture in model.captures)
                        //{
                        //bool captureExistsInOrder = orderCaptures.Any(orderCapture => orderCapture.captureId == capture.id);

                        //if (!captureExistsInOrder)
                        //{
                        CultureInfo cultureInfo = CultureInfo.InvariantCulture;
                        var captureAmount = ResStatus.amount;
                        bool checkCaptureAmount = double.TryParse(captureAmount, NumberStyles.Any, cultureInfo, out double doubleValue);

                        if (checkCaptureAmount)
                        {
                            var sendPaymentRequest = new
                            {
                                amount = doubleValue
                            };

                            var sendPaymentRequestJSON = JsonConvert.SerializeObject(sendPaymentRequest);

                            string captureUrl = "https://api.tabby.ai/api/v1/payments/" + model.id + "/captures";
                            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                            var httpContent = new StringContent(sendPaymentRequestJSON, Encoding.UTF8, "application/json");
                            var responseMessage = await httpClient.PostAsync(captureUrl, httpContent);
                            var resCapture = await responseMessage.Content.ReadAsStringAsync();
                            var ResCaptureStatus = JsonConvert.DeserializeObject<Root>(resCapture);

                            //if (ResCaptureStatus.status == "AUTHORIZED" || ResCaptureStatus.status=="CLOSED")
                            //{
                            //    var ordercapture = new OrdersCaptures()
                            //    {
                            //        captureId = capture.id,
                            //        amount = doubleValue,
                            //        Createdate = capture.created_at,
                            //        OrderId = order.OrderId,
                            //        currency = currency
                            //    };

                            //    _context.OrdersCaptures.Add(ordercapture);
                            //    _context.SaveChanges();
                            //}

                            var requestFileName = GenerateUniqueFileName("payment_capture_request", model.id);
                            var responseFileName = GenerateUniqueFileName("payment_capture_response", model.id);

                            await SaveDataToFileAsync(requestFileName, sendPaymentRequestJSON);
                            await SaveDataToFileAsync(responseFileName, resCapture);
                        }
                        //}
                        //}

                        return Ok(new { Status = 200, Message = "Succeeded" });
                    }

                    if (ResStatus.status == "CLOSED")
                    {

                    }
                }
            }

            return Ok(new { Status = 200, Message = "Not Succeeded" });
        }


        //[HttpPost]
        //[Route("retrieve")]
        //public async Task<ActionResult> retrieve([FromBody] string modelJson)
        //{
        //    var model = JsonConvert.DeserializeObject<WebHookEventResponse>(modelJson);

        //    var modelFileName = GenerateUniqueFileName("model", model.id);
        //    var captureFileName = GenerateUniqueFileName("capture", model.id);

        //    var modelJSON = JsonConvert.SerializeObject(model);
        //    await SaveDataToFileAsync(modelFileName, modelJSON);
        //    //var modelFileName = GenerateUniqueFileName("model");
        //    //var modelJSON = JsonConvert.SerializeObject(model);
        //    //await SaveDataToFileAsync(modelFileName, modelJSON);
        //    string token = "sk_test_a212d9c5-4c21-4a64-8c96-bfab29894c19";
        //    if (model.status == "AUTHORIZED" && model.captures.Count == 0)
        //    {
        //        string url = "https://api.tabby.ai/api/v1/payments/" + model.id;


        //        var client = new HttpClient();
        //        var request = new HttpRequestMessage(HttpMethod.Get, url);
        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //        //request.Headers.Add("Authorization", "Bearer sk_test_bf04ca3f-b6e5-451b-8013-8aefeaeffd60");
        //        //request.Headers.Add("Cookie", "_cfuvid=R6P0.2ESKgiMKGYmO3NQ4qEZQuDr.W5EPlICzx3a2iQ-1691234705097-0-604800000");
        //        var response = await client.SendAsync(request);
        //        var res = await response.Content.ReadAsStringAsync();
        //        if (res != null)
        //        {
        //            var ResStatus = JsonConvert.DeserializeObject<Root>(res);
        //            if (ResStatus.status == "AUTHORIZED")
        //            {
        //                CultureInfo cultureInfo = CultureInfo.InvariantCulture; // Use the appropriate culture

        //                bool checkResamount = double.TryParse(model.amount, NumberStyles.Any, cultureInfo, out double doubleValue);
        //                if (checkResamount)
        //                {
        //                    var sendPaymentRequest = new
        //                    {
        //                        amount = doubleValue
        //                    };

        //                    var sendPaymentRequestJSON = JsonConvert.SerializeObject(sendPaymentRequest);

        //                    string catureurl = "https://api.tabby.ai/api/v1/payments/" + model.id + "/captures";
        //                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //                    var httpContent = new StringContent(sendPaymentRequestJSON, Encoding.UTF8, "application/json");
        //                    var responseMessage = httpClient.PostAsync(catureurl, httpContent);
        //                    var rescatpture = await responseMessage.Result.Content.ReadAsStringAsync();
        //                    //var requestFileName = GenerateUniqueFileName("payment_capture_request");
        //                    //var responseFileName = GenerateUniqueFileName("payment_capture_response");

        //                    //await SaveDataToFileAsync(requestFileName, sendPaymentRequestJSON);
        //                    //await SaveDataToFileAsync(responseFileName, rescatpture);
        //                    var requestFileName = GenerateUniqueFileName("payment_capture_request", model.id);
        //                    var responseFileName = GenerateUniqueFileName("payment_capture_response", model.id);

        //                    await SaveDataToFileAsync(requestFileName, sendPaymentRequestJSON);
        //                    await SaveDataToFileAsync(responseFileName, rescatpture);
        //                    return Ok(new { Status = 200, Message = "Succeded" });
        //                }
        //            }
        //        }

        //    }
        //    return Ok(new { Status = 400, Message = "not Succeded" });

        //}
        // DELETE api/<WebhookEvent2Contoller>/5

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        private string GenerateUniqueFileName(string baseName, string paymentId)
        {
            return $"{baseName}_{paymentId}.txt";
        }

        //private string GenerateUniqueFileName(string baseName)
        //{
        //    string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
        //    string randomId = Guid.NewGuid().ToString("N").Substring(0, 8);
        //    string uniqueFileName = $"{baseName}_{timestamp}_{randomId}.txt";
        //    return uniqueFileName;
        //}

        private async Task SaveDataToFileAsync(string fileName, string content)
        {
            var webRootPath = _hostingEnvironment.WebRootPath;
            var filePath = Path.Combine(webRootPath, fileName);

            await using (StreamWriter writer = new StreamWriter(filePath, append: true))
            {
                await writer.WriteLineAsync(content);
            }
        }

    }
}
