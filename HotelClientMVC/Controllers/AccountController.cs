using HotelClientMVC.DTOs;
using HotelClientMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace HotelClientMVC.Controllers
{
	public class AccountController : Controller
	{
		string baseUrl = "https://localhost:7169/api/Auth/login";
		private readonly HttpClient _httpClient;

		public AccountController(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}
		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return View(model);
				}
				var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
				var response = await _httpClient.PostAsync(baseUrl, content);//baseUrl calisacaqmi baxacam
				if (response.IsSuccessStatusCode)
				{
					var jsonResult = await response.Content.ReadAsStringAsync();
					var result = JsonConvert.DeserializeObject<AuthResponse>(jsonResult);

					//store token in the session
					HttpContext.Session.SetString("AccessToken", result.Data.AccessToken);
					HttpContext.Session.SetString("RefreshToken", result.Data.RefreshToken);
					return RedirectToAction("Rooms", "Room");
				}
				 ModelState.AddModelError(string.Empty, "Invalid login attempt");
				return View(model);
			}
			catch (Exception ex)
			{
				ModelState.AddModelError(String.Empty, "Something went wrong :(");
				return View(model);
			}

		}

		[HttpPost]
		public IActionResult Logout()
		{
			HttpContext.Session.Remove("AccessToken");
			HttpContext.Session.Remove("RefreshToken");

			return RedirectToAction("Rooms", "Room");
		}

	}
}
