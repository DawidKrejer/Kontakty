using Kontakty.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kontakty.Controllers
{
    [Route("api/kontakty")]
    [ApiController]
    public class KontaktyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public KontaktyController(ApplicationDbContext context)
        {
            _context = context;
        }
        //wyświetlanie podstawowych informacji o kontakcie
        [HttpGet]
        [Route("WyswietlKontakty")]
        public async Task<IEnumerable<ContactInfo>> WyswietlKontakty()
        {
            var contacts = await _context.Kontakty
                .Select(c => new ContactInfo { Imie = c.Imie, Nazwisko = c.Nazwisko, Email = c.Email })
                .ToListAsync();

            return contacts;
        }

        public class ContactInfo
        {
            public string Imie { get; set; }
            public string Nazwisko { get; set; }
            public string Email { get; set; }
        }
        //Wyświetlanie szczegółowych informacji
        [HttpGet]
        [Route("WyswietlSzczegoly")]
        public async Task<IEnumerable<Kontakt>> WyswietlSzczegoly()
        {
            return await _context.Kontakty.ToListAsync();
        }
        //Dodawanie kontaktu
        [Authorize]
        [HttpPost]
        [Route("AddKontakt")]
        public async Task<IActionResult> AddKontakt(Kontakt addKontakt)
        {

            try
            {
                _context.Kontakty.Add(addKontakt);
                await _context.SaveChangesAsync();
                return Ok(addKontakt);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        //Edytowanie kontaktu
        [Authorize]
        [HttpPatch]
        [Route("UpdateKontakt/{id}")]
        public async Task<IActionResult> UpdateKontakt(int id, Kontakt updatedKontakt)
        {
            var kontakt = await _context.Kontakty.FindAsync(id);

            if (kontakt == null)
            {
                return NotFound();
            }

            kontakt.Imie = updatedKontakt.Imie;
            kontakt.Nazwisko = updatedKontakt.Nazwisko;
            kontakt.Email = updatedKontakt.Email;
            kontakt.Hasło = updatedKontakt.Hasło;
            kontakt.Kategoria = updatedKontakt.Kategoria;
            kontakt.Podkategoria = updatedKontakt.Podkategoria;
            kontakt.Numer = updatedKontakt.Numer;
            kontakt.DataUrodzenia = updatedKontakt.DataUrodzenia;

            try
            {
                _context.Entry(kontakt).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(kontakt);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        //Usuwanie kontaktu
        [Authorize]
        [HttpDelete]
        [Route("UsunKontakt/{id}")]
        public bool UsunKontakt(int id)
        {
            bool a = false;
            var kontakt = _context.Kontakty.Find(id);
            if (kontakt != null)
            {
                a = true;
                _context.Entry(kontakt).State = EntityState.Deleted;
                _context.SaveChanges();
            }
            else
            {
                a = false;
            }
            return a;
        }
    }
}
