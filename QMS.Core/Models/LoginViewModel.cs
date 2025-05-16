using System.ComponentModel.DataAnnotations;

namespace QMS.Core.Models;
public class LoginViewModel
{
	[Required(ErrorMessage = "Username is required.")]
	[Display(Name = "Username")]
	public string? Username { get; set; }

	[Required(ErrorMessage = "Password is required.")]
	[DataType(DataType.Password)]
	public string? Password { get; set; }

	public string? WrongCredentials { get; set; }
    public bool Deleted { get; set; }
}
