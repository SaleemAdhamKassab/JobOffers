using JobOffers.Data;
using JobOffers.Interfaces;
using JobOffers.Models;
using JobOffers.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Policy;

namespace JobOffers.Repositories
{
    public class JobsRepo : IJobsRepo
    {
        private readonly ApplicationDbContext _db;

        public JobsRepo(ApplicationDbContext applicationDbContext) => _db = applicationDbContext;

        public IQueryable<Job> Get(Expression<Func<Job, bool>> condition) => _db.Jobs.Include(e => e.User).Include(e => e.Category).Where(condition);

        public async Task<Job> GetAsync(int id) => await _db.Jobs.FindAsync(id);

        public async Task<JobDetailesViewModel> DetailsAsync(int id) =>
           await _db.Jobs
           .Select(e => new JobDetailesViewModel()
           {
               Id = e.Id,
               Title = e.Title,
               CompanyName = e.CompanyName,
               Category = e.Category.Name,
               User = e.User.FirstName + ' ' + e.User.LastName,
               Description = e.Description,
               Image = e.Image,
               AddedOn = e.AddedOn,
           })
            .SingleAsync(e => e.Id == id);

        public IQueryable<ApplyJob> GetApplyedUsersForJob(int jobId) =>
           _db.ApplyJobs
               .Include(e => e.User)
               .Where(e => e.JobId == jobId);
        public async Task<int> CandidatesCount(int jobId) => await _db.ApplyJobs.Where(e => e.JobId == jobId).CountAsync();

        public async Task<bool> IsUserAppliedForJobBefore(int jobId, string userId) => _db.ApplyJobs.Where(e => e.JobId == jobId && e.UserId == userId).Any();

        public async Task<Job> Create(Job job)
        {
            await _db.AddAsync(job);
            _db.SaveChanges();

            return job;
        }

        public async Task<ApplyJob> Create(ApplyJob applyJob)
        {
            await _db.AddAsync(applyJob);
            _db.SaveChanges();

            return applyJob;
        }

        public void Edit(Job job)
        {
            _db.Update(job);
            _db.SaveChanges();
        }

        public void Delete(Job job)
        {
            _db.Remove(job);
            _db.SaveChanges();
        }
    }
}