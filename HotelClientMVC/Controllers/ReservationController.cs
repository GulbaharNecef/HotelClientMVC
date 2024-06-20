using HotelClientMVC.DTOs;
using HotelClientMVC.Models;
using HotelClientMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;

namespace HotelClientMVC.Controllers
{
    public class ReservationController : Controller
    {
        string baseUrl = "https://localhost:7169/api/Reservation";
		public readonly HttpClient _httpClient;
		public readonly TokenService _tokenService;
        public ReservationController(HttpClient httpClient, TokenService tokenService)
        {
            _httpClient = httpClient;
            _tokenService = tokenService;
        }

        [HttpGet]
        public async Task<IActionResult> ReserveRoom(string id)
        {
            var token = HttpContext.Session.GetString("AccessToken");//AccessToken
			if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "Please log in to reserve a room.";
                return RedirectToAction("Login", "Account");

			}
			// Pass the roomId to the view
			ViewBag.RoomId = id;
			return View();
        }
        [HttpPost]
        public async Task<IActionResult> ReserveRoom(string id, ReservationCreateDTO reservationCreateDTO)
        {
			var token = HttpContext.Session.GetString("AccessToken");

			if (string.IsNullOrEmpty(token))
			{
				TempData["ErrorMessage"] = "Please log in to reserve a room.";
				return RedirectToAction("Login", "Account");
			}

			reservationCreateDTO.RoomId = id; // Set the roomId in the DTO

			_httpClient.BaseAddress = new Uri(baseUrl);
			_httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
			_httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

			var content = new StringContent(JsonConvert.SerializeObject(reservationCreateDTO), Encoding.UTF8, "application/json");
			HttpResponseMessage res = await _httpClient.PostAsync(baseUrl, content);

            if (res.IsSuccessStatusCode)
            {
				var reservationDetails = await res.Content.ReadAsStringAsync();
				TempData["ReservationDetails"] = reservationDetails;
				return RedirectToAction("ReservationConfirmation");
			}
            //in case of failure
			var errorMessage = await res.Content.ReadAsStringAsync();
			ModelState.AddModelError(string.Empty, errorMessage);
			return View(reservationCreateDTO);
		}

		[HttpGet]
		public async Task<IActionResult> ReservationConfirmation()
        {
            var reservationDetails = TempData["ReservationDetails"] as string;
            if (reservationDetails == null)
            {
				TempData["ErrorMessage"] = "No reservation details available.";
				return RedirectToAction("Rooms", "Room");
			}
            var reservationResponse = JsonConvert.DeserializeObject<GenericResponseModel<ReservationDetailsDTO>>(reservationDetails);
            var reservation = reservationResponse?.Data;
            if (reservation == null)
            {
                TempData["ErrorMessage"] = "Failed to retrieve reservation details.";
                return RedirectToAction("Rooms", "Room");
            }
            return View(reservation);
        }
    }

    
}
