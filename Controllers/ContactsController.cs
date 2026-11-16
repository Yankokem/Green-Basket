using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Green_Basket.Data;
using Green_Basket.Models;

namespace Green_Basket.Controllers
{
    [Authorize]
    public class ContactsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Contacts
        [Authorize(Roles = "Admin,DRCoordinator,Staff")]
        public async Task<IActionResult> Index(int? assetId)
        {
            var query = _context.Contacts.Include(c => c.Asset).AsQueryable();
            if (assetId.HasValue)
            {
                query = query.Where(c => c.AssetId == assetId.Value);
                ViewBag.CurrentAsset = await _context.Assets.FindAsync(assetId.Value);
            }

            var contacts = await query.OrderBy(c => c.Asset!.Name).ThenBy(c => c.Name).ToListAsync();
            ViewData["AssetId"] = new SelectList(await _context.Assets.OrderBy(a => a.Name).ToListAsync(), "Id", "Name", assetId);
            return View(contacts);
        }

        // GET: Contacts/Details/5
        [Authorize(Roles = "Admin,DRCoordinator,Staff")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                .Include(c => c.Asset)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // GET: Contacts/Create
        [Authorize(Roles = "Admin,DRCoordinator")]
        public IActionResult Create(int? assetId)
        {
            ViewData["AssetId"] = new SelectList(_context.Assets.OrderBy(a => a.Name), "Id", "Name", assetId);
            var contact = new Contact();
            if (assetId.HasValue)
            {
                contact.AssetId = assetId.Value;
            }
            return View(contact);
        }

        // POST: Contacts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DRCoordinator")]
        public async Task<IActionResult> Create([Bind("AssetId,Name,Role,Phone,Email")] Contact contact)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contact);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Contact '{contact.Name}' was successfully added.";
                return RedirectToAction(nameof(Index), new { assetId = contact.AssetId });
            }
            ViewData["AssetId"] = new SelectList(_context.Assets.OrderBy(a => a.Name), "Id", "Name", contact.AssetId);
            return View(contact);
        }

        // GET: Contacts/Edit/5
        [Authorize(Roles = "Admin,DRCoordinator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }
            ViewData["AssetId"] = new SelectList(_context.Assets.OrderBy(a => a.Name), "Id", "Name", contact.AssetId);
            return View(contact);
        }

        // POST: Contacts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DRCoordinator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AssetId,Name,Role,Phone,Email")] Contact contact)
        {
            if (id != contact.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contact);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Contact '{contact.Name}' was updated.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactExists(contact.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { assetId = contact.AssetId });
            }
            ViewData["AssetId"] = new SelectList(_context.Assets.OrderBy(a => a.Name), "Id", "Name", contact.AssetId);
            return View(contact);
        }

        // GET: Contacts/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                .Include(c => c.Asset)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // POST: Contacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact != null)
            {
                _context.Contacts.Remove(contact);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Contact '{contact.Name}' was removed.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ContactExists(int id)
        {
            return _context.Contacts.Any(e => e.Id == id);
        }
    }
}
