using AgriEnergyConnect.Data;
using AgriEnergyConnect.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgriEnergyConnect.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index(string category, DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.Products.Include(p => p.Farmer).AsQueryable();

            if (!string.IsNullOrEmpty(category))
                query = query.Where(p => p.Category == category);

            if (fromDate.HasValue)
                query = query.Where(p => p.ProductionDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(p => p.ProductionDate <= toDate.Value);

            return View(query.ToList());
        }

        [Authorize(Roles = "Farmer")]
        public IActionResult Create() => View();

        [HttpPost]
        [Authorize(Roles = "Farmer")]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var farmer = _context.Farmers.FirstOrDefault(f => f.Email == user.Email);
                product.FarmerId = farmer.Id;

                _context.Products.Add(product);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }
    }
}
