using JobOffers.Models;
using JobOffers.ViewModels;
using System.Linq.Expressions;

namespace JobOffers.Interfaces
{
    public interface IJobsRepo
    {
        IQueryable<Job> Get(Expression<Func<Job, bool>> condition);
        Task<Job> GetAsync(int id);
        IQueryable<ApplyJob> GetApplyedUsersForJob(int jobId);
        Task<JobDetailesViewModel> DetailsAsync(int id);
        Task<int> CandidatesCount(int jobId);
        Task<bool> IsUserAppliedForJobBefore(int jobId, string userId);
        Task<Job> Create(Job job);
        Task<ApplyJob> Create(ApplyJob applyJob);
        void Edit(Job job);
        void Delete(Job job);
    }
}