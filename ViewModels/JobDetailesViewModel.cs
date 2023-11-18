namespace JobOffers.ViewModels
{
    public class JobDetailesViewModel
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string Title { get; set; }
        public string CompanyName { get; set; }
        public string User { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public DateTime AddedOn { get; set; }
    }
}
