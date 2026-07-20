using LOCPS.Constants;
using LOCPS.Models;
using LOCPS.Services.Interfaces;
using LOCPS.ViewModels.Users;
using Microsoft.AspNetCore.Mvc;
using LOCPS.Enums;

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
                UserId    = u.UserId,
                UserName  = u.UserName,
                Email     = u.Email,
                FullName  = u.FullName,
                RoleName  = u.Role?.Roles.ToString() ?? RoleConstants.GetRoleFromId(u.RoleId).ToString(),
                IsActive  = u.IsActive,
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
                UserId      = user.UserId,
                UserName    = user.UserName,
                Email       = user.Email,
                FullName    = user.FullName,
                PhoneNumber = user.PhoneNumber,
                RoleId      = user.RoleId,
                IsActive    = user.IsActive
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

        // DELETE – POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);
                TempData["Success"] = "User account deleted.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        // TOGGLE ACTIVE – POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            var vm = new UserUpdateViewModel
            {
                UserId      = user.UserId,
                UserName    = user.UserName,
                Email       = user.Email,
                FullName    = user.FullName,
                PhoneNumber = user.PhoneNumber,
                RoleId      = user.RoleId,
                IsActive    = !user.IsActive   // toggle
            };
            await _userService.UpdateUserAsync(vm);
            TempData["Success"] = $"User {(vm.IsActive ? "activated" : "deactivated")} successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
