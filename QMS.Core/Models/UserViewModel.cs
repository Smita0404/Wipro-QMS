using System.ComponentModel.DataAnnotations.Schema;

namespace QMS.Core.Models;

public class UserViewModel
{
    public int UserId { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public int? RoleId { get; set; }

    public string? Role { get; set; }

    public string? AdId { get; set; }

    public string? Designation { get; set; }

    public int? DivisionId { get; set; }

    public string? MobileNo { get; set; }

    public string? User_Type { get; set; }

    public bool Deleted { get; set; }
}
