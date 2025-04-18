using System.ComponentModel.DataAnnotations;

namespace ImelMVC.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string UserSpecificId { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "Ime je obavezno.")]
        [StringLength(50, ErrorMessage = "Ime ne može imati više od 50 karaktera.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Prezime je obavezno.")]
        [StringLength(50, ErrorMessage = "Prezime ne može imati više od 50 karaktera.")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Lozinka je obavezna.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Lozinka mora imati najmanje 6 karaktera.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email je obavezan.")]
        [EmailAddress(ErrorMessage = "Neispravan format email adrese.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Uloga je obavezna.")]
        public string Role { get; set; } = "User";

        [Required(ErrorMessage = "Status je obavezan.")]
        public string Status { get; set; } = "Active";

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int VersionNum { get; set; } = 1;

        public string ChangedBy { get; set; } = string.Empty;

        [DataType(DataType.DateTime)]
        public DateTime ChangedAt { get; set; } = DateTime.Now;

        public int isDeleted { get; set; } = 0;
    }
}
