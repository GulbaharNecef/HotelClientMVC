using System.ComponentModel.DataAnnotations;

namespace HotelClientMVC.ViewModels
{
	public class LoginViewModel
	{
		public string usernameOrEmail { get; set; }
		public string password { get; set; }
	}
}
