using HotelClientMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HotelClientMVC.Controllers
{
    public class RoomController : Controller
    {
        string baseUrl = "https://localhost:7169/api/Room/";
        [HttpGet]
        public async Task<IActionResult> Rooms()
        {
            GenericResponseModel<List<Room>> rooms = new GenericResponseModel<List<Room>>();
            using(var client  = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage res = await client.GetAsync($"{baseUrl}get-range");/*.Result;*/
                if(res.IsSuccessStatusCode)
                {
                    var roomRes = res.Content.ReadAsStringAsync().Result;
                    rooms = JsonConvert.DeserializeObject<GenericResponseModel<List<Room>>>(roomRes);
                }
                return View("~/Views/Room/Rooms.cshtml",rooms);

            }
        }

        [HttpGet]
        public async Task<IActionResult> FilterRooms(decimal? minPrice, decimal? maxPrice, string? roomType) 
        {
            string apiUrl = $"{baseUrl}filter?";
            if (minPrice.HasValue)
            {
                apiUrl += $"minPrice={minPrice}&";
            }
            if(maxPrice.HasValue)
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


            GenericResponseModel<List<Room>> rooms = new GenericResponseModel<List<Room>>();
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage res = await client.GetAsync(apiUrl);
                if(res.IsSuccessStatusCode)
                {
                    var roomRes = res.Content.ReadAsStringAsync().Result;
                    rooms = JsonConvert.DeserializeObject<GenericResponseModel<List<Room>>>(roomRes);
                }
            }
            return View("~/Views/Room/Rooms.cshtml", rooms);
        }

        [HttpGet]
        public async Task<IActionResult> RoomDetails(string roomId)
        {
            GenericResponseModel<Room> room = null;//todo buna bele null vermek olarmi 
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage res = await client.GetAsync($"{baseUrl}id?id={roomId}");
                if(res.IsSuccessStatusCode)
                {
                    var roomDetail = res.Content.ReadAsStringAsync().Result;
                    room = JsonConvert.DeserializeObject<GenericResponseModel<Room>>(roomDetail);
                }
            }
            return View("~/Views/Room/RoomDetails.cshtml", room);
        }

        
    }
}
