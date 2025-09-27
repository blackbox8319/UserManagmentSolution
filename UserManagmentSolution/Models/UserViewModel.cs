using System.ComponentModel.DataAnnotations;
namespace UserManagmentSolution.Models
{

    public class UserViewModel
    {
        public string? Id { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        public string? FullName { get; set; }

        public string? Role { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string? ConfirmPassword { get; set; }
    }
}
