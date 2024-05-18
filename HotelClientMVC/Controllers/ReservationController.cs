using HotelClientMVC.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace HotelClientMVC.Controllers
{
    public class ReservationController : Controller
    {
        string baseUrl = "https://localhost:7169/api/Reservation";

        [HttpGet]
        public async Task<IActionResult> ReserveRoom()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ReserveRoom(string roomId, ReservationCreateDTO reservationCreateDTO)
        {
            TempData["ReservationDetails"] = reservationCreateDTO;
            return RedirectToAction("ReservationConfirmation"/*, roomId, reservationCreateDTO*/);
        }

        [HttpPost]
        public async Task<IActionResult> ReservationConfirmation(/*string roomId*/)
        {
            return View("~/Views/Reservation/TempViewHiHI");
        }
    }

    
}
