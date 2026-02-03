using Microsoft.AspNetCore.Mvc;
using JobApplicationWeb.Data;
using System.Linq;

namespace JobApplicationWeb.Controllers
{
    public class JobsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JobsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var jobs = _context.Jobs.ToList();
            
            // Get current user ID from session
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            
            // Create a dictionary of jobs the user has applied to
            var appliedJobIds = _context.Applications
                .Where(a => a.UserId == userId)
                .Select(a => a.JobId)
                .ToList();

            // Pass both jobs and applied job IDs to view
            ViewData["AppliedJobIds"] = appliedJobIds;
            
            return View(jobs);
        }

        // GET: Jobs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Jobs/Create
        [HttpPost]
        public IActionResult Create(JobApplicationWeb.Models.Job job)
        {
            if (ModelState.IsValid)
            {
                job.CreatedAt = DateTime.Now;
                _context.Jobs.Add(job);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(job);
        }
    }
}
