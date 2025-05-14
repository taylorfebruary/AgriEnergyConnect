using AgriEnergyConnect.Data;
using AgriEnergyConnect.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Employee")]
public class EmployeeController : Controller
{
    private readonly ApplicationDbContext _context;

    public EmployeeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> AllProducts(string category, DateTime? startDate, DateTime? endDate)
    {
        var products = _context.Products.Include(p => p.Farmer).AsQueryable();

        if (!string.IsNullOrEmpty(category))
            products = products.Where(p => p.Category == category);

        if (startDate.HasValue)
            products = products.Where(p => p.ProductionDate >= startDate.Value);

        if (endDate.HasValue)
            products = products.Where(p => p.ProductionDate <= endDate.Value);

        return View(await products.ToListAsync());
    }

    public IActionResult AddFarmer() => View();

    [HttpPost]
    public async Task<IActionResult> AddFarmer(Farmer farmer)
    {
        if (ModelState.IsValid)
        {
            _context.Farmers.Add(farmer);
            await _context.SaveChangesAsync();
            return RedirectToAction("AllProducts");
        }
        return View(farmer);
    }
}
