using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Green_Basket.Data;
using Green_Basket.Models;

namespace Green_Basket.Controllers
{
    [Authorize]
    public class RecoveryTasksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RecoveryTasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: RecoveryTasks
        [Authorize(Roles = "Admin,DRCoordinator,Staff")]
        public async Task<IActionResult> Index(int? assetId)
        {
            var query = _context.RecoveryTasks.Include(r => r.Asset).AsQueryable();
            if (assetId.HasValue)
            {
                query = query.Where(r => r.AssetId == assetId.Value);
                ViewBag.CurrentAsset = await _context.Assets.FindAsync(assetId.Value);
            }

            var tasks = await query
                .OrderBy(t => t.Status == "Pending" ? 1 : t.Status == "In Progress" ? 2 : 3)
                .ThenBy(t => t.DueDate)
                .ToListAsync();

            ViewData["AssetId"] = new SelectList(await _context.Assets.OrderBy(a => a.Name).ToListAsync(), "Id", "Name", assetId);
            return View(tasks);
        }

        // GET: RecoveryTasks/Details/5
        [Authorize(Roles = "Admin,DRCoordinator,Staff")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recoveryTask = await _context.RecoveryTasks
                .Include(r => r.Asset)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (recoveryTask == null)
            {
                return NotFound();
            }

            return View(recoveryTask);
        }

        // GET: RecoveryTasks/Create
        [Authorize(Roles = "Admin,DRCoordinator")]
        public IActionResult Create(int? assetId, string? suggestedTitle = null)
        {
            ViewData["AssetId"] = new SelectList(_context.Assets.OrderBy(a => a.Name), "Id", "Name", assetId);
            var task = new RecoveryTask { DueDate = DateTime.Today.AddDays(7), Status = "Pending" };
            if (assetId.HasValue)
            {
                task.AssetId = assetId.Value;
            }
            if (!string.IsNullOrWhiteSpace(suggestedTitle))
            {
                task.Title = suggestedTitle;
            }
            return View(task);
        }

        // POST: RecoveryTasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DRCoordinator")]
        public async Task<IActionResult> Create([Bind("AssetId,Title,AssignedTo,Status,DueDate")] RecoveryTask recoveryTask)
        {
            if (ModelState.IsValid)
            {
                _context.Add(recoveryTask);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Task '{recoveryTask.Title}' successfully created.";
                return RedirectToAction(nameof(Index), new { assetId = recoveryTask.AssetId });
            }
            ViewData["AssetId"] = new SelectList(_context.Assets.OrderBy(a => a.Name), "Id", "Name", recoveryTask.AssetId);
            return View(recoveryTask);
        }

        // GET: RecoveryTasks/Edit/5
        [Authorize(Roles = "Admin,DRCoordinator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recoveryTask = await _context.RecoveryTasks.FindAsync(id);
            if (recoveryTask == null)
            {
                return NotFound();
            }
            ViewData["AssetId"] = new SelectList(_context.Assets.OrderBy(a => a.Name), "Id", "Name", recoveryTask.AssetId);
            return View(recoveryTask);
        }

        // POST: RecoveryTasks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DRCoordinator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AssetId,Title,AssignedTo,Status,DueDate")] RecoveryTask recoveryTask)
        {
            if (id != recoveryTask.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recoveryTask);
                    await _context.SaveChangesAsync();

                    if (string.Equals(recoveryTask.Status, "Done", StringComparison.OrdinalIgnoreCase))
                    {
                        var asset = await _context.Assets
                            .Include(a => a.RecoveryTests)
                            .Include(a => a.RecoveryTasks)
                            .FirstOrDefaultAsync(a => a.Id == recoveryTask.AssetId);

                        if (asset != null && asset.OpenTaskCount == 0 && asset.ReadinessStatus == "At Risk")
                        {
                            TempData["SuccessMessage"] = $"All remediation tasks for '{asset.Name}' are Done — ready to log a re-test to verify RTO/RPO compliance.";
                        }
                        else
                        {
                            TempData["SuccessMessage"] = $"Task '{recoveryTask.Title}' updated.";
                        }
                    }
                    else
                    {
                        TempData["SuccessMessage"] = $"Task '{recoveryTask.Title}' updated.";
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecoveryTaskExists(recoveryTask.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { assetId = recoveryTask.AssetId });
            }
            ViewData["AssetId"] = new SelectList(_context.Assets.OrderBy(a => a.Name), "Id", "Name", recoveryTask.AssetId);
            return View(recoveryTask);
        }

        // GET: RecoveryTasks/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recoveryTask = await _context.RecoveryTasks
                .Include(r => r.Asset)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (recoveryTask == null)
            {
                return NotFound();
            }

            return View(recoveryTask);
        }

        // POST: RecoveryTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recoveryTask = await _context.RecoveryTasks.FindAsync(id);
            if (recoveryTask != null)
            {
                _context.RecoveryTasks.Remove(recoveryTask);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Task '{recoveryTask.Title}' was removed.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool RecoveryTaskExists(int id)
        {
            return _context.RecoveryTasks.Any(e => e.Id == id);
        }
    }
}
