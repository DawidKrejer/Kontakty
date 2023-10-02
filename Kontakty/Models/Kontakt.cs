using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
namespace Kontakty.Models
{
    //Model tworzenia Kontaktu
    public class Kontakt
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Imie { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nazwisko { get; set; }

        [Unique(typeof(ApplicationDbContext))]
        [EmailAddress]
        [MaxLength(256)]
        [Required(ErrorMessage = "Adres email jest już w użyciu.")]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Hasło { get; set; }

        [Required]
        [EnumDataType(typeof(CategoryType))]
        public CategoryType Kategoria { get; set; }

        [Podkategoria(nameof(Kategoria))]
        public string Podkategoria { get; set; }

        [Required]
        [Phone]
        public string Numer { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DataUrodzenia { get; set; }
    }
    //Zapisanie w bazie mozliwych kategorii i wyświetlanie ich
    public enum CategoryType
    {
        [Display(Name = "Służbowy")]
        Służbowy,

        [Display(Name = "Prywatny")]
        Prywatny,

        [Display(Name = "Inny")]
        Inny
    }
    //Walidacja unikalności wartości pola Kategoria
    [AttributeUsage(AttributeTargets.Property)]
    public class UniqueAttribute : ValidationAttribute
    {
        private readonly Type _contextType;

        public UniqueAttribute(Type contextType)
        {
            _contextType = contextType;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var dbContext = (DbContext)validationContext.GetService(_contextType);

            if (dbContext == null)
            {
                throw new InvalidOperationException("Atrybut [Unique] wymaga dostępu do DbContext.");
            }

            var propertyName = validationContext.MemberName;
            var entityType = validationContext.ObjectType;
            var propertyInfo = entityType.GetProperty(propertyName);

            var currentValue = propertyInfo.GetValue(validationContext.ObjectInstance, null);

            var dbSet = dbContext.Set<Kontakt>();
            var exists = dbSet.AsEnumerable().Any(e => e != validationContext.ObjectInstance && e.GetType().GetProperty(propertyName).GetValue(e, null).Equals(currentValue));

            return exists ? new ValidationResult(ErrorMessage) : ValidationResult.Success;
        }
    }
    //Próba zrobienia podobnego działania co w przypadku Kategorii(nieskończona część z błędami złe założenia)
    [AttributeUsage(AttributeTargets.Property)]
    public class PodkategoriaAttribute : ValidationAttribute
    {
        public string ZaleznoscOdWlasciwosci { get; }

        public PodkategoriaAttribute(string zaleznoscOdWlasciwosci)
        {
            ZaleznoscOdWlasciwosci = zaleznoscOdWlasciwosci;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyName = ZaleznoscOdWlasciwosci;
            var entityType = validationContext.ObjectType;
            var propertyInfo = entityType.GetProperty(propertyName);
            var parentValue = propertyInfo.GetValue(validationContext.ObjectInstance, null);

            if (parentValue == null)
            {
                return ValidationResult.Success;
            }

            var kategoria = (CategoryType)parentValue;
            var podkategoria = (string)value;

            if (kategoria == CategoryType.Służbowy)
            {
                if (podkategoria != "szef" && podkategoria != "klient")
                {
                    return new ValidationResult("Nieprawidłowa wartość dla Podkategorii.");
                }
            }
            else if (kategoria == CategoryType.Prywatny)
            {
                var validPodkategorie = new[] { "rodzina", "znajomi" };
                if (!validPodkategorie.Contains(podkategoria))
                {
                    return new ValidationResult("Nieprawidłowa wartość dla Podkategorii.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
