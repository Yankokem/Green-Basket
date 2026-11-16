using System.Text.Json;
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

            var tests = await query.OrderByDescending(t => t.TestDate).ThenByDescending(t => t.Id).ToListAsync();
            await PopulateAssetTargetsAsync(assetId);
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
                    .ThenInclude(a => a!.RecoveryTasks)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (recoveryTest == null)
            {
                return NotFound();
            }

            await PopulateAssetTargetsAsync(recoveryTest.AssetId);
            return View(recoveryTest);
        }

        // GET: RecoveryTests/Create
        [Authorize(Roles = "Admin,DRCoordinator")]
        public async Task<IActionResult> Create(int? assetId)
        {
            await PopulateAssetTargetsAsync(assetId);
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
        public async Task<IActionResult> Create(
            [Bind("AssetId,TestDate,Result,ActualRtoMinutes,ActualRpoMinutes,Notes,LessonsLearned")] RecoveryTest recoveryTest,
            bool autoCreateTask = true)
        {
            var eval = await EvaluateAndValidateTestAsync(recoveryTest);

            if (ModelState.IsValid && eval.Asset != null)
            {
                _context.Add(recoveryTest);

                RecoveryTask? remediationTask = null;
                if (recoveryTest.Result == "Failed" && autoCreateTask)
                {
                    var breachSummary = BuildBreachSummary(eval.Asset, recoveryTest);
                    remediationTask = new RecoveryTask
                    {
                        AssetId = eval.Asset.Id,
                        Title = $"Remediate DR drill failure ({breachSummary}) — {recoveryTest.TestDate:yyyy-MM-dd}",
                        AssignedTo = eval.Asset.Owner,
                        Status = "Pending",
                        DueDate = DateTime.Today.AddDays(7)
                    };
                    _context.RecoveryTasks.Add(remediationTask);
                }

                await _context.SaveChangesAsync();

                if (recoveryTest.Result == "Passed")
                {
                    TempData["SuccessMessage"] = $"Drill passed — actual RTO/RPO met targets for '{eval.Asset.Name}'. Readiness verified.";
                }
                else
                {
                    var prefix = eval.AutoFailed
                        ? "RTO/RPO targets breached — drill automatically classified as Failed."
                        : "Drill recorded as Failed.";
                    var suffix = remediationTask != null
                        ? $" Follow-up recovery task assigned to {eval.Asset.Owner}."
                        : " Assign remediation tasks or update recovery procedures before re-testing.";
                    TempData["WarningMessage"] = prefix + suffix;
                }

                return RedirectToAction(nameof(Details), new { id = recoveryTest.Id });
            }

            await PopulateAssetTargetsAsync(recoveryTest.AssetId);
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
            await PopulateAssetTargetsAsync(recoveryTest.AssetId);
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

            var eval = await EvaluateAndValidateTestAsync(recoveryTest);

            if (ModelState.IsValid && eval.Asset != null)
            {
                try
                {
                    _context.Update(recoveryTest);
                    await _context.SaveChangesAsync();

                    if (recoveryTest.Result == "Passed")
                    {
                        TempData["SuccessMessage"] = $"Recovery test updated — RTO/RPO targets met for '{eval.Asset.Name}'.";
                    }
                    else
                    {
                        TempData["WarningMessage"] = eval.AutoFailed
                            ? "Updated metrics breach target RTO/RPO — result automatically set to Failed. Review remediation tasks before re-testing."
                            : "Recovery test updated as Failed. Ensure corrective tasks and procedures are addressed.";
                    }
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
                return RedirectToAction(nameof(Details), new { id = recoveryTest.Id });
            }

            await PopulateAssetTargetsAsync(recoveryTest.AssetId);
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

            await PopulateAssetTargetsAsync(recoveryTest.AssetId);
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

        private async Task PopulateAssetTargetsAsync(int? selectedAssetId = null)
        {
            var assets = await _context.Assets.OrderBy(a => a.Name).ToListAsync();
            ViewData["AssetId"] = new SelectList(assets, "Id", "Name", selectedAssetId);
            var targetMap = assets.ToDictionary(
                a => a.Id.ToString(),
                a => new { name = a.Name, rto = a.RtoMinutes, rpo = a.RpoMinutes, owner = a.Owner }
            );
            ViewBag.AssetTargetsJson = JsonSerializer.Serialize(targetMap);
        }

        /// <summary>
        /// Enforces RTO/RPO threshold evaluation and requires root-cause documentation on failed drills.
        /// </summary>
        private async Task<(Asset? Asset, bool AutoFailed)> EvaluateAndValidateTestAsync(RecoveryTest recoveryTest)
        {
            var asset = await _context.Assets.FindAsync(recoveryTest.AssetId);
            if (asset == null)
            {
                return (null, false);
            }

            bool isRtoMet = recoveryTest.ActualRtoMinutes <= asset.RtoMinutes;
            bool isRpoMet = recoveryTest.ActualRpoMinutes <= asset.RpoMinutes;
            bool autoFailed = false;

            if (!isRtoMet || !isRpoMet)
            {
                if (!string.Equals(recoveryTest.Result, "Failed", StringComparison.OrdinalIgnoreCase))
                {
                    autoFailed = true;
                }
                recoveryTest.Result = "Failed";
            }

            if (string.Equals(recoveryTest.Result, "Failed", StringComparison.OrdinalIgnoreCase)
                && string.IsNullOrWhiteSpace(recoveryTest.LessonsLearned))
            {
                ModelState.AddModelError(
                    nameof(recoveryTest.LessonsLearned),
                    "Lessons Learned (root cause) is required whenever a recovery test fails or breaches RTO/RPO targets.");
            }

            return (asset, autoFailed);
        }

        private static string BuildBreachSummary(Asset asset, RecoveryTest test)
        {
            var breaches = new List<string>();
            if (test.ActualRtoMinutes > asset.RtoMinutes)
            {
                breaches.Add($"+{test.ActualRtoMinutes - asset.RtoMinutes}m RTO");
            }
            if (test.ActualRpoMinutes > asset.RpoMinutes)
            {
                breaches.Add($"+{test.ActualRpoMinutes - asset.RpoMinutes}m RPO");
            }
            return breaches.Any() ? string.Join(", ", breaches) : "Operational failure";
        }

        private bool RecoveryTestExists(int id)
        {
            return _context.RecoveryTests.Any(e => e.Id == id);
        }
    }
}
