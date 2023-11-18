namespace JobOffers.Models
{
    public class Job
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public DateTime AddedOn { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public List<ApplyJob> ApplyJob { get; set; }
    }
}