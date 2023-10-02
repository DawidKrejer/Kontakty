using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Kontakty.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Kontakt> Kontakty { get; set; }
        public DbSet<User> Users { get; set; }
        //Połączenie z bazą danych do uzupełnienia nazwa urządzenia, nazwa bazy, login i hasło 
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=; Initial Catalog=; User Id=; password=; TrustServerCertificate= True");
        }
    }
}
