using System.ComponentModel.DataAnnotations;

namespace JobOffers.ViewModels
{
    public class AddCategoryViewModel
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
