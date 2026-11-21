using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ScadaWebUI.Models;
using ScadaWebUI.Security;

namespace ScadaWebUI.Controllers;

[Authorize(Roles = AppRoles.Admin)]
public class UsersController : Controller
{
    private static readonly string[] AllowedRoles =
        { AppRoles.Operator, AppRoles.Technician, AppRoles.Admin };

    private readonly UserManager<IdentityUser> _userManager;

    public UsersController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        List<UserRoleRow> rows = new();
        List<IdentityUser> users = _userManager.Users.OrderBy(u => u.Email).ToList();

        foreach (IdentityUser user in users)
        {
            IList<string> roles = await _userManager.GetRolesAsync(user);
            rows.Add(new UserRoleRow
            {
                Id = user.Id,
                Email = user.Email ?? "",
                Role = roles.Count > 0 ? roles[0] : ""
            });
        }

        ViewBag.Roles = AllowedRoles;
        return View(rows);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetRole(string id, string role)
    {
        if (!AllowedRoles.Contains(role))
        {
            return BadRequest();
        }

        IdentityUser? user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        if (user.Id == _userManager.GetUserId(User) && role != AppRoles.Admin)
        {
            TempData["Message"] = "You cannot remove your own Admin role.";
            return RedirectToAction("Index");
        }

        IList<string> currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, role);
        await _userManager.UpdateSecurityStampAsync(user);

        TempData["Message"] = $"{user.Email} is now {role}";
        return RedirectToAction("Index");
    }
}