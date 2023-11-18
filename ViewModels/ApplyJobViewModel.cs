using JobOffers.Models;

namespace JobOffers.ViewModels
{
    public class ApplyJobViewModel
    {
        public string UserId { get; set; }
        public int JobId { get; set; }
        public string? Message { get; set; }
    }
}