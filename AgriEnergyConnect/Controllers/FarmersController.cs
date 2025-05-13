using AgriEnergyConnect.Data;
using AgriEnergyConnect.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriEnergyConnect.Controllers
{
    [Authorize(Roles = "Employee")]
    public class FarmersController : Controller
    {
        private readonly ApplicationDbContext _context;
        public FarmersController(ApplicationDbContext context) => _context = context;

        public IActionResult Index() => View(_context.Farmers.ToList());

        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(Farmer farmer)
        {
            if (ModelState.IsValid)
            {
                _context.Farmers.Add(farmer);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(farmer);
        }
    }
}
