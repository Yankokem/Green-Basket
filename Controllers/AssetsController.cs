using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Green_Basket.Data;
using Green_Basket.Models;

namespace Green_Basket.Controllers
{
    [Authorize]
    public class AssetsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssetsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Assets
        [Authorize(Roles = "Admin,DRCoordinator,Staff")]
        public async Task<IActionResult> Index()
        {
            var assets = await _context.Assets
                .Include(a => a.RecoveryProcedures)
                .Include(a => a.Contacts)
                .Include(a => a.RecoveryTests)
                .Include(a => a.RecoveryTasks)
                .OrderBy(a => a.Priority == "Critical" ? 1 : a.Priority == "High" ? 2 : a.Priority == "Medium" ? 3 : 4)
                .ThenBy(a => a.Name)
                .ToListAsync();

            return View(assets);
        }

        // GET: Assets/Details/5
        [Authorize(Roles = "Admin,DRCoordinator,Staff")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asset = await _context.Assets
                .Include(a => a.RecoveryProcedures)
                .Include(a => a.Contacts)
                .Include(a => a.RecoveryTests)
                .Include(a => a.RecoveryTasks)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (asset == null)
            {
                return NotFound();
            }

            var allAssets = await _context.Assets.OrderBy(a => a.Name).ToListAsync();
            ViewData["AssetId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(allAssets, "Id", "Name", asset.Id);
            ViewBag.AssetTargetsJson = System.Text.Json.JsonSerializer.Serialize(
                allAssets.ToDictionary(a => a.Id.ToString(), a => new { name = a.Name, rto = a.RtoMinutes, rpo = a.RpoMinutes, owner = a.Owner })
            );

            return View(asset);
        }

        // GET: Assets/Create
        [Authorize(Roles = "Admin,DRCoordinator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Assets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DRCoordinator")]
        public async Task<IActionResult> Create([Bind("Name,Type,Owner,Priority,RtoMinutes,RpoMinutes,BackupDependency")] Asset asset)
        {
            if (ModelState.IsValid)
            {
                _context.Add(asset);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Asset '{asset.Name}' was successfully created.";
                return RedirectToAction(nameof(Index));
            }
            return View(asset);
        }

        // GET: Assets/Edit/5
        [Authorize(Roles = "Admin,DRCoordinator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asset = await _context.Assets.FindAsync(id);
            if (asset == null)
            {
                return NotFound();
            }
            return View(asset);
        }

        // POST: Assets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DRCoordinator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Type,Owner,Priority,RtoMinutes,RpoMinutes,BackupDependency")] Asset asset)
        {
            if (id != asset.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(asset);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Asset '{asset.Name}' was successfully updated.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssetExists(asset.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(asset);
        }

        // GET: Assets/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asset = await _context.Assets
                .Include(a => a.RecoveryProcedures)
                .Include(a => a.Contacts)
                .Include(a => a.RecoveryTests)
                .Include(a => a.RecoveryTasks)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (asset == null)
            {
                return NotFound();
            }

            var allAssets = await _context.Assets.OrderBy(a => a.Name).ToListAsync();
            ViewData["AssetId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(allAssets, "Id", "Name", asset.Id);
            ViewBag.AssetTargetsJson = System.Text.Json.JsonSerializer.Serialize(
                allAssets.ToDictionary(a => a.Id.ToString(), a => new { name = a.Name, rto = a.RtoMinutes, rpo = a.RpoMinutes, owner = a.Owner })
            );

            return View(asset);
        }

        // POST: Assets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var asset = await _context.Assets.FindAsync(id);
            if (asset != null)
            {
                _context.Assets.Remove(asset);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Asset '{asset.Name}' and its associated recovery records were deleted.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool AssetExists(int id)
        {
            return _context.Assets.Any(e => e.Id == id);
        }
    }
}
