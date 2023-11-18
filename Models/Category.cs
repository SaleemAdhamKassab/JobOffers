namespace JobOffers.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<Job> Jobs { get; set; }
    }
}
