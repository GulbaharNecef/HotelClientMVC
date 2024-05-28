namespace HotelClientMVC.Models
{
    public class RoomUpdateViewModel
    {
        public string Id { get; set; }
        public string RoomNumber { get; set; }
        public string RoomType { get; set; }
        public string Status { get; set; }
        public decimal Price { get; set; }
        public IFormFile ImageFile { get; set; }
    }
}
