using Microsoft.AspNetCore.Mvc;
using MyMvcApp.Models;
using MyMvcApp.Data;
using System.Linq;

namespace MyMvcApp.Controllers
{
    public class BestillingController : Controller
    {
        private readonly AppDbContext _context;

        public BestillingController(AppDbContext context)
        {
            _context = context;
        }

        // INDEX: Varer på bestilling (not Levert)
        public IActionResult Index()
        {
            var varer = _context.Bestillinger
                .Where(b => b.Status == "Bestilling")
                .ToList();

            return View(varer);
        }

        // CREATE - GET
        public IActionResult Create()
        {
            return View();
        }

        // CREATE - POST
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

        // EDIT - GET
        public IActionResult Edit(int id)
        {
            var vare = _context.Bestillinger.FirstOrDefault(b => b.Id == id);
            if (vare == null) return NotFound();
            return View(vare);
        }

        // EDIT - POST
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
            vare.Status = updated.Status;

            vare.Status = "Bestilling";

            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        // DELETE
        public IActionResult Delete(int id)
        {
            var vare = _context.Bestillinger.FirstOrDefault(b => b.Id == id);
            if (vare == null) return NotFound();

            _context.Bestillinger.Remove(vare);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // LEVERT - GET
        public IActionResult Levert()
        {
            var varer = _context.Bestillinger
                .Where(b => b.Status == "Levert")
                .ToList();

            return View(varer);
        }


        // SET STATUS — Used for all status buttons
        [HttpPost]
        public IActionResult SetStatus(int id, string status)
        {
            var vare = _context.Bestillinger.FirstOrDefault(b => b.Id == id);
            if (vare == null) return NotFound();

            vare.Status = status;

            if (status == "Levert")
                vare.DatoLevert = DateTime.UtcNow;

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

        // SET PLASSERING
        [HttpPost]
        public IActionResult SetPlassering(int id, string plassering)
        {
            var vare = _context.Bestillinger.FirstOrDefault(b => b.Id == id);
            if (vare == null) return NotFound();

            vare.Plassering = plassering;
            _context.SaveChanges();

            return RedirectToAction("Levert");
        }

        // MONTERING - GET
        public IActionResult Montering()
        {
            var varer = _context.Bestillinger
                .Where(b => b.Status == "Montering")
                .ToList();

            return View(varer);
        }

        // SET DATO FOR MONTERING
        [HttpPost]
        public IActionResult SetMonteringsdato(int id, DateTime dato)
        {
            var vare = _context.Bestillinger.FirstOrDefault(b => b.Id == id);
            if (vare == null) return NotFound();

            vare.DatoMontering = DateTime.SpecifyKind(dato, DateTimeKind.Utc);
            _context.SaveChanges();

            return RedirectToAction("Montering");
        }

        // START OPPDRAG
        [HttpPost]
        public IActionResult StartOppdrag(int id)
        {
            var vare = _context.Bestillinger.FirstOrDefault(b => b.Id == id);
            if (vare == null) return NotFound();

            vare.OppdragStartet = true;
            _context.SaveChanges();

            return RedirectToAction("Montering");
        }

        // HENTES - GET
        public IActionResult Hentes()
        {
            var varer = _context.Bestillinger
                .Where(b => b.Status == "Hentes")
                .ToList();

            return View(varer);
        }

        // SET "Kunde informert"
        [HttpPost]
        public IActionResult SetInformert(int id, bool KundeInformert)
        {
            var vare = _context.Bestillinger.FirstOrDefault(b => b.Id == id);
            if (vare == null) return NotFound();

            vare.KundeInformert = KundeInformert;
            _context.SaveChanges();

            return RedirectToAction("Hentes");
        }

        // SET NOTE
        [HttpPost]
        public IActionResult SetNote(int id, string note)
        {
            var vare = _context.Bestillinger.FirstOrDefault(b => b.Id == id);
            if (vare == null) return NotFound();

            vare.Note = note;
            _context.SaveChanges();

            return RedirectToAction("Hentes");
        }

        // HENTET (Delete from DB)
        [HttpPost]
        public IActionResult Hentet(int id)
        {
            var vare = _context.Bestillinger.FirstOrDefault(b => b.Id == id);
            if (vare == null) return NotFound();

            _context.Bestillinger.Remove(vare);
            _context.SaveChanges();

            return RedirectToAction("Hentes");
        }

        public IActionResult Retur()
        {
            var varer = _context.Bestillinger
                .Where(b => b.Status == "Retur" && !b.Solgt && !b.KastetUt) // Hides marked items
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
        

    }
}
