using LOCPS.Constants;
using LOCPS.Models;
using LOCPS.Services.Interfaces;
using LOCPS.ViewModels.Users;
using Microsoft.AspNetCore.Mvc;
using LOCPS.Enums;
using Microsoft.EntityFrameworkCore; // Added for DbUpdateException handling

namespace LOCPS.Controllers
{
    public class UserManagementController : Controller
    {
        private readonly IUserService _userService;

        public UserManagementController(IUserService userService)
        {
            _userService = userService;
        }

        // LIST all users
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            var vms = users.Select(u => new UserListViewModel
            {
                UserId = u.UserId,
                UserName = u.UserName,
                Email = u.Email,
                FullName = u.FullName,
                RoleName = u.Role?.Roles.ToString() ?? RoleConstants.GetRoleFromId(u.RoleId).ToString(),
                IsActive = u.IsActive,
                CreatedDate = u.CreatedDate
            }).ToList();
            return View(vms);
        }

        // CREATE – GET
        public IActionResult Create() => View(new UserCreateViewModel());

        // CREATE – POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _userService.RegisterUserAsync(model);
                TempData["Success"] = "User account created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        // EDIT – GET
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            var vm = new UserUpdateViewModel
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                RoleId = user.RoleId,
                IsActive = user.IsActive
            };
            return View(vm);
        }

        // EDIT – POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserUpdateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _userService.UpdateUserAsync(model);
                TempData["Success"] = "User account updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        // DELETE – POST (With precise database exception trapping)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);
                TempData["Success"] = "User account deleted successfully.";
            }
            catch (DbUpdateException dbEx)
            {
                // Capture foreign key rule blocks from SQL Server and display them safely
                TempData["Error"] = $"Database Error: Cannot delete this user because they are linked to existing loan records or audit logs. ({dbEx.InnerException?.Message ?? dbEx.Message})";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        // TOGGLE ACTIVE – POST (Safeguarded)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null) return NotFound();

                var vm = new UserUpdateViewModel
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    Email = user.Email,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    RoleId = user.RoleId,
                    IsActive = !user.IsActive   // toggle status
                };

                await _userService.UpdateUserAsync(vm);
                TempData["Success"] = $"User {(vm.IsActive ? "activated" : "deactivated")} successfully.";
            }
            catch (DbUpdateException dbEx)
            {
                TempData["Error"] = $"Database Error updating status: {dbEx.InnerException?.Message ?? dbEx.Message}";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}