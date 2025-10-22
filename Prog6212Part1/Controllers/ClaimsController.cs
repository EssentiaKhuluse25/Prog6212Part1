using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Prog6212Part1.Areas.Identity.Data;
using Prog6212Part1.Models;

namespace Prog6212Part1.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ClaimsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // === Lecturer Claim Form ===
        [HttpGet]
        public IActionResult Claims()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Claims(Claims claims)
        {
            if (ModelState.IsValid)
            {
                List<string> savedFileNames = new List<string>();

                // Save supporting documents
                if (claims.SupportingDocuments != null && claims.SupportingDocuments.Any())
                {
                    string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);

                    foreach (var file in claims.SupportingDocuments)
                    {
                        if (file.Length > 0)
                        {
                            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                            string filePath = Path.Combine(uploadPath, fileName);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }
                            savedFileNames.Add(fileName);
                        }
                    }
                }

                claims.UploadedFileNames = string.Join(",", savedFileNames);
                claims.TotalHours = claims.HoursWorked * claims.RateHour;
                claims.Status = "Pending"; // default status

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

        // === List for Lecturers ===
        public async Task<IActionResult> Lists()
        {
            var claims = await _context.Claims.ToListAsync();
            return View(claims);
        }

        // === List for Coordinators/Managers to verify claims ===
        [HttpGet]
        public async Task<IActionResult> VerifyClaims()
        {
            var pendingClaims = await _context.Claims
                .Where(c => c.Status == "Pending")
                .ToListAsync();
            return View(pendingClaims);
        }

        // === Approve ===
        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null) return NotFound();

            claim.Status = "Approved";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(VerifyClaims));
        }

        // === Reject ===
        [HttpPost]
        public async Task<IActionResult> Reject(int id)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null) return NotFound();

            claim.Status = "Rejected";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(VerifyClaims));
        }
    }
}
