using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobOffers.ViewModels;

namespace JobOffers.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public RolesController(RoleManager<IdentityRole> roleManager) => _roleManager = roleManager;

        public async Task<IActionResult> Index() => View(await _roleManager.Roles.OrderBy(e => e.Name).ToListAsync());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(RoleFormViewModel model)
        {
            if (await _roleManager.RoleExistsAsync(model.Name))
            {
                ModelState.AddModelError("Name", "Role is exists!");
                return View("Index", await _roleManager.Roles.ToListAsync());
            }


            await _roleManager.CreateAsync(new IdentityRole()
            {
                Name = model.Name.Trim(),
                NormalizedName = model.Name.Trim().ToUpper(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            });

            return RedirectToAction(nameof(Index));
        }
    }
}