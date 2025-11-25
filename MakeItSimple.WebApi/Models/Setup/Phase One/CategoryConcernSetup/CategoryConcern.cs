namespace MakeItSimple.WebApi.Models.Setup.Phase_One.CategoryConcernSetup
{
    public class CategoryConcern
    {

        public int Id { get; set; }
        public string CategoryConcernName { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? DateUpdated { get; set; }
        public bool? IsActive { get; set; }

        public ICollection<CategoryConcernChannel> CategoryConcernChannels { get; set; }
    }
}
