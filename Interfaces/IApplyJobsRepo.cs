using JobOffers.Models;
using System.Linq.Expressions;

namespace JobOffers.Interfaces
{
    public interface IApplyJobsRepo
    {
        IQueryable<ApplyJob> Get(Expression<Func<ApplyJob, bool>> condition);
        Task<ApplyJob> Get(int id);
        void Edit(ApplyJob applyJob);
        void Delete(ApplyJob applyJob);
    }
}