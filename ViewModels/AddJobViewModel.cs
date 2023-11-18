using JobOffers.Models;
using Microsoft.AspNetCore.Mvc;

namespace JobOffers.ViewModels
{
    public class AddJobViewModel
    {
        public int CategoryId { get; set; }
        public List<Category>? Categories { get; set; }
        public string Title { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public IFormFile Image { get; set; }
    }
}
