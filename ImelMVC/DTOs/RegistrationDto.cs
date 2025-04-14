using System.ComponentModel.DataAnnotations;

namespace ImelMVC.DTOs
{
    public class RegistrationDto
    {
        [Required(ErrorMessage = "Ime je obavezno.")]
        [StringLength(50, ErrorMessage = "Ime ne može imati više od 50 karaktera.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Prezime je obavezno.")]
        [StringLength(50, ErrorMessage = "Prezime ne može imati više od 50 karaktera.")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Lozinka je obavezna.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Lozinka mora imati najmanje 6 karaktera.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Potvrda je obavezna.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Email je obavezan.")]
        [EmailAddress(ErrorMessage = "Neispravan format email adrese.")]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; } = "User";

        [Required]
        public string Status { get; set; } = "Active";

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int VersionNum { get; set; } = 1;

        public int isDeleted { get; set; } = 0;
    }
}
