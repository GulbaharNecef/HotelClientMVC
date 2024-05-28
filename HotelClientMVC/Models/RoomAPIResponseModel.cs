namespace HotelClientMVC.Models
{
	public class RoomAPIResponseModel
	{
		public string Id { get; set; }
		public string RoomNumber { get; set; }
		public string RoomType { get; set; }
		public string Status { get; set; }
		public decimal Price { get; set; }
		public byte[] ImageData { get; set; }//byte[] 
	}
}
