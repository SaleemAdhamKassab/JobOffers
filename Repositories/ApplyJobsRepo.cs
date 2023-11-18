using JobOffers.Data;
using JobOffers.Interfaces;
using JobOffers.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace JobOffers.Repositories
{
    public class ApplyJobsRepo : IApplyJobsRepo
    {
        private readonly ApplicationDbContext _db;
        public ApplyJobsRepo(ApplicationDbContext applicationDbContext) => _db = applicationDbContext;

        public IQueryable<ApplyJob> Get(Expression<Func<ApplyJob, bool>> condition) => _db.ApplyJobs.Include(e => e.User).Include(e => e.Job).ThenInclude(e => e.Category).Where(condition);

        public async Task<ApplyJob> Get(int id) => await _db.ApplyJobs.Include(e => e.User).Include(e => e.Job).ThenInclude(e => e.Category).SingleOrDefaultAsync(e => e.Id == id);

        public void Edit(ApplyJob applyJob)
        {
            _db.Update(applyJob);
            _db.SaveChanges();
        }

        public void Delete(ApplyJob applyJob)
        {
            _db.Remove(applyJob);
            _db.SaveChanges();
        }

    }
}