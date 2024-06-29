using HotelClientMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HotelClientMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

		public IActionResult AboutUs()
		{
			return View();
		}
        public IActionResult Services()
        {
            return View();
        }
        public IActionResult ContactUs()
        {
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(/*ContactViewModel model*/)
        {
            if (ModelState.IsValid)
            {
                //  i will process the contact form submission  (e.g. send an email)
                //Bunui sonra davam ederem

                //return RedirectToAction("ThankYou");
                return View();
            }

            return View(/*model*/);
        }

        public IActionResult ThankYou()
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
