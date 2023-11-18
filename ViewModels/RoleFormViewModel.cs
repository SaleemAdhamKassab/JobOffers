using System.ComponentModel.DataAnnotations;

namespace JobOffers.ViewModels
{
    public class RoleFormViewModel
    {
        [Required, StringLength(256)]
        public string Name { get; set; }
    }
}
