using JobOffers.Models;

namespace JobOffers.Interfaces
{
    public interface ICategoriesRepo
    {
        Task<Category> Get(int id);
        Task<Category> Get(string name);
        Task<List<Category>> Get();
        Task<List<Category>> Search(string searchCriteria);
        Task<bool> IsNameUsedForAnotherCategory(Category category);
        Task<Category> Create(Category category);
        void Edit(Category category);
        void Delete(Category category);
    }
}
