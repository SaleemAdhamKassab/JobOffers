using System.ComponentModel.DataAnnotations;

namespace JobOffers.ViewModels
{
    public class EditCategoryViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
