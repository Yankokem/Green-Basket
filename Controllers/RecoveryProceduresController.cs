using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Green_Basket.Data;
using Green_Basket.Models;

namespace Green_Basket.Controllers
{
    [Authorize]
    public class RecoveryProceduresController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RecoveryProceduresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: RecoveryProcedures
        [Authorize(Roles = "Admin,DRCoordinator,Staff")]
        public async Task<IActionResult> Index(int? assetId)
        {
            var query = _context.RecoveryProcedures.Include(r => r.Asset).AsQueryable();
            if (assetId.HasValue)
            {
                query = query.Where(r => r.AssetId == assetId.Value);
                ViewBag.CurrentAsset = await _context.Assets.FindAsync(assetId.Value);
            }

            var procedures = await query.OrderBy(p => p.Asset!.Name).ThenBy(p => p.Title).ToListAsync();
            return View(procedures);
        }

        // GET: RecoveryProcedures/Details/5
        [Authorize(Roles = "Admin,DRCoordinator,Staff")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var procedure = await _context.RecoveryProcedures
                .Include(r => r.Asset)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (procedure == null)
            {
                return NotFound();
            }

            return View(procedure);
        }

        // GET: RecoveryProcedures/Create
        [Authorize(Roles = "Admin,DRCoordinator")]
        public IActionResult Create(int? assetId)
        {
            ViewData["AssetId"] = new SelectList(_context.Assets.OrderBy(a => a.Name), "Id", "Name", assetId);
            var model = new RecoveryProcedure();
            if (assetId.HasValue)
            {
                model.AssetId = assetId.Value;
            }
            return View(model);
        }

        // POST: RecoveryProcedures/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DRCoordinator")]
        public async Task<IActionResult> Create([Bind("AssetId,Title,Steps")] RecoveryProcedure procedure)
        {
            if (ModelState.IsValid)
            {
                _context.Add(procedure);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Recovery procedure '{procedure.Title}' was added.";
                return RedirectToAction(nameof(Index), new { assetId = procedure.AssetId });
            }
            ViewData["AssetId"] = new SelectList(_context.Assets.OrderBy(a => a.Name), "Id", "Name", procedure.AssetId);
            return View(procedure);
        }

        // GET: RecoveryProcedures/Edit/5
        [Authorize(Roles = "Admin,DRCoordinator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var procedure = await _context.RecoveryProcedures.FindAsync(id);
            if (procedure == null)
            {
                return NotFound();
            }
            ViewData["AssetId"] = new SelectList(_context.Assets.OrderBy(a => a.Name), "Id", "Name", procedure.AssetId);
            return View(procedure);
        }

        // POST: RecoveryProcedures/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DRCoordinator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AssetId,Title,Steps")] RecoveryProcedure procedure)
        {
            if (id != procedure.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(procedure);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Procedure '{procedure.Title}' was updated.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProcedureExists(procedure.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { assetId = procedure.AssetId });
            }
            ViewData["AssetId"] = new SelectList(_context.Assets.OrderBy(a => a.Name), "Id", "Name", procedure.AssetId);
            return View(procedure);
        }

        // GET: RecoveryProcedures/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var procedure = await _context.RecoveryProcedures
                .Include(r => r.Asset)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (procedure == null)
            {
                return NotFound();
            }

            return View(procedure);
        }

        // POST: RecoveryProcedures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var procedure = await _context.RecoveryProcedures.FindAsync(id);
            if (procedure != null)
            {
                _context.RecoveryProcedures.Remove(procedure);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Procedure '{procedure.Title}' was removed.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProcedureExists(int id)
        {
            return _context.RecoveryProcedures.Any(e => e.Id == id);
        }
    }
}
