using JobOffers.Data;
using JobOffers.Interfaces;
using JobOffers.Models;
using Microsoft.EntityFrameworkCore;

namespace JobOffers.Repositories
{
    public class CategoriesRepo : ICategoriesRepo
    {
        private readonly ApplicationDbContext _db;
        public CategoriesRepo(ApplicationDbContext applicationDbContext) => _db = applicationDbContext;


        public async Task<Category> Get(int id) => await _db.Categories.FindAsync(id);

        public async Task<Category> Get(string name) => await _db.Categories.Where(e => e.Name.Trim().ToLower() == name.Trim().ToLower()).FirstOrDefaultAsync();

        public async Task<List<Category>> Get() => await _db.Categories.OrderBy(e => e.Name).ToListAsync();

        public async Task<List<Category>> Search(string searchCriteria) => await _db.Categories.Where(e => e.Name.Trim().ToLower().Contains(searchCriteria.Trim().ToLower())).ToListAsync();

        public async Task<bool> IsNameUsedForAnotherCategory(Category category)
        {
            Category categoryToCheck = await Get(category.Name);

            if (categoryToCheck == null)
                return false;

            if (categoryToCheck.Name == category.Name && categoryToCheck.Id != category.Id)
                return true;

            return false;
        }

        public async Task<Category> Create(Category category)
        {
            await _db.AddAsync(category);
            _db.SaveChanges();

            return category;
        }

        public void Edit(Category category)
        {
            _db.Update(category);
            _db.SaveChanges();
        }

        public void Delete(Category category)
        {
            _db.Remove(category);
            _db.SaveChanges();
        }
    }
}
