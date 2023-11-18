namespace JobOffers.Models
{
    public class ApplyJob
    {
        public int Id { get; set; }
        public string? Message { get; set; }
        public DateTime AddedOn { get; set; }


        public string UserId { get; set; }
        public ApplicationUser User { get; set; }


        public int JobId { get; set; }
        public Job Job { get; set; }
    }
}
