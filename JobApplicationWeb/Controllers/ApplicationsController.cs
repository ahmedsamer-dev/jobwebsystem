using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobApplicationWeb.Data;
using JobApplicationWeb.Models;
using System.Linq;

namespace JobApplicationWeb.Controllers
{
    public class ApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApplicationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // Apply for Job
        // =========================
        // GET: Applications/Apply?jobId=5
        public IActionResult Apply(int jobId)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            if (userId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            // Check if user has already applied for this job
            var existingApplication = _context.Applications
                .FirstOrDefault(a => a.UserId == userId && a.JobId == jobId);

            if (existingApplication != null)
            {
                TempData["ErrorMessage"] = "You have already applied for this job.";
                return RedirectToAction("Index", "Jobs");
            }

            // Create new application
            var application = new JobApplication
            {
                JobId = jobId,
                UserId = userId,
                Status = "Pending",
                AppliedAt = DateTime.Now
            };

            _context.Applications.Add(application);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Application submitted successfully.";
            return RedirectToAction("Index", "Jobs");
        }

        // =========================
        // My Applications
        // =========================
        // GET: Applications/MyApplications
        public IActionResult MyApplications()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            if (userId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            var applications = _context.Applications
                .Include(a => a.Job)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.AppliedAt)
                .ToList();

            return View(applications);
        }
    }
}
