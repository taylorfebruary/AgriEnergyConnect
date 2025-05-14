using AgriEnergyConnect.Data;
using AgriEnergyConnect.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgriEnergyConnect.Controllers
{
    [Authorize(Roles = "Employee")]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmployeeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> AllProducts(string category, DateTime? startDate, DateTime? endDate)
        {
            var products = _context.Products.Include(p => p.Farmer).AsQueryable();

            if (!string.IsNullOrEmpty(category))
                products = products.Where(p => p.Category.Contains(category));

            if (startDate.HasValue)
                products = products.Where(p => p.ProductionDate >= startDate.Value);

            if (endDate.HasValue)
                products = products.Where(p => p.ProductionDate <= endDate.Value);

            return View(await products.ToListAsync());
        }

        public IActionResult AddFarmer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddFarmer(Register model)
        {
            if (ModelState.IsValid)
            {
                var farmer = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Role = "Farmer"
                };

                var result = await _userManager.CreateAsync(farmer, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(farmer, "Farmer");
                    return RedirectToAction("AllProducts");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }
    }
}
