using JobOffers.Interfaces;
using JobOffers.Models;
using JobOffers.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static JobOffers.Configurations.Helper;

namespace JobOffers.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class CategoriesController : Controller
    {
        private readonly ICategoriesRepo _categoriesRepo;
        public CategoriesController(ICategoriesRepo categoriesRepo) => _categoriesRepo = categoriesRepo;


        private async Task<bool> isNameUsedforAnotherCategory(string name)
        {
            Category category = await _categoriesRepo.Get(name);

            if (category != null)
                return true;

            return false;
        }


        //Get
        public async Task<IActionResult> Index() => View(await _categoriesRepo.Get());


        //Search
        public async Task<IActionResult> Search(string categoryName)
        {
            if (!String.IsNullOrEmpty(categoryName))
            {
                List<Category> categories = await _categoriesRepo.Search(categoryName);

                if (categories.Count > 0)
                    return View(nameof(Index), categories);
            }

            return RedirectToAction(nameof(Index));
        }


        //Create
        public async Task<IActionResult> Create() => View(new AddCategoryViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddCategoryViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid Model");

            try
            {
                Category category = await _categoriesRepo.Get(model.Name);

                if (category != null)
                    return BadRequest($"The Category ({model.Name}) is already exists!");


                Category categoryToAdd = new()
                {
                    Name = model.Name
                };

                await _categoriesRepo.Create(categoryToAdd);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        //Edit
        public async Task<IActionResult> Edit(int id)
        {
            Category category = await _categoriesRepo.Get(id);
            if (category == null)
                return NotFound();

            EditCategoryViewModel model = new()
            {
                Id = id,
                Name = category.Name,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditCategoryViewModel model)
        {
            Category category = await _categoriesRepo.Get(id);

            if (category == null)
                return BadRequest($"The Category ({model.Name}) doesn't exists!");

            if (category.Name.Trim() == model.Name)
                return BadRequest($"You have not made any changes!");

            if (await isNameUsedforAnotherCategory(model.Name))
                return BadRequest($"The Category ({model.Name}) is already exists!");

            try
            {
                category.Name = model.Name;
                _categoriesRepo.Edit(category);
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
            Category category = await _categoriesRepo.Get(id);

            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Category category)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid Model");

            _categoriesRepo.Delete(category);
            return RedirectToAction(nameof(Index));
        }
    }
}
