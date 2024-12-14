using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace VideStore.Shared.DTOs.Requests
{
    public class UpdateUserDto
    {
        [Required]
        public string FirstName { get; set; } = null!;
        [Required]
        public string LastName { get; set; } = null!;
        [EmailAddress]
        [Required]
        public string Email { get; set; } = null!;
        [PasswordPropertyText]
        [Required]
        public string PhoneNumber { get; set; } = null!;

    }
}
