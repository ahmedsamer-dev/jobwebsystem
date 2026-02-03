using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobApplicationWeb.Data;
using System.Linq;

namespace JobApplicationWeb.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // Admin Dashboard
        // =========================
        // GET: Admin/Index
        public IActionResult Index()
        {
            // Role-based access control
            string userRole = HttpContext.Session.GetString("UserRole") ?? "";
            if (userRole != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            // Get all applications with related data
            var applications = _context.Applications
                .Include(a => a.User)
                .Include(a => a.Job)
                .OrderByDescending(a => a.AppliedAt)
                .ToList();

            // Calculate statistics
            var statistics = new
            {
                TotalApplications = applications.Count,
                PendingApplications = applications.Count(a => a.Status == "Pending"),
                AcceptedApplications = applications.Count(a => a.Status == "Approved"),
                RejectedApplications = applications.Count(a => a.Status == "Rejected")
            };

            // Pass statistics and applications to view
            ViewData["Statistics"] = statistics;

            return View(applications);
        }

        // =========================
        // Update Application Status
        // =========================
        // POST: Admin/UpdateStatus
        [HttpPost]
        public IActionResult UpdateStatus(int applicationId, string status)
        {
            // Role-based access control
            string userRole = HttpContext.Session.GetString("UserRole") ?? "";
            if (userRole != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            // Validate status
            if (!new[] { "Pending", "Approved", "Rejected" }.Contains(status))
            {
                TempData["ErrorMessage"] = "Invalid status.";
                return RedirectToAction("Index");
            }

            // Find and update application
            var application = _context.Applications.FirstOrDefault(a => a.Id == applicationId);
            if (application == null)
            {
                TempData["ErrorMessage"] = "Application not found.";
                return RedirectToAction("Index");
            }

            application.Status = status;
            _context.SaveChanges();

            string statusMessage = status == "Approved" ? "accepted" : "rejected";
            TempData["SuccessMessage"] = $"Application {statusMessage} successfully.";

            return RedirectToAction("Index");
        }
    }
}
