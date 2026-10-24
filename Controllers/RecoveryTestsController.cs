using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Green_Basket.Data;
using Green_Basket.Models;

namespace Green_Basket.Controllers
{
    [Authorize]
    public class RecoveryTestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RecoveryTestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: RecoveryTests
        [Authorize(Roles = "Admin,DRCoordinator,Staff")]
        public async Task<IActionResult> Index(int? assetId)
        {
            var query = _context.RecoveryTests.Include(r => r.Asset).AsQueryable();
            if (assetId.HasValue)
            {
                query = query.Where(r => r.AssetId == assetId.Value);
                ViewBag.CurrentAsset = await _context.Assets.FindAsync(assetId.Value);
            }

            var tests = await query.OrderByDescending(t => t.TestDate).ToListAsync();
            return View(tests);
        }

        // GET: RecoveryTests/Details/5
        [Authorize(Roles = "Admin,DRCoordinator,Staff")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recoveryTest = await _context.RecoveryTests
                .Include(r => r.Asset)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (recoveryTest == null)
            {
                return NotFound();
            }

            return View(recoveryTest);
        }

        // GET: RecoveryTests/Create
        [Authorize(Roles = "Admin,DRCoordinator")]
        public IActionResult Create(int? assetId)
        {
            ViewData["AssetId"] = new SelectList(_context.Assets.OrderBy(a => a.Name), "Id", "Name", assetId);
            var test = new RecoveryTest { TestDate = DateTime.Today };
            if (assetId.HasValue)
            {
                test.AssetId = assetId.Value;
            }
            return View(test);
        }

        // POST: RecoveryTests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DRCoordinator")]
        public async Task<IActionResult> Create([Bind("AssetId,TestDate,Result,ActualRtoMinutes,ActualRpoMinutes,Notes,LessonsLearned")] RecoveryTest recoveryTest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(recoveryTest);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Recovery drill test record successfully saved.";
                return RedirectToAction(nameof(Index), new { assetId = recoveryTest.AssetId });
            }
            ViewData["AssetId"] = new SelectList(_context.Assets.OrderBy(a => a.Name), "Id", "Name", recoveryTest.AssetId);
            return View(recoveryTest);
        }

        // GET: RecoveryTests/Edit/5
        [Authorize(Roles = "Admin,DRCoordinator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recoveryTest = await _context.RecoveryTests.FindAsync(id);
            if (recoveryTest == null)
            {
                return NotFound();
            }
            ViewData["AssetId"] = new SelectList(_context.Assets.OrderBy(a => a.Name), "Id", "Name", recoveryTest.AssetId);
            return View(recoveryTest);
        }

        // POST: RecoveryTests/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DRCoordinator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AssetId,TestDate,Result,ActualRtoMinutes,ActualRpoMinutes,Notes,LessonsLearned")] RecoveryTest recoveryTest)
        {
            if (id != recoveryTest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recoveryTest);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Recovery test record updated.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecoveryTestExists(recoveryTest.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { assetId = recoveryTest.AssetId });
            }
            ViewData["AssetId"] = new SelectList(_context.Assets.OrderBy(a => a.Name), "Id", "Name", recoveryTest.AssetId);
            return View(recoveryTest);
        }

        // GET: RecoveryTests/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recoveryTest = await _context.RecoveryTests
                .Include(r => r.Asset)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (recoveryTest == null)
            {
                return NotFound();
            }

            return View(recoveryTest);
        }

        // POST: RecoveryTests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recoveryTest = await _context.RecoveryTests.FindAsync(id);
            if (recoveryTest != null)
            {
                _context.RecoveryTests.Remove(recoveryTest);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Recovery test record removed.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool RecoveryTestExists(int id)
        {
            return _context.RecoveryTests.Any(e => e.Id == id);
        }
    }
}
