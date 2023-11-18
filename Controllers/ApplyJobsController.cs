using JobOffers.Configurations;
using JobOffers.Interfaces;
using JobOffers.Models;
using JobOffers.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static JobOffers.Configurations.Helper;

namespace JobOffers.Controllers
{
    [Authorize(Roles = Roles.JobSeeker)]
    public class ApplyJobsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJobsRepo _jobsRepo;
        private readonly IApplyJobsRepo _applyJobsRepo;

        public ApplyJobsController(UserManager<ApplicationUser> userManager, IJobsRepo jobsRepo, IApplyJobsRepo applyJobsRepo)
        {
            _userManager = userManager;
            _jobsRepo = jobsRepo;
            _applyJobsRepo = applyJobsRepo;
        }

        public async Task<IActionResult> Index(string currentFilter, string searchString, int? pageNumber)
        {
            ApplicationUser loggedUser = await _userManager.FindByNameAsync(User.Identity.Name);

            if (loggedUser is null)
                return Unauthorized("Invalid User");

            if (searchString is not null)
                pageNumber = 1;
            else
                searchString = currentFilter;

            ViewData["CurrentFilter"] = searchString;

            var appliedJobs = _applyJobsRepo.Get(e => e.UserId == loggedUser.Id);

            if (!String.IsNullOrEmpty(searchString))
                appliedJobs = appliedJobs.Where(s => s.Job.Title.ToLower().Contains(searchString.Trim().ToLower()));

            int pageSize = PageSize.Home;
            return View(await PaginatedList<ApplyJob>.CreateAsync(appliedJobs.OrderByDescending(e => e.AddedOn).AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        //Apply for a job
        public async Task<IActionResult> Apply(int id)
        {
            ApplicationUser loggedUser = await _userManager.FindByNameAsync(User.Identity.Name);

            if (loggedUser is null)
                return Unauthorized("Invalid User");

            ApplyJobViewModel model = new()
            {
                JobId = id,
                UserId = loggedUser.Id
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Apply(ApplyJobViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid Model");

            try
            {
                if (await _jobsRepo.IsUserAppliedForJobBefore(model.JobId, model.UserId))
                    ViewBag.Result = "Already applied for this job";
                else
                {
                    ApplyJob applyJob = new()
                    {
                        JobId = model.JobId,
                        UserId = model.UserId,
                        Message = model.Message
                    };

                    await _jobsRepo.Create(applyJob);
                    ViewBag.Result = "Thank you for applying for this job. Our hiring team is currently reviewing all applications and we are planning to schedule interviews if you are among qualified candidates.";
                }

                return View();
            }

            catch (Exception e)
            {
                ModelState.AddModelError("Error", e.Message);
                return View(model);
            }
        }

        //Edit
        public async Task<IActionResult> Edit(int id)
        {
            ApplicationUser loggedUser = await _userManager.FindByNameAsync(User.Identity.Name);

            if (loggedUser is null)
                return Unauthorized("Invalid User");

            ApplyJob applyJob = await _applyJobsRepo.Get(id);

            if (applyJob is null)
                return NotFound("invalid user or job");

            return View(applyJob);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ApplyJob applyJob)
        {
            try
            {
                _applyJobsRepo.Edit(applyJob);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        //Delete
        public async Task<IActionResult> Delete(int id)
        {
            ApplicationUser loggedUser = await _userManager.FindByNameAsync(User.Identity.Name);

            if (loggedUser is null)
                return Unauthorized("Invalid User");

            ApplyJob applyJob = await _applyJobsRepo.Get(id);

            if (applyJob is null)
                return NotFound("invalid user or job");

            return View(applyJob);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(ApplyJob applyJob)
        {
            _applyJobsRepo.Delete(applyJob);
            return RedirectToAction(nameof(Index));
        }
    }
}