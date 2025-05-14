using AgriEnergyConnect.Data;
using AgriEnergyConnect.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Farmer")]
public class FarmerController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public FarmerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> MyProducts()
    {
        var user = await _userManager.GetUserAsync(User);
        var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.Email == user.Email);

        var products = await _context.Products
            .Where(p => p.FarmerId == farmer.Id)
            .ToListAsync();

        return View(products);
    }

    public IActionResult AddProduct() => View();

    [HttpPost]
    public async Task<IActionResult> AddProduct(Product product)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);
            var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.Email == user.Email);

            product.FarmerId = farmer.Id;
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("MyProducts");
        }
        return View(product);
    }
}
