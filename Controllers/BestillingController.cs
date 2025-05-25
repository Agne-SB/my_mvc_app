using Microsoft.AspNetCore.Mvc;
using MyMvcApp.Models;
using MyMvcApp.Data;
using System.Net.Http;
using System.Net.Http.Json;
using System.Linq;
using System.Threading.Tasks;

namespace MyMvcApp.Controllers
{
    public class BestillingController : Controller
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;

        public BestillingController(AppDbContext context)
        {
            _context = context;
            _httpClient = new HttpClient();
        }

        public IActionResult Index()
        {
            var varer = _context.Bestillinger
                .Where(b => b.Status == "Bestilling")
                .ToList();
            return View(varer);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Bestilling nyBestilling)
        {
            nyBestilling.SendesFraLager = DateTime.SpecifyKind(nyBestilling.SendesFraLager, DateTimeKind.Utc);
            nyBestilling.Status = "Bestilling";

            _context.Bestillinger.Add(nyBestilling);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var vare = _context.Bestillinger.FirstOrDefault(b => b.Id == id);
            if (vare == null) return NotFound();
            return View(vare);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Bestilling updated)
        {
            var vare = _context.Bestillinger.FirstOrDefault(b => b.Id == id);
            if (vare == null) return NotFound();

            vare.RefNo = updated.RefNo;
            vare.Selger = updated.Selger;
            vare.Varegruppe = updated.Varegruppe;
            vare.InfoOmVare = updated.InfoOmVare;
            vare.SendesFraLager = DateTime.SpecifyKind(updated.SendesFraLager, DateTimeKind.Utc);
            vare.Status = "Bestilling";

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> SetStatus(int id, string status)
        {
            var vare = _context.Bestillinger.FirstOrDefault(b => b.Id == id);
            if (vare == null) return NotFound();

            var oldStatus = vare.Status;
            vare.Status = status;

            if (status == "Levert")
                vare.DatoLevert = DateTime.UtcNow;

            if (status == "Montering")
            {
                vare.DatoMontering = DateTime.UtcNow;

                var dto = new
                {
                    RefNo = vare.RefNo,
                    Adresse = vare.Plassering ?? "Ukjent adresse",
                    MonteringDato = vare.DatoMontering,
                    Worker = ""
                };

                try
                {
                    var response = await _httpClient.PostAsJsonAsync(
                        "https://montering-service.onrender.com/api/montering", dto);

                    if (!response.IsSuccessStatusCode)
                        Console.WriteLine(" Failed to send to MonteringService: " + response.StatusCode);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error sending to MonteringService: " + ex.Message);
                }
            }

            if (oldStatus == "Montering" && status != "Montering")
            {
                try
                {
                    var response = await _httpClient.DeleteAsync(
                        $"https://montering-service.onrender.com/api/montering/{vare.RefNo}");

                    if (!response.IsSuccessStatusCode)
                        Console.WriteLine(" Failed to delete from MonteringService: " + response.StatusCode);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(" Error deleting from MonteringService: " + ex.Message);
                }
            }

            _context.SaveChanges();

            return status switch
            {
                "Levert" => RedirectToAction("Levert"),
                "Montering" => RedirectToAction("Montering"),
                "Hentes" => RedirectToAction("Hentes"),
                "Retur" => RedirectToAction("Retur"),
                _ => RedirectToAction("Index")
            };
        }

        [HttpPost]
        public IActionResult SetPlassering(int id, string plassering)
        {
            var vare = _context.Bestillinger.FirstOrDefault(b => b.Id == id);
            if (vare == null) return NotFound();

            vare.Plassering = plassering;
            _context.SaveChanges();
            return RedirectToAction("Levert");
        }

        public IActionResult Montering()
        {
            var varer = _context.Bestillinger
                .Where(b => b.Status == "Montering")
                .ToList();
            return View(varer);
        }

        public IActionResult MonteringsOppdrag()
        {
            var varer = _context.Bestillinger
                .Where(b => b.Status == "Montering")
                .ToList();
            return View(varer);
        }

        public IActionResult Levert()
        {
            var varer = _context.Bestillinger
                .Where(b => b.Status == "Levert")
                .ToList();

            return View(varer);
        }


        [HttpPost]
        public IActionResult SetMonteringsdato(int id, DateTime dato)
        {
            var vare = _context.Bestillinger.FirstOrDefault(b => b.Id == id);
            if (vare == null) return NotFound();

            vare.DatoMontering = DateTime.SpecifyKind(dato, DateTimeKind.Utc);
            _context.SaveChanges();

            return RedirectToAction("Montering");
        }

        [HttpPost]
        public IActionResult StartOppdrag(int id)
        {
            var vare = _context.Bestillinger.FirstOrDefault(b => b.Id == id);
            if (vare == null) return NotFound();

            vare.OppdragStartet = true;
            _context.SaveChanges();

            return RedirectToAction("Montering");
        }

        public IActionResult Hentes()
        {
            var varer = _context.Bestillinger
                .Where(b => b.Status == "Hentes")
                .ToList();
            return View(varer);
        }

        [HttpPost]
        public IActionResult SetInformert(int id, bool KundeInformert)
        {
            var vare = _context.Bestillinger.FirstOrDefault(b => b.Id == id);
            if (vare == null) return NotFound();

            vare.KundeInformert = KundeInformert;
            _context.SaveChanges();

            return RedirectToAction("Hentes");
        }

        [HttpPost]
        public IActionResult SetNote(int id, string note)
        {
            var vare = _context.Bestillinger.FirstOrDefault(b => b.Id == id);
            if (vare == null) return NotFound();

            vare.Note = note;
            _context.SaveChanges();

            return RedirectToAction("Hentes");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var vare = _context.Bestillinger.FirstOrDefault(b => b.Id == id);
            if (vare == null) return NotFound();

            if (vare.Status == "Montering")
            {
                try
                {
                    await _httpClient.DeleteAsync($"https://montering-service.onrender.com/api/montering/{vare.RefNo}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error deleting from MonteringService: " + ex.Message);
                }
            }

            _context.Bestillinger.Remove(vare);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Retur()
        {
            var varer = _context.Bestillinger
                .Where(b => b.Status == "Retur" && !b.Solgt && !b.KastetUt)
                .ToList();

            ViewData["Total"] = varer.Count;
            ViewData["Solgt"] = _context.Bestillinger.Count(v => v.Status == "Retur" && v.Solgt);
            ViewData["Kastet"] = _context.Bestillinger.Count(v => v.Status == "Retur" && v.KastetUt);

            return View(varer);
        }

        [HttpPost]
        public IActionResult SetGrunnAvRetur(int id, string grunn)
        {
            var vare = _context.Bestillinger.FirstOrDefault(v => v.Id == id);
            if (vare == null) return NotFound();

            vare.GrunnAvRetur = grunn;
            _context.SaveChanges();
            return RedirectToAction("Retur");
        }

        [HttpPost]
        public IActionResult ToggleOutlet(int id)
        {
            var vare = _context.Bestillinger.FirstOrDefault(v => v.Id == id);
            if (vare == null) return NotFound();

            vare.OutletLagtUt = !vare.OutletLagtUt;
            _context.SaveChanges();
            return RedirectToAction("Retur");
        }

        [HttpPost]
        public IActionResult MarkAs(string type, int id)
        {
            var vare = _context.Bestillinger.FirstOrDefault(v => v.Id == id);
            if (vare == null) return NotFound();

            if (type == "Solgt") vare.Solgt = true;
            if (type == "Kastet") vare.KastetUt = true;

            _context.SaveChanges();
            return RedirectToAction("Retur");
        }

        [HttpPost]
        public IActionResult SlettFraMontering(int id)
        {
            var vare = _context.Bestillinger.FirstOrDefault(b => b.Id == id);
            if (vare == null) return NotFound();

            _context.Bestillinger.Remove(vare);
            _context.SaveChanges();

            return RedirectToAction("Montering");
        }

    }
}
