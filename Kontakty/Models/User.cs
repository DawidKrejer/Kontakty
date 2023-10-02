﻿using System.ComponentModel.DataAnnotations;

namespace Kontakty.Models
{
    public class User
    {
        //Model uzytkownika do logowania w aplikacji
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
