using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace JobOffers.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string LastName { get; set; }

        public string Image { get; set; }
        public string? CV { get; set; }

        public List<Job> Jobs { get; set; }
        public List<ApplyJob> ApplyJob { get; set; }
    }
}