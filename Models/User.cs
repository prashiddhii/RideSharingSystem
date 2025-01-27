using System.ComponentModel.DataAnnotations;

namespace RideSharingSystem.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [MaxLength(15)]
        public string PhoneNumber { get; set; }

        [Required]
        public string Role { get; set; } // 'Rider' or 'Driver'
    }
}
