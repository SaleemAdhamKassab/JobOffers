using JobOffers.Configurations;
using JobOffers.Interfaces;
using JobOffers.Models;
using JobOffers.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Expressions;
using static JobOffers.Configurations.Helper;

namespace JobOffers.Controllers
{
    [Authorize(Roles = Roles.Publisher)]

    public class JobsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJobsRepo _jobsRepo;
        private readonly ICategoriesRepo _categoriesRepo;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;


        public JobsController(UserManager<ApplicationUser> userManager, IJobsRepo jobsRepo, ICategoriesRepo categoriesRepo, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment)
        {
            _userManager = userManager;
            _jobsRepo = jobsRepo;
            _environment = environment;
            _categoriesRepo = categoriesRepo;
        }

        //Note: Index method receives a parameters from the query string in the URL
        //https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/sort-filter-page?view=aspnetcore-7.0
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user is null)
                return BadRequest("Invalild User");

            Expression<Func<Job, bool>> publisherId = e => e.UserId == user.Id;

            ViewData["CurrentSort"] = sortOrder;
            ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString is not null)
                pageNumber = 1;
            else
                searchString = currentFilter;


            ViewData["CurrentFilter"] = searchString;

            IQueryable<Job> jobs = _jobsRepo.Get(publisherId);

            if (!String.IsNullOrEmpty(searchString))
                jobs = jobs.Where(s => s.Title.Contains(searchString));


            jobs = sortOrder switch
            {
                "title_desc" => jobs.OrderByDescending(s => s.Title),
                "Date" => jobs.OrderBy(s => s.AddedOn),
                "date_desc" => jobs.OrderByDescending(s => s.AddedOn),
                _ => jobs.OrderBy(s => s.Title),
            };

            int pageSize = PageSize.Default;
            return View(await PaginatedList<Job>.CreateAsync(jobs.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        public async Task<ActionResult> GetApplyedUsersForJob(int id, string currentFilter, string searchString, int? pageNumber)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user is null)
                return BadRequest("Invalild User");

            if (searchString is not null)
                pageNumber = 1;
            else
                searchString = currentFilter;

            ViewData["CurrentFilter"] = searchString;

            IQueryable<ApplyJob> applyJobs = _jobsRepo.GetApplyedUsersForJob(id);

            if (!String.IsNullOrEmpty(searchString))
                applyJobs = applyJobs
                    .Where(s => s.User.FirstName.ToLower().Contains(searchString.Trim().ToLower()) || s.User.LastName.ToLower().Contains(searchString.Trim()
                .ToLower()));

            int pageSize = PageSize.Candidates;
            return View(await PaginatedList<ApplyJob>.CreateAsync(applyJobs.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        //Details
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            if (id < 1)
                return NotFound($"Invalid Job Id {id}!!");

            JobDetailesViewModel model = await _jobsRepo.DetailsAsync(id);

            if (model == null)
                return NotFound();

            return View(model);
        }


        //Create
        public async Task<IActionResult> Create()
        {
            AddJobViewModel model = new()
            {
                Categories = await _categoriesRepo.Get()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddJobViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid Model");

            ApplicationUser loggedUser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (loggedUser is null)
                return Unauthorized("Invalid User!");

            if (!UploadFile.ImageAllowedExtenstions.Contains(Path.GetExtension(model.Image.FileName).ToLower()))
                return BadRequest("Only .png, jpeg and .jpg images are allowed!");

            if (model.Image.Length > UploadFile.MaxAllowedSize)
                return BadRequest("Max allowed size for Image is 1MB!");

            try
            {
                //Upload file reference: https://www.youtube.com/watch?v=T4Ft_gpXAXk&ab_channel=ASPSnippets 
                string wwwPath = _environment.WebRootPath;
                string path = Path.Combine(wwwPath, @"images\Jobs");
                string fileName = Path.GetFileName(model.Image.FileName);

                using (FileStream stream = new(Path.Combine(path, fileName), FileMode.Create))
                {
                    model.Image.CopyTo(stream);
                }

                Job job = new()
                {
                    Title = model.Title,
                    CompanyName = model.CompanyName,
                    Description = model.Description,
                    Image = model.Image.FileName,
                    CategoryId = model.CategoryId,
                    UserId = loggedUser.Id,
                    AddedOn = DateTime.Now
                };

                await _jobsRepo.Create(job);
                return RedirectToAction(nameof(Index));
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
            JobDetailesViewModel job = await _jobsRepo.DetailsAsync(id);

            EditJobViewModel model = new()
            {
                Id = job.Id,
                CompanyName = job.CompanyName,
                Title = job.Title,
                Category = job.Category,
                Description = job.Description
            };

            if (model == null)
                return NotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditJobViewModel model)
        {
            ApplicationUser loggedUser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (loggedUser == null)
                return Unauthorized($"invalid User!");

            Job job = await _jobsRepo.GetAsync(id);

            if (job is null)
                return BadRequest($"The Job ({model.Title}) doesn't exists!");

            try
            {
                job.Id = model.Id;
                job.Title = model.Title;
                job.CompanyName = model.CompanyName;
                job.Description = model.Description;

                _jobsRepo.Edit(job);
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
            JobDetailesViewModel model = await _jobsRepo.DetailsAsync(id);

            if (model == null)
                return NotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(JobDetailesViewModel model)
        {
            Job job = await _jobsRepo.GetAsync(model.Id);

            if (job is null)
                return NotFound();

            try
            {
                _jobsRepo.Delete(job);

                string wwwPath = _environment.WebRootPath;
                string path = Path.Combine(wwwPath, @"images\Jobs");
                string image = $"{path}\\{job.Image}";
                {
                    if (System.IO.File.Exists(image))
                        System.IO.File.Delete(image);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}