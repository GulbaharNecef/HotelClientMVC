using HotelClientMVC.DTOs;
using HotelClientMVC.Models;
using HotelClientMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Newtonsoft.Json;

namespace HotelClientMVC.Controllers
{
	public class RoomController : Controller
	{
		string baseUrl = "https://localhost:7169/api/Room/";
		public readonly HttpClient _httpClient;
		public readonly TokenService _tokenService;
		public RoomController(HttpClient httpClient, TokenService tokenService)
		{
			_httpClient = httpClient;
			_tokenService = tokenService;
		}
		[HttpGet]
		public async Task<IActionResult> Rooms()
		{//todo butun otaqlari sehifelerde goster pagination u frontda tetbiq etmek olurmuu bu o zaman yoxsa yene api daki get range ye request gondermeliyem fikirlesecem
			try
			{
				var token = HttpContext.Session.GetString("AccessToken");
				if (!string.IsNullOrEmpty(token))
				{
					var isAdmin = _tokenService.IsUserAdmin(token);
					ViewBag.IsAdmin = isAdmin;
				}
				// _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);


				_httpClient.BaseAddress = new Uri(baseUrl);
				_httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
				HttpResponseMessage res = await _httpClient.GetAsync($"{baseUrl}get-range");/*.Result;*/
				if (res.IsSuccessStatusCode)
				{
					var roomRes = res.Content.ReadAsAsync<GenericResponseModel<List<RoomAPIResponseModel>>>().Result;
					var rooms = roomRes.Data.Select(roomApiResponse => new Room
					{
						Id = roomApiResponse.Id,
						Price = roomApiResponse.Price,
						RoomNumber = roomApiResponse.RoomNumber,
						RoomType = roomApiResponse.RoomType,
						Status = roomApiResponse.Status,
						ImageBase64 = roomApiResponse.ImageData != null ? Convert.ToBase64String(roomApiResponse.ImageData) : null
					}).ToList();

					var reponseModel = new GenericResponseModel<List<Room>>()
					{
						Data = rooms,
						Message = roomRes.Message,
						StatusCode = roomRes.StatusCode
					};

					return View("~/Views/Room/Rooms.cshtml", reponseModel);
				}
				return View("~/Views/Room/Rooms.cshtml");
			}
			catch (Exception ex)
			{
				return View();
			}
		}

		[HttpGet]
		public async Task<IActionResult> FilterRooms(decimal? minPrice, decimal? maxPrice, string? roomType)
		{
			try
			{
				string apiUrl = $"{baseUrl}filter?";
				if (minPrice.HasValue)
				{
					apiUrl += $"minPrice={minPrice}&";
				}
				if (maxPrice.HasValue)
				{
					apiUrl += $"maxPrice={maxPrice}&";
				}
				if (!string.IsNullOrEmpty(roomType))
				{
					apiUrl += $"roomType={roomType}&";//todo burda niye & isaresi oldu baxacam
				}

				if (apiUrl.EndsWith("&"))
				{
					apiUrl = apiUrl.Remove(apiUrl.Length - 1);
				}


				var token = HttpContext.Session.GetString("AccessToken");

				if (!string.IsNullOrEmpty(token))
				{
					var isAdmin = _tokenService.IsUserAdmin(token);
					ViewBag.IsAdmin = isAdmin;
				}

				_httpClient.BaseAddress = new Uri(baseUrl);
				_httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
				HttpResponseMessage res = await _httpClient.GetAsync(apiUrl);
				if (res.IsSuccessStatusCode)
				{
					var roomRes = res.Content.ReadAsAsync<GenericResponseModel<List<RoomAPIResponseModel>>>().Result;
					var rooms = roomRes.Data?.Select(roomApiResponse => new Room
					{
						Id = roomApiResponse.Id,
						Price = roomApiResponse.Price,
						RoomNumber = roomApiResponse.RoomNumber,
						RoomType = roomApiResponse.RoomType,
						Status = roomApiResponse.Status,
						ImageBase64 = roomApiResponse.ImageData != null ? Convert.ToBase64String(roomApiResponse.ImageData) : null
					}).ToList();

					var reponseModel = new GenericResponseModel<List<Room>>()
					{
						Data = rooms,
						Message = roomRes.Message,
						StatusCode = roomRes.StatusCode
					};
					return View("~/Views/Room/Rooms.cshtml", reponseModel);
				}
				return View("~/Views/Room/Rooms.cshtml");
			}
			catch (Exception ex)
			{
				return View("~/Views/Room/Rooms.cshtml");
			}
		}

		[HttpGet]
		public async Task<IActionResult> RoomDetails(string roomId)//check availability
		{
			try
			{
				_httpClient.BaseAddress = new Uri(baseUrl);
				_httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
				HttpResponseMessage res = await _httpClient.GetAsync($"{baseUrl}id?id={roomId}");
				if (res.IsSuccessStatusCode)
				{
					var roomRes = res.Content.ReadAsAsync<GenericResponseModel<RoomAPIResponseModel>>().Result;
					var roomApiResponse = roomRes.Data;
					if (roomApiResponse != null)
					{
						var room = new Room()
						{
							Id = roomApiResponse.Id,
							Price = roomApiResponse.Price,
							RoomNumber = roomApiResponse.RoomNumber,
							RoomType = roomApiResponse.RoomType,
							Status = roomApiResponse.Status,
							ImageBase64 = roomApiResponse.ImageData != null ? Convert.ToBase64String(roomApiResponse.ImageData) : null
						};
						var reponseModel = new GenericResponseModel<Room>()
						{
							Data = room,
							Message = roomRes.Message,
							StatusCode = roomRes.StatusCode
						};
						return View("~/Views/Room/RoomDetails.cshtml", reponseModel);//room
					}
				}
				ModelState.AddModelError(string.Empty, "Unable to retrieve room details.");
				return View("~/Views/Room/Rooms.cshtml");

			}
			catch (Exception ex)
			{
				return View("~/Views/Room/Rooms.cshtml");
			}

		}

		[HttpGet]
		public async Task<IActionResult> UpdateRoom(string roomId)
		{
			_httpClient.BaseAddress = new Uri(baseUrl);
			_httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
			HttpResponseMessage res = await _httpClient.GetAsync($"id?id={roomId}");
			if (res.IsSuccessStatusCode)
			{
				var room = res.Content.ReadAsAsync<GenericResponseModel<RoomUpdateViewModel>>().Result;
				return View(room.Data);
			}
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> UpdateRoom(RoomUpdateViewModel model)
		{
			if (ModelState.IsValid)
			{
				//_httpClient.BaseAddress = new Uri(baseUrl);
				//_httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

				using (var content = new MultipartFormDataContent())
				{
					content.Add(new StringContent(model.Id), nameof(model.Id));
					content.Add(new StringContent(model.Status), nameof(model.Status));
					content.Add(new StringContent(model.RoomNumber), nameof(model.RoomNumber));
					content.Add(new StringContent(model.RoomType), nameof(model.RoomType));
					content.Add(new StringContent(model.Price.ToString()), nameof(model.Price));

					if (model.ImageFile != null && model.ImageFile.Length > 0)
					{
						var stream = model.ImageFile.OpenReadStream();
						var fileContent = new StreamContent(stream);
						fileContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
						{
							Name = "image",
							FileName = model.ImageFile.FileName
						};
						content.Add(fileContent);
					}

					var accessToken = HttpContext.Session.GetString("AccessToken");
					_httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

					var response = await _httpClient.PutAsync($"{baseUrl}{model.Id}", content);
					if (response.IsSuccessStatusCode)
					{
						return RedirectToAction("FilterRooms");
					}
					ModelState.AddModelError(string.Empty, "An error occurred while updating the room.");
				}
			}
			return View(model);
		}



		//todo bunu da duzelt admin ucun 
		/*[HttpPost]
        public async Task<IActionResult> CreateRoom(RoomUpdateViewModel model)
        {
            if (ModelState.IsValid)
            {
                //_httpClient.BaseAddress = new Uri(baseUrl);
                //_httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StringContent(model.Id), nameof(model.Id));
                    content.Add(new StringContent(model.Status), nameof(model.Status));
                    content.Add(new StringContent(model.RoomNumber), nameof(model.RoomNumber));
                    content.Add(new StringContent(model.RoomType), nameof(model.RoomType));
                    content.Add(new StringContent(model.Price.ToString()), nameof(model.Price));

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        var stream = model.ImageFile.OpenReadStream();
                        var fileContent = new StreamContent(stream);
                        fileContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                        {
                            Name = "image",
                            FileName = model.ImageFile.FileName
                        };
                        content.Add(fileContent);
                    }
                    var token = HttpContext.Session.GetString("AccessToken");
                    var accessToken = HttpContext.Session.GetString("AccessToken");
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                    var response = await _httpClient.PutAsync(baseUrl, content);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Rooms");
                    }
                    ModelState.AddModelError(string.Empty, "An error occurred while updating the room.");
                }
            }
            return View(model);
        }*/

	}
}
