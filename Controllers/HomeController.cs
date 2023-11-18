using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using JobOffers.Models;
using JobOffers.Data;
using Microsoft.EntityFrameworkCore;
using JobOffers.Interfaces;
using JobOffers.Configurations;
using static JobOffers.Configurations.Helper;

namespace JobOffers.Controllers
{
    public class HomeController : Controller
    {
        private readonly IJobsRepo _jobsRepo;

        public HomeController(ApplicationDbContext applicationDbContext, IJobsRepo jobsRepo) => _jobsRepo = jobsRepo;

        //Note: Index method receives a parameters from the query string in the URL
        //https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/sort-filter-page?view=aspnetcore-7.0
        public async Task<IActionResult> Index(string currentFilter, string searchString, int? pageNumber)
        {
            if (searchString is not null)
                pageNumber = 1;
            else
                searchString = currentFilter;

            ViewData["CurrentFilter"] = searchString;

            var jobs = _jobsRepo.Get(e => true);

            if (!String.IsNullOrEmpty(searchString))
                jobs = jobs.Where(s => s.Title.ToLower().Contains(searchString.Trim().ToLower()));

            int pageSize = PageSize.Home;
            return View(await PaginatedList<Job>.CreateAsync(jobs.OrderByDescending(e => e.AddedOn).AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}