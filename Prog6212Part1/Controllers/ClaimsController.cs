using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Prog6212Part1.Areas.Identity.Data;
using Prog6212Part1.Models;
using Prog6212Part1.Areas.Identity.Data;

namespace Prog6212_Part1.Controllers
{ }
public class ClaimsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ClaimsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpGet]
    public IActionResult Claims()
    {
        return View();
    }

    // [Authorize(Roles = "Lecturer")]
    public async Task<IActionResult> Lists()
    {
        var claims = await _context.Claims.ToListAsync();
        return View(claims);
        // Ensure this matches the name of the view file
    }

    [HttpPost]
    public async Task<IActionResult> Claims(Claims claims)
    {
        if (ModelState.IsValid)
        {
            // Save supporting documents
            if (claims.SupportingDocuments != null && claims.SupportingDocuments.Any())
            {
                string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath,
                    "uploads");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                foreach (var file in claims.SupportingDocuments)
                {
                    if (file.Length > 0)
                    {
                        string fileName = Guid.NewGuid().ToString()
                            + Path.GetExtension(file.FileName);
                        string filePath = Path.Combine(uploadPath, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                    }
                }
            }

            // Calculate total hours
            claims.TotalHours = claims.HoursWorked * claims.RateHour;
            _context.Add(claims);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ClaimSubmitted));
        }
        return View(claims);
    }


    public IActionResult ClaimSubmitted()
    {
        return View();
    }
}




