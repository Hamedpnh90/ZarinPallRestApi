using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ZarinPallRestApi.Models;
using ZarinpalRestApi.Helpers;
using ZarinpalRestApi.Models;
using ZarinpalRestApi.ZarinPack;

namespace ZarinPallRestApi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var product = ProductDatabase.Data;
            return View(product);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Index(int Id)
        {
            
           var Product= ProductDatabase.GetById(Id);


            // کدی که درگاه زرین پال به شما میدهد
            String MerchantID = "YourMerchendCode";
            long Amount = Product.Price;
            String Description = "شارژ حساب";

            var request = new ZarinpalModelV4.Payment.Request
            {
                
                MerchantId = MerchantID,
                Amount = Amount * 10, // V4 in Rials
                CallbackUrl = $"{Request.Scheme}://{Request.Host}/OnlinePayment/{Product.Id}",//ادرس برگشت از درگاه به متدی که باید وریفای رو انجام دهد
                Description = Description
                
                

            };

            var response = RestApiVer4.PaymentRequest(request);

            if (response.Data.Code == 100)
            {
                var gatewayLink = RestApiVer4.GenerateGatewayLink(response.Data.Authority);
                return Redirect(gatewayLink);
            }

            TempData["Message"] = response.Data.Code;
            return View("Index");
        }




        ////CallbackUrl
        [Route("OnlinePayment/{id}")]
        public async Task<IActionResult> OnlinePayment(int id, string authority, string status)
        {

            var Product = ProductDatabase.GetById(id);
            String MerchantID = "YourMerchendCode";
       

            if (status == "NOK")
            {

                ViewBag.Text = "Transaction unsuccessful.";
            }

            else if (status == "OK")
            {

                var request = new ZarinpalModelV4.Verify.Request
                {
                    MerchantId = MerchantID,
                    Authority = authority,
                    Amount = Product.Id * 10
                };

                var response = RestApiVer4.Verify(request);

                if (response.Data.Code == 100) // موفقیت امیز
                {
                    ViewBag.IsSuccess = true;
                    ViewBag.code = $"شماره تراکنش: {response.Data.RefId}";
                    
                    
                }
                else if (response.Data.Code == 101) // تکرار تراکنشی که موفقیت امیز بوده
                {
                    ViewBag.IsSuccess = true;
                    ViewBag.code = $"شماره تراکنش: {response.Data.RefId}";
                }
                else // خطا
                {

                    ViewBag.Error = $"Transaction unsuccessful. Status: {response.Data.Code}";
                }
            }

            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}