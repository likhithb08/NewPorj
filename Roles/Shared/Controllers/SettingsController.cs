using LOCPS.Common;
using LOCPS.Constants;
using LOCPS.Data;
using LOCPS.Models;
using LOCPS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LOCPS.Controllers
{
    public class SettingsController : Controller
    {
        private readonly IAuditLogService _auditLogService;
        private readonly AppDbContext     _db;

        public SettingsController(IAuditLogService auditLogService, AppDbContext db)
        {
            _auditLogService = auditLogService;
            _db              = db;
        }

        public IActionResult Index() => View();

        // ─── Scoring Rules ────────────────────────────────────────────────────────

        [HttpGet]
        public async Task<IActionResult> ScoringRules()
        {
            var config = await _db.ScoringConfigs.FirstOrDefaultAsync()
                         ?? new ScoringConfig();
            return View(config);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ScoringRules(ScoringConfig model)
        {
            // Validate weights sum to 100
            int total = model.BureauScoreWeight + model.DebtToIncomeWeight +
                        model.CreditHistoryAgeWeight + model.RepaymentConsistencyWeight +
                        model.CreditUtilizationWeight;

            if (total != 100)
            {
                ModelState.AddModelError(string.Empty,
                    $"Scoring weights must sum to 100%. Current total: {total}%");
                return View(model);
            }

            if (!ModelState.IsValid)
                return View(model);

            var existing = await _db.ScoringConfigs.FirstOrDefaultAsync();
            if (existing == null)
            {
                model.ConfigId = 1;
                model.LastUpdated = DateTime.UtcNow;
                _db.ScoringConfigs.Add(model);
            }
            else
            {
                existing.MinCreditScore            = model.MinCreditScore;
                existing.BureauScoreWeight         = model.BureauScoreWeight;
                existing.DebtToIncomeWeight        = model.DebtToIncomeWeight;
                existing.CreditHistoryAgeWeight    = model.CreditHistoryAgeWeight;
                existing.RepaymentConsistencyWeight = model.RepaymentConsistencyWeight;
                existing.CreditUtilizationWeight   = model.CreditUtilizationWeight;
                existing.LastUpdated               = DateTime.UtcNow;

                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdStr, out var uid))
                    existing.UpdatedByUserId = uid;
            }

            await _db.SaveChangesAsync();

            // Log the config update
            var actorIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int actorId    = int.TryParse(actorIdStr, out var aid) ? aid : 0;
            await _auditLogService.LogAsync(actorId, Enums.Actions.Updated,
                "ScoringConfig:1", null, $"MinScore={model.MinCreditScore}", null, null);

            TempData["Success"] = "Scoring engine configuration saved successfully.";
            return RedirectToAction(nameof(ScoringRules));
        }

        // ─── Audit Logs ───────────────────────────────────────────────────────────

        public async Task<IActionResult> AuditLogs()
        {
            var query  = new PagedQuery { Page = 1, PageSize = 100 };
            var result = await _auditLogService.GetPagedAsync(query);
            return View(result);
        }
    }
}
