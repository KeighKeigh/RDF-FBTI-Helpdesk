using MakeItSimple.WebApi.Models.Setup.Phase_One.CategoryConcernSetup;

namespace MakeItSimple.WebApi.Models.Setup.Phase_One.ContractorSetup
{
    public class Contractor
    {
        public int Id { get; set; }
        public string ContractorName { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? DateUpdated { get; set; }
        public bool? IsActive { get; set; }

        public ICollection<ContractorChannel> ContractorChannels { get; set; }
    }
}
