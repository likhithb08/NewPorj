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

        // ─── Account Settings ─────────────────────────────────────────────────────

        [HttpGet("/Settings")]
        [HttpGet("/Settings/Index")]
        public async Task<IActionResult> Index()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
                return RedirectToAction("Login", "Account");

            var user = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null) return RedirectToAction("Login", "Account");

            return View("Index", user);
        }

        [HttpPost("/Settings/SaveProfile")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveProfile(string fullName, string phoneNumber)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
                return RedirectToAction("Login", "Account");

            var user = await _db.Users.FindAsync(userId);
            if (user == null) { TempData["Error"] = "User not found."; return RedirectToAction(nameof(Index)); }

            if (string.IsNullOrWhiteSpace(fullName))
            {
                TempData["Error"] = "Full name cannot be empty.";
                return RedirectToAction(nameof(Index));
            }

            user.FullName    = fullName.Trim();
            user.PhoneNumber = phoneNumber?.Trim() ?? user.PhoneNumber;
            await _db.SaveChangesAsync();
            await _auditLogService.LogAsync(userId, Enums.Actions.Updated, $"User:{userId}", null, "Profile updated", null, null);

            TempData["Success"] = "Profile updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("/Settings/ChangePassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 8)
            {
                TempData["PwdError"] = "New password must be at least 8 characters.";
                return RedirectToAction(nameof(Index));
            }
            if (newPassword != confirmPassword)
            {
                TempData["PwdError"] = "New password and confirmation do not match.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _db.Users.FindAsync(userId);
            if (user == null) { TempData["PwdError"] = "User not found."; return RedirectToAction(nameof(Index)); }

            var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
            var verify = hasher.VerifyHashedPassword(user, user.PasswordHash, currentPassword);
            if (verify == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Failed)
            {
                TempData["PwdError"] = "Current password is incorrect.";
                return RedirectToAction(nameof(Index));
            }

            user.PasswordHash = hasher.HashPassword(user, newPassword);
            await _db.SaveChangesAsync();
            await _auditLogService.LogAsync(userId, Enums.Actions.Updated, $"User:{userId}", null, "Password changed", null, null);

            TempData["PwdSuccess"] = "Password changed successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
