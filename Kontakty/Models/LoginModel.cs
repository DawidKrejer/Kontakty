using System.ComponentModel.DataAnnotations;

namespace Kontakty.Models
{
    public class LoginModel
    {

        [Required(ErrorMessage = "Pole Email jest wymagane.")]
        [EmailAddress(ErrorMessage = "Podaj prawidłowy adres email.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Pole Hasło jest wymagane.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
